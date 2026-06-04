using MySqlConnector;
using System.Text;
using TronBeTongV3.CSDL;
using TronBeTongV3.CSDL.Server;

namespace TronBeTongV3.CSDL.Server
{
    public class DbServerSync
    {
        private DbBridge _db = DbBridge.Instance;
        public string SrvConnStr = "";

        /// <summary>
        /// {0}: table name, {1}: extra params with type, {2} [field=param], {3}: fields, {4}: params
        /// </summary>
        public static readonly string QueryUpsertTemplate = @"
            DROP PROCEDURE IF EXISTS srv_upsert_{0};

            CREATE PROCEDURE srv_upsert_{0} (
                IN p_local_id INT,
                IN p_src_id INT,
                IN p_local_updated_at TIMESTAMP,
                IN p_local_created_at TIMESTAMP,
                {1}
            )
            BEGIN
                DECLARE existing_id INT;

                -- Tìm id nếu đã tồn tại văn bản
                SELECT id INTO existing_id
                FROM {0}
                WHERE local_id = p_local_id AND source_id = p_src_id
                LIMIT 1;

                IF existing_id IS NOT NULL THEN
                    UPDATE {0}
                    SET local_updated_at=p_local_updated_at, local_created_at=p_local_created_at,{2}
                    WHERE id = existing_id;

                ELSE
                    INSERT IGNORE INTO {0} (local_id, source_id, local_updated_at, local_created_at, {3})
                    VALUES (p_local_id, p_src_id, p_local_updated_at, p_local_created_at, {4});
                END IF;
            END;";

        public void CreateConnStr(string srv, string db, string user, string pw)
        {
            SrvConnStr = string.Format("Server={0};Port={1};Database={2};User={3};Password={4};", srv, 3306, db, user, pw);
        }

        public async Task<List<TableInfo>> Init(string[] sync_tables)
        {
            List<TableInfo> _tblInfos = [];

            var localConn = await _db.OpenConnAsync();

            for (int i = 0; i < sync_tables.Length; i++)
            {
                var tbl = sync_tables[i];
                var tblInfo = await GetTableInfos(localConn, tbl);
                tblInfo.Index = i + 1;
                tblInfo.Build();
                _tblInfos.Add(tblInfo);
            }

            await _db.CloseConnAsync(localConn);

            return _tblInfos;
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                using var connection = new MySqlConnection(SrvConnStr);
                await connection.OpenAsync();

                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public async Task CreateServerTables(Dictionary<string, TableInfo> sync_tables)
        {
            using var srvConn = new MySqlConnection(SrvConnStr);
            await srvConn.OpenAsync();

            string query = @"CREATE TABLE IF NOT EXISTS sources (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ten VARCHAR(63), ma VARCHAR(128), flags INT,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            var cmd = new MySqlCommand(query, srvConn);
            await cmd.ExecuteNonQueryAsync();

            foreach (var table in sync_tables.Values)
            {
                await CreateServeTable(srvConn, table);
            }

            await srvConn.CloseAsync();
        }

        private async Task<TableInfo> GetTableInfos(MySqlConnection conn, string tblname)
        {
            var tblInfo = new TableInfo(tblname);

            // Lấy danh sách cột từ INFORMATION_SCHEMA
            string sqlCols = @"
                SELECT COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @table";
            var cmd = new MySqlCommand(sqlCols, conn);
            cmd.Parameters.AddWithValue("@table", tblname);

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string colName = reader.GetString("COLUMN_NAME");
                string colType = reader.GetString("COLUMN_TYPE");
                string nullable = reader.GetString("IS_NULLABLE") == "YES" ? "" : "NOT NULL";
                object? defaultVal = colType.Contains("VARCHAR") ? reader["COLUMN_DEFAULT"] : null;
                string extra = reader.GetString("EXTRA");
                string defaultStr = "";
                if (defaultVal != DBNull.Value && defaultVal != null)
                {
                    defaultStr = $"DEFAULT '{defaultVal}'";
                }

                if (colName == "updated_at") defaultStr = "DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP";
                else if (colName == "created_at") defaultStr = "DEFAULT CURRENT_TIMESTAMP";

                tblInfo.Columns.Add(new ColumnInfo(colName, colType, nullable, defaultStr, extra));
            }
            await reader.CloseAsync();

            return tblInfo;
        }

        private async Task CreateServeTable(MySqlConnection srvConn, TableInfo tblInfo)
        {
            List<string> paramWithTypes = [];
            List<string> fields = [];
            List<string> fieldEquParams = [];
            List<string> values = [];
            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE IF NOT EXISTS {tblInfo.TableName} (");

            // Thêm cột id server
            sb.AppendLine("  id INT AUTO_INCREMENT PRIMARY KEY,");

            // Thêm cột local_id, source_id
            sb.AppendLine("  local_id INT NOT NULL,");
            sb.AppendLine("  source_id INT NOT NULL,");
            sb.AppendLine("  local_updated_at TIMESTAMP,");
            sb.AppendLine("  local_created_at TIMESTAMP,");

            foreach (var i in tblInfo.Columns)
            {
                // Bỏ qua id cũ trong local
                if (i.ColName.Equals("id", StringComparison.OrdinalIgnoreCase)) continue;
                if (i.ColName.Equals("updated_at", StringComparison.OrdinalIgnoreCase)) continue;
                if (i.ColName.Equals("created_at", StringComparison.OrdinalIgnoreCase)) continue;

                sb.AppendLine($"  {i.ColName} {i.ColType} {i.Nullable} {i.DefaultValue} {i.Extra},");

                if (i.ColName != "id" && i.ColName != "updated_at" && i.ColName != "created_at")
                {
                    paramWithTypes.Add($"IN p_{i.ColName} {i.ColType}");
                    fields.Add(i.ColName);
                    fieldEquParams.Add($"{i.ColName}=p_{i.ColName}");
                    values.Add($"p_{i.ColName}");
                }
            }
            sb.AppendLine("  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,");
            sb.AppendLine("  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,");

            // Thêm unique key để tránh trùng local_id + source_id
            sb.AppendLine("  UNIQUE KEY uq_sync (source_id, local_id)");
            sb.AppendLine(") ENGINE=InnoDB;");

            string createSql = sb.ToString();

            // Tạo bảng trên server
            var cmdServer = new MySqlCommand(createSql, srvConn);
            await cmdServer.ExecuteNonQueryAsync();

            string createProc = string.Format(QueryUpsertTemplate,
                tblInfo.TableName,
                string.Join(",", paramWithTypes),
                string.Join(",", fieldEquParams),
                string.Join(",", fields),
                string.Join(",", values));

            cmdServer.CommandText = createProc;
            await cmdServer.ExecuteNonQueryAsync();
        }

        public Dictionary<string, int> SavedRecords { get; set; } = [];

        public async Task Server_Sync(Dictionary<string, TableInfo> sync_tables, long srcid)
        {
            SavedRecords.Clear();

            using var localConn = new MySqlConnection(_db.ConnStr);
            using var srvConn = new MySqlConnection(SrvConnStr);
            await localConn.OpenAsync();
            await srvConn.OpenAsync();

            foreach (var tbl in sync_tables.Values)
            {
                int n = await Server_Sync_Table(localConn, srvConn, tbl, srcid);
                SavedRecords[tbl.TableName] = n;
            }

            await srvConn.CloseAsync();
            await localConn.CloseAsync();
        }

        private async Task<int> Server_Sync_Table(MySqlConnection localConn, MySqlConnection srvConn, TableInfo tbl, long srcid)
        {
            int n = 0;

            var ids = await _db.Sync_GetIdsByTable(localConn, tbl.TableName, tbl.Index);
            if (ids.Count > 0)
            {
                string query = $"SELECT {tbl.SelectFields} FROM {tbl.TableName} WHERE id IN ({string.Join(",", ids)});";
                using var cmd = new MySqlCommand(query, localConn);
                var localRd = await cmd.ExecuteReaderAsync();

                using var transaction = await srvConn.BeginTransactionAsync();

                while (await localRd.ReadAsync())
                {
                    using var srvCmd = new MySqlCommand($"srv_upsert_{tbl.TableName}", srvConn);
                    srvCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    srvCmd.Transaction = transaction;
                    srvCmd.Prepare();

                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        var c = tbl.Columns[i];
                        srvCmd.Parameters.AddWithValue($"p_{c.TargetCol}", localRd.GetValue(i));
                    }
                    srvCmd.Parameters.AddWithValue($"p_src_id", srcid);

                    await srvCmd.ExecuteNonQueryAsync();
                    n++;
                }
                await localRd.CloseAsync();

                await transaction.CommitAsync();

                await _db.Sync_UpdateTimeAsync(localConn, tbl.Index);

                return n;
            }

            return 0;
        }

        public async Task<int> Server_Source_Init(string ten, string ma1, string? ma2)
        {
            using var conn = new MySqlConnection(SrvConnStr);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT id,ten,ma1,flags FROM sources WHERE ten=@ten AND ma1=@ma1;", conn);
            cmd.Parameters.AddWithValue("@ten", ten);
            cmd.Parameters.AddWithValue("@ma1", ma1);
            var reader = await cmd.ExecuteReaderAsync();

            int id = 0;
            int flags = 0;
            while (await reader.ReadAsync())
            {
                id = reader.GetInt32(0);
                flags = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                break;
            }
            await reader.CloseAsync();

            if (id == 0)
            {
                cmd.CommandText = "INSERT INTO sources(ten,ma1,ma2) VALUES (@ten,@ma1,@ma2);";
                cmd.Parameters.AddWithValue("@ma2", ma2);
                await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();

            if (id > 0)
                if ((flags & 1) == 1) return id;
                else return -id;
            return id;
        }

        public async Task<int> Server_Get_Factory_Id(string ma1)
        {
            using var conn = new MySqlConnection(SrvConnStr);
            await conn.OpenAsync();

            using var cmd = new MySqlCommand("SELECT id,ten,ma1,flags FROM sources WHERE ma1=@ma1;", conn);
            cmd.Parameters.AddWithValue("@ma1", ma1);
            var reader = await cmd.ExecuteReaderAsync();

            int id = 0;
            int flags = 0;
            while (await reader.ReadAsync())
            {
                id = reader.GetInt32(0);
                flags = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                break;
            }
            await reader.CloseAsync();

            await conn.CloseAsync();

            if (id > 0)
                if ((flags & 1) == 1) return id;
                else return -id;
            return id;
        }
    }
}
