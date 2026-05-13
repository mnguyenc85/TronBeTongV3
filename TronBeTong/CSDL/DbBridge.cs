using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using MySqlConnector;
using TronBeTongV3.Data.DO;
using TronBeTongV3.Data.DO.Business;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Data.DO.ThongKe;
using TronBeTongV3.Data.ViewModel;

namespace TronBeTongV3.CSDL
{
    public class DbBridge
    {
        #region Properties
        private static DbBridge? _instance;
        public static DbBridge Instance { get { return _instance ??= new DbBridge(); } }

        public string? ConnStr { get; private set; }

        public string? DbSrcName { get; set; }
        private readonly Dictionary<string, DbConfig> _dbConfigs = new();
        #endregion

        #region Init
        public DbBridge()
        {
            // TODO: encrypt
            _dbConfigs.Add("0", new DbConfig("localhost", -1, "TronBeTong3", "root", "ncmanh191"));
            _dbConfigs.Add("1", new DbConfig("localhost", 13306, "TronBeTong3", "root", "ncmanh191"));
            _dbConfigs.Add("2", new DbConfig("localhost", -1, "TronBeTong3", "root", "mocmeo1410"));
        }

        private string CreateConnStr(string srv, int port, string? db, string user, string pw)
        {
            if (port < 0)
            {
                if (db != null) return string.Format("Server={0};Database={1};User={2};Password={3};", srv, db, user, pw);
                else return string.Format("Server={0};User={1};Password={2};", srv, user, pw);
            }
            else
            {
                if (db != null) return string.Format("Server={0};Port={1};Database={2};User={3};Password={4};", srv, port, db, user, pw);
                else return string.Format("Server={0};Port={1};User={2};Password={3};", srv, port, user, pw);
            }
        }

        private string CreateConnStr(DbConfig c)
        {
            return CreateConnStr(c.Server, c.Port, c.Db, c.User, c.Pw);
        }

        public void Init()
        {
            if (string.IsNullOrEmpty(DbSrcName) || !_dbConfigs.ContainsKey(DbSrcName)) DbSrcName = "0";
            ConnStr = CreateConnStr(_dbConfigs[DbSrcName]);
        }

        /// <summary>
        /// Lưu tên csdl sử dụng vào Application Settings
        /// </summary>
        public void Save()
        {
            Properties.Settings.Default.DbSrcName = DbSrcName;
            Properties.Settings.Default.Save();
        }

        public async Task CreateDatabaseAsync()
        {
            if (DbSrcName == null) return;
            var c = _dbConfigs[DbSrcName];

            string connStr = CreateConnStr(c.Server, c.Port, null, c.User, c.Pw);
            using var connection = new MySqlConnection(connStr);
            await connection.OpenAsync();

            string query = $"CREATE DATABASE IF NOT EXISTS {c.Db} CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
            using var command = new MySqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        public async Task CreateTableAsync()
        {
            if (ConnStr == null) return;

            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            using var command = new MySqlCommand();
            command.Connection = connection;

            await PM_CreateTablesAsync(command);

            await KD_CreateTablesAsync(command);

            await CT_CreateTablesAsync(command);

            await HT_CreateTablesAsync(command);

            await Sync_CreateTablesAsync(command);

            await MySQLProceduces.ExecuteAddProcs(command);

            await connection.CloseAsync();
        }

        #endregion

        #region Dữ liệu của ứng dụng
        private async Task PM_CreateTablesAsync(MySqlCommand command)
        {
            // pm_nhanvien: username, pw, ten, mobile, role_flags, login
            string query = @"CREATE TABLE IF NOT EXISTS pm_nhanvien (
                id INT AUTO_INCREMENT PRIMARY KEY,
                username VARCHAR(31), pw VARCHAR(31),
                ten VARCHAR(100), mobile VARCHAR(21), role_flags INT, login INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // pm_settings: ten, giatri, kieu
            query = @"CREATE TABLE IF NOT EXISTS pm_settings (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ten VARCHAR(127) NOT NULL, giatri VARCHAR(255), kieu INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // pm_settings: ten, giatri, kieu
            query = @"CREATE TABLE IF NOT EXISTS pm_strings (
                id INT AUTO_INCREMENT PRIMARY KEY,
                vanban VARCHAR(255), phanloai INT, sudung INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // pm_activites: id, act_id, noidung
            query = @"CREATE TABLE IF NOT EXISTS pm_activities (
                id INT AUTO_INCREMENT PRIMARY KEY,
                act_cat INT, act_id INT, noidung VARCHAR(255),
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }

        #region pm_settings (id, ten, giatri, kieu)
        public async Task LoadSettingsAsync(DbSettings s)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = "SELECT id,ten,giatri,kieu FROM pm_settings";
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string ten = reader.IsDBNull(1) ? "": reader.GetString(1);
                    string? val = reader.IsDBNull(2) ? null : reader.GetString(2);
                    int kieu = reader.GetInt32(3);
                    s.LoadSetting(new SettingValueDO(ten) { Id = id, ValType = (SettingValueTypes)kieu, Value = val });
                }
            }
        }
        
        public async Task SaveSettingsAsync(DbSettings settings)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query;
            foreach (SettingValueDO s in settings.Data.Values)
            {
                if (s.Id <= 0)
                {
                    query = "INSERT INTO pm_settings(ten,giatri,kieu) VALUES (@ten,@gt,@kieu); SELECT LAST_INSERT_ID();";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ten", s.Name);
                    command.Parameters.AddWithValue("@gt", s.Value);
                    command.Parameters.AddWithValue("@kieu", s.ValType);
                    var ret = await command.ExecuteScalarAsync();
                    if (ret != null) s.Id = Convert.ToInt32(ret);
                }
                else if (s.Changed)
                {
                    query = "UPDATE pm_settings SET ten=@ten,giatri=@gt,kieu=@kieu WHERE id=@id;";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@id", s.Id);
                    command.Parameters.AddWithValue("@ten", s.Name);
                    command.Parameters.AddWithValue("@gt", s.Value);
                    command.Parameters.AddWithValue("@kieu", s.ValType);
                    await command.ExecuteNonQueryAsync();
                }
            }

            await connection.CloseAsync();
        }
        #endregion

        #region pm_nhanvien(username, pw, ten, mobile, role_flags, login)
        public async Task<List<PMNhanVienDO>> NhanVienSelectAllAsync(string? cond = null)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = cond == null?
                $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien;":
                $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE {cond};";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<PMNhanVienDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(PMNhanVienDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<List<PMNhanVienDO>> NhanVienSelectByUsernameAsync(string username)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE username LIKE %un;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("%un", $"%{username}%");
            using var reader = await command.ExecuteReaderAsync();

            List<PMNhanVienDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(PMNhanVienDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<PMNhanVienDO?> NhanVienGetByHoTenAsync(string hoten)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE ten=@hoten;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@hoten", hoten);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return PMNhanVienDO.FromDataReader(reader);
            }
            return null;
        }

        public async Task<int> NhanVienCountByFlagsAsync(int flags)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT COUNT(id) FROM pm_nhanvien WHERE role_flags&@flags=@flags;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@flags", flags);

            object? ret = await command.ExecuteScalarAsync();
            if (ret != null) return (int)(long)ret;
            return 0;
        }

        public async Task<PMNhanVienDO?> NhanVienGetByFlagsAsync(int flags)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE role_flags&@flags=@flags;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@flags", flags);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) {
                return PMNhanVienDO.FromDataReader(reader);
            }
            return null;
        }

        public async Task<PMNhanVienDO?> NhanVienGetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE id={id};";
            using var command = new MySqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return PMNhanVienDO.FromDataReader(reader);
            }
            return null;
        }

        public async Task<List<PMNhanVienDO>> NhanVienGetListByFlagsAsync(int[] flags)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE role_flags IN ({string.Join(',', flags)});"; 
            using var command = new MySqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            var lst = new List<PMNhanVienDO>();
            while (await reader.ReadAsync())
            {
                lst.Add(PMNhanVienDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<bool> NhanVienInsertNoAccAsync(PMNhanVienDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id <= 0)
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
            return true;
        }

        public async Task<bool> NhanVienSaveAsync(PMNhanVienDO m, string oldpw = "", bool isAdmin = false)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                if (!isAdmin)
                    if (!await NhanVienCheckPwAsync(conn, m, oldpw))
                        return false;

                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
            return true;
        }

        public async Task<PMNhanVienDO?> NhanVienLoginAsync(string username, string pw)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {PMNhanVienDO.SelectFields} FROM pm_nhanvien WHERE username=@user AND pw=@pw;";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@user", username);
            command.Parameters.AddWithValue("@pw", pw);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return PMNhanVienDO.FromDataReader(reader);
            }
            return null;
        }

        public async void NhanVienUpdateCountAsync(int id, int count)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = "UPDATE pm_nhanvien SET login=@login WHERE id=@id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@login", count);

            await command.ExecuteNonQueryAsync();
        }

        private async Task<bool> NhanVienCheckPwAsync(MySqlConnection conn, PMNhanVienDO m, string pw)
        {
            string query = "SELECT COUNT(id) FROM pm_nhanvien WHERE id=@id AND pw=@pw;";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@pw", pw);
            var ret = await cmd.ExecuteScalarAsync();
            return ret != null && (long)ret > 0;
        }
        #endregion

        #region pm_strings(vanban,phanloai,sudung)
        public async Task<List<StringDO>> PM_Strings_SelectAsync(string? cond)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = cond == null ?
                $"SELECT {StringDO.SelectFields} FROM pm_strings ORDER BY id;" :
                $"SELECT {StringDO.SelectFields} FROM pm_strings WHERE {cond} ORDER BY id;";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<StringDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(StringDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task PM_Strings_SaveAsync(MySqlConnection conn, StringDO s)
        {
            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("pm_upsert_string", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_vanban", s.VanBan);
                command.Parameters.AddWithValue("p_phanloai", s.PhanLoai);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                s.Id = resultId;
            }
        }
        #endregion

        #region Activities
        public async Task PM_Activites_SaveAsync(int act_cat, int act_id, string msg)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = "INSERT INTO pm_activities(act_cat, act_id, noidung) VALUES (@act_cat,@act_id,@nd); SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@act_cat", act_cat);
            command.Parameters.AddWithValue("@act_id", act_id);
            command.Parameters.AddWithValue("@nd", msg);
            var ret = await command.ExecuteScalarAsync();
            //if (ret != null) s.Id = Convert.ToInt32(ret);
        }
        
        public async Task PM_Activites_DelAsync()
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = "DELETE FROM pm_activities WHERE created_at < @tg;";
            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@tg", DateTime.Now - new TimeSpan(60, 0, 0, 0));
            var ret = await command.ExecuteScalarAsync();
            //if (ret != null) s.Id = Convert.ToInt32(ret);
        }
        #endregion
        #endregion

        #region Kinh doanh
        private async Task KD_CreateTablesAsync(MySqlCommand command)
        {
            // kd_khachhang: ma, ten, diachi, sdt, email, mst, lienhe, ghichu
            string query = @"CREATE TABLE IF NOT EXISTS kd_khachhang (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63), ten VARCHAR(255),
                diachi VARCHAR(255), sdt VARCHAR(31), email VARCHAR(63),
                mst VARCHAR(63), lienhe VARCHAR(63), ghichu VARCHAR(255),
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // kd_duan: duan,congtrinh,hangmuc,diachi,ghichu,kh_id
            query = @"CREATE TABLE IF NOT EXISTS kd_duan (
                id INT AUTO_INCREMENT PRIMARY KEY,
                duan INT, congtrinh INT, hangmuc INT, diachi INT, 
                ghichu VARCHAR(255), kh_id INT DEFAULT 0,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // kd_xe: bsx,dungtich,dungtichmin
            query = @"CREATE TABLE IF NOT EXISTS kd_xe (
                id INT AUTO_INCREMENT PRIMARY KEY,
                bsx VARCHAR(63), dungtich DOUBLE, dungtichmin DOUBLE,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
            
            // kd_laixe: ten,sdt
            query = @"CREATE TABLE IF NOT EXISTS kd_laixe (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ten VARCHAR(127), sdt VARCHAR(31),
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // kd_xelx: xe_id,lx_id
            query = @"CREATE TABLE IF NOT EXISTS kd_xelx (
                xe_id INT, lx_id INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (xe_id, lx_id));";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }

        #region kd_khachang(ma,ten,diachi,sdt,email,mst,lienhe,ghichu)
        public async Task<KDKhachHangDO?> KhachHangGetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {KDKhachHangDO.SelectFields} FROM kd_khachhang WHERE id={id};";
            using var command = new MySqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return KDKhachHangDO.FromDataReader(reader);
            }
            return null;
        }

        public async Task<List<KDKhachHangDO>> KhachHangSelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDKhachHangDO.SelectFields} FROM kd_khachhang ORDER BY id LIMIT {i0},{n};" :
                $"SELECT {KDKhachHangDO.SelectFields} FROM kd_khachhang ORDER BY id;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDKhachHangDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDKhachHangDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task KhachHangSaveAsync(KDKhachHangDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        #endregion

        #region kd_duan(duan,congtrinh,hangmuc,diachi,ghichu,kh_id)
        public async Task<List<KDDuAnDO>> KD_DuAn_SelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDDuAnDO.SelectFields} FROM kd_duan ORDER BY id LIMIT {i0},{n};" :
                $"SELECT {KDDuAnDO.SelectFields} FROM kd_duan ORDER BY id;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDDuAnDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDDuAnDO.FromDataReader(reader));
            }
            return lst;
        }

        /// <summary>
        /// Không dùng: sử dụng KD_DuAn_SaveProcAsync
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task KD_DuAn_SaveAsync(MySqlConnection conn, KDDuAnDO m)
        {
            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }

        public async Task KD_DuAn_SaveProcAsync(MySqlConnection conn, KDDuAnDO d)
        {
            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("kd_upsert_duan", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_inid", d.Id);
                command.Parameters.AddWithValue("p_duan", d.DuAn);
                command.Parameters.AddWithValue("p_congtrinh", d.CongTrinh);
                command.Parameters.AddWithValue("p_hangmuc", d.HangMuc);
                command.Parameters.AddWithValue("p_diachi", d.DiaChi);
                command.Parameters.AddWithValue("p_ghichu", d.GhiChu);
                command.Parameters.AddWithValue("p_khid", d.KHId);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                d.Id = resultId;
            }
        }
        #endregion

        #region kd_xe: bsx,dungtich,dungtichmin
        public async Task<List<KDXeDO>> Xe_SelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDXeDO.SelectFields} FROM kd_xe ORDER BY id DESC LIMIT {i0},{n};" :
                $"SELECT {KDXeDO.SelectFields} FROM kd_xe ORDER BY id DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDXeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDXeDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<List<KDXeDO>> Xe_SelectByXeLXAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDXeDO.SelectFields} FROM kd_xe WHERE id IN (SELECT DISTINT xe_id FROM kd_xelx LIMIT {i0},{n}) ORDER BY id" :
                $"SELECT {KDXeDO.SelectFields} FROM kd_xe WEHRE id IN (SELECT DISTINT xe_id FROM kd_xelx)  ORDER BY id;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDXeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDXeDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task Xe_SaveAsync(KDXeDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        #endregion

        #region kd_laixe(ten,sdt)
        public async Task<List<KDLaiXeDO>> LaiXeSelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDLaiXeDO.SelectFields} FROM kd_laixe ORDER BY id DESC LIMIT {i0},{n};" :
                $"SELECT {KDLaiXeDO.SelectFields} FROM kd_laixe ORDER BY id DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDLaiXeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDLaiXeDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<List<KDLaiXeDO>> LaiXeSelectByXeLXAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {KDLaiXeDO.SelectFields} FROM kd_laixe WHERE id IN (SELECT DISTINT lx_id FROM kd_xelx ORDER BY id DESC LIMIT {i0},{n};" :
                $"SELECT {KDLaiXeDO.SelectFields} FROM kd_laixe WEHRE id IN (SELECT DISTINT lx_id FROM kd_xelx ORDER BY id DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDLaiXeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(KDLaiXeDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task LaiXeSaveAsync(KDLaiXeDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        #endregion

        #region kd_xelx(xe_id,lx_id)
        public async Task<List<KDXeLaiXeDO>> XLxSelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT xe_id,lx_id FROM kd_xelx ORDER BY created_at DESC LIMIT {i0},{n};" :
                $"SELECT xe_id,lx_id FROM kd_xelx ORDER BY created_at DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<KDXeLaiXeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(new KDXeLaiXeDO()
                {
                    XeId = reader.GetInt32(0),
                    LaiXeId = reader.GetInt32(1)
                });
            }
            return lst;
        }

        public async Task XLxInsertAsync(int xeid, int lxid)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"INSERT IGNORE INTO kd_xelx(xe_id,lx_id) VALUES({xeid}, {lxid});";
            using var command = new MySqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }
        #endregion
        #endregion

        #region Công thức (cấp phối)
        #region ct_nguyenlieu:ma,ten,phanloai,doam
        public async Task<List<SiloNguyenLieuDO>> NguyenLieu_SelectLimitAsync(int i0, int n)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = n > 0 ?
                $"SELECT {SiloNguyenLieuDO.SelectFields} FROM ct_nguyenlieu ORDER BY id DESC LIMIT {i0},{n};" :
                $"SELECT {SiloNguyenLieuDO.SelectFields} FROM tblsilonguct_nguyenlieuyenlieu ORDER BY id DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<SiloNguyenLieuDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(SiloNguyenLieuDO.FromDataReader(reader));
            }
            return lst;
        }
        
        /// <summary>
        /// Thay bằng NguyenLieu_SaveProcAsync
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private async Task NguyenLieu_SaveAsync(SiloNguyenLieuDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        
        public async Task NguyenLieu_SaveProcAsync(SiloNguyenLieuDO m)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("ct_save_nguyenlieu_unique_ma", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_inid", m.Id);
                command.Parameters.AddWithValue("p_ma", m.Ma);
                command.Parameters.AddWithValue("p_ten", m.Ten);
                command.Parameters.AddWithValue("p_pl", m.PhanLoai);
                command.Parameters.AddWithValue("p_doam", m.DoAm);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                m.Id = resultId;
            }
        }
        #endregion

        #region ct_silo:phanloai,stt,nl_id,ts_id
        public async Task<List<SiloCauHinhDO>> Silo_SelectAllAsync()
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {SiloCauHinhDO.SelectFields} FROM ct_silo;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<SiloCauHinhDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(SiloCauHinhDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task Silo_SaveAsync(SiloCauHinhDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        #endregion

        #region Công thức
        private async Task CT_CreateTablesAsync(MySqlCommand command)
        {
            // silo
            string query = @"CREATE TABLE IF NOT EXISTS ct_nguyenlieu (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63), ten VARCHAR(63), phanloai INT, doam DOUBLE,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            query = @"CREATE TABLE IF NOT EXISTS ct_silo_thamso (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63),
                klxacham DOUBLE, chukynhay DOUBLE, tgdongnhay DOUBLE,
                flags INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            query = @"CREATE TABLE IF NOT EXISTS ct_silo (
                id INT AUTO_INCREMENT PRIMARY KEY,
                phanloai INT, stt INT,
                nl_id INT, ts_id INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // Công thức bê tông
            query = @"CREATE TABLE IF NOT EXISTS ct_congthuc (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63),
                mac VARCHAR(63), slump VARCHAR(31), wcratio DOUBLE, klnuoc DOUBLE, kthat DOUBLE,
                sotp INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            query = @"CREATE TABLE IF NOT EXISTS ct_thanhphan (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ct_id INT, nl_id INT, kl DOUBLE,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<BTCongThucDO>> BTCongThuc_SelectAllAsync()
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {BTCongThucDO.SelectFields} FROM ct_congthuc;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<BTCongThucDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(BTCongThucDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task BTCongThuc_SaveAsync(BTCongThucDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn, "ct_congthuc");
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn, "ct_congthuc");
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }

        public async Task BTThanhPhan_DeleteByCTAsync(int ctid)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            string query = $"DELETE FROM ct_thanhphan WHERE ct_id = {ctid};";
            using var command = new MySqlCommand(query, conn);
            await command.ExecuteNonQueryAsync();
        }

        public async Task BTThanhPhan_DeleteAsync(int id)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            string query = $"DELETE FROM ct_thanhphan WHERE id = {id};";
            using var command = new MySqlCommand(query, conn);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<BTThanhPhanDO>> BTThanhPhan_SelectByCTIdAsync(int ctid)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $"SELECT {BTThanhPhanDO.SelectFields} FROM ct_thanhphan WHERE ct_id={ctid};";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<BTThanhPhanDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(BTThanhPhanDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task BTThanhPhan_SaveNewAsync(BTCongThucVM ct)
        {
            if (ct.Id <= 0) return;

            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            string query = $"DELETE FROM ct_thanhphan WHERE ct_id = {ct.Id};";
            using var command = new MySqlCommand(query, conn);
            await command.ExecuteNonQueryAsync();

            using (var transaction = await conn.BeginTransactionAsync())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "INSERT INTO ct_thanhphan(ct_id,nl_id,kl) VALUES (@ctid,@nlid,@kl)";
                    cmd.Parameters.Add("@ctid", MySqlDbType.Int32);
                    cmd.Parameters.Add("@nlid", MySqlDbType.Int32);
                    cmd.Parameters.Add("@kl", MySqlDbType.Double);
                    cmd.Prepare();

                    foreach (var tp in ct.DsThanhPhan)
                    {
                        if (tp.NL != null && tp.NL.PhanLoai != Core.LoaiThanhPhan.Nuoc && tp.KL > 0)
                        {
                            cmd.Parameters["@ctid"].Value = ct.Id;
                            cmd.Parameters["@nlid"].Value = tp.NL.Id;
                            cmd.Parameters["@kl"].Value = tp.KL;
                            await cmd.ExecuteNonQueryAsync();
                            tp.State = Core.ViewModelStates.None;
                        }
                    }
                }
                await transaction.CommitAsync();
            }
            await conn.CloseAsync();
        }

        public async Task BTThanhPhan_SaveAsync(BTThanhPhanDO m)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn);
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }

        public async Task BTCongThuc_ProcDeleteAsync(int id)
        {
            using var conn = new MySqlConnection(ConnStr);
            await conn.OpenAsync();

            using var command = new MySqlCommand("ct_delete_by_id", conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("p_id", id);
            await command.ExecuteNonQueryAsync();
        }
        #endregion
        #endregion

        #region Hoàn thành
        private async Task HT_CreateTablesAsync(MySqlCommand command)
        {
            // Lưu thông tin đơn hàng
            string query = @"CREATE TABLE IF NOT EXISTS ht_donhang (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63),
                kh_id INT, da_id INT, ct_id INT, hm_id INT, diachi_id INT,thetichdh DOUBLE,
                ghichu VARCHAR(255), trangthai INT,
                thetichht DOUBLE, klht DOUBLE, meht INT, tght DATETIME,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            //// Lưu thông tin phiếu cân
            query = @"CREATE TABLE IF NOT EXISTS ht_phieu (
                id INT AUTO_INCREMENT PRIMARY KEY,
                sophieu VARCHAR(63), donhang_id INT,
                xe_id INT, lx_id INT, congthuc_id INT, 
                thetichdat DOUBLE, kldat DOUBLE, medat INT,
                thetichht DOUBLE, klht DOUBLE, meht INT, 
                tgbd DATETIME, tght DATETIME,
                dongia DOUBLE, 
                don_stt INT, don_tt DOUBLE, don_stt_sim INT, don_tt_sim DOUBLE,
                trangthai INT, ghichu VARCHAR(255), kepchi VARCHAR(127),
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // Lưu công thức đi kèm phiếu cân
            query = @"CREATE TABLE IF NOT EXISTS ht_congthuc (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63),
                mac VARCHAR(63), slump VARCHAR(31), wcratio DOUBLE, kthat DOUBLE,
                klnuoc DOUBLE, sotp INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // Lưu từng thành phần trong công thức đơn hàng
            query = @"CREATE TABLE IF NOT EXISTS ht_thanhphan (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ma VARCHAR(63), ten VARCHAR(127), phanloai INT, silo INT,
                klcongthuc DOUBLE, kltong DOUBLE, klme DOUBLE,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // ht_congthuc <-> ht_thanhphan
            query = @"CREATE TABLE IF NOT EXISTS ht_congthuc_thanhphan (
                id INT AUTO_INCREMENT PRIMARY KEY,
                ct_id INT, tp_id INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            // Lưu mẻ trộn
            query = @"CREATE TABLE IF NOT EXISTS ht_me (
                id INT AUTO_INCREMENT PRIMARY KEY,
                phieu_id INT, stt INT, m3tron DOUBLE,
                cl1 DOUBLE, cl2 DOUBLE, cl3 DOUBLE, cl4 DOUBLE, cl5 DOUBLE, cl6 DOUBLE,
                xi1 DOUBLE, xi2 DOUBLE, xi3 DOUBLE, xi4 DOUBLE, xi5 DOUBLE,
                pg1 DOUBLE, pg2 DOUBLE, pg3 DOUBLE, pg4 DOUBLE, pg5 DOUBLE, pg6 DOUBLE, pg7 DOUBLE, pg8 DOUBLE,
                hu1 DOUBLE, hu2 DOUBLE, hu3 DOUBLE, hu4 DOUBLE, hu5 DOUBLE, hu6 DOUBLE,
                nuoc DOUBLE, flags INT,
                updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();

            //// Lưu từng thành phần trong mẻ trộn
            //query = @"CREATE TABLE IF NOT EXISTS ht_me_can (
            //    id INT AUTO_INCREMENT PRIMARY KEY,
            //    me_id INT, phanloai INT, silo INT, klcan DOUBLE);";
            //command.CommandText = query;
            //await command.ExecuteNonQueryAsync();

            // TODO: thêm bảng lưu thông tin kinh doanh
        }

        #region Đơn hàng
        public async IAsyncEnumerable<DHDonDO> HT_DonHang_SelectOnlineAsync()
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = $@"SELECT {DHDonDO.SelectedFields} WHERE a.trangthai=0 GROUP BY a.id ORDER BY created_at DESC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return DHDonDO.FromDataReader(reader);
            }
        }

        /// <summary>
        /// Sử dụng DonHang_SaveProcAsync thay thế
        /// </summary>
        private async Task HT_DonHang_SaveAsync(MySqlConnection conn, DHDonDO d)
        {
            if (d.Id > 0)
            {
                //using var command = d.CreateUpdate(conn, "ht_donhang");
                //await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = d.CreateInsert(conn, "ht_donhang");
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) d.Id = Convert.ToInt32(ret);
            }
        }
        
        public async Task HT_DonHang_ProcSaveAsync(DHDonDO d)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("ht_upsert_donhang", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_inid", d.Id);
                command.Parameters.AddWithValue("p_ma", d.Ma);
                
                command.Parameters.AddWithValue("p_duan", d.DuAn);
                command.Parameters.AddWithValue("p_congtrinh", d.CongTrinh);
                command.Parameters.AddWithValue("p_hangmuc", d.HangMuc);
                command.Parameters.AddWithValue("p_diachi", d.DiaChi);
                command.Parameters.AddWithValue("p_thetich", d.TheTichDH);

                command.Parameters.AddWithValue("p_ghichu", d.GhiChu);
                command.Parameters.AddWithValue("p_khid", d.KhachHangId);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                d.Id = resultId;
            }
        }

        public async Task<long> HT_DonHang_GetTotalOrdersInDayAsync(MySqlConnection conn, DateTime tg)
        {
            string query = "SELECT COUNT(id) FROM ht_donhang WHERE created_at >= @tg0 AND created_at <= @tg1;";
            using var command = new MySqlCommand(query, conn);
            DateTime tg0 = new(tg.Year, tg.Month, tg.Day, 0, 0, 0);
            DateTime tg1 = tg0 + new TimeSpan(24, 0, 0);
            command.Parameters.AddWithValue("@tg0", tg0);
            command.Parameters.AddWithValue("@tg1", tg1);
            object? ret = await command.ExecuteScalarAsync();
            if (ret != null) return (long)ret;
            return 0;
        }
        #endregion

        #region Phiếu
        public async Task HT_Phieu_SaveAsync(MySqlConnection conn, DHPhieuDO m)
        {
            if (m.Id > 0)
            {
                //using var command = m.CreateUpdate(conn);
                //await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn, "ht_phieu");
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }

        public async Task HT_Phieu_UpdateHoanThanhAsync(MySqlConnection conn, DHPhieuDO ph)
        {
            string query = $"UPDATE ht_phieu SET thetichht=@ttht,klht=@klht,meht=@meht,tght=@tght,don_stt=@dstt,don_tt=@dm3,don_stt_sim=@dsttsim,don_tt_sim=@dm3sim WHERE id={ph.Id};";
            using var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ttht", ph.TheTichHT);
            command.Parameters.AddWithValue("@klht", ph.KLHT);
            command.Parameters.AddWithValue("@meht", ph.MeHT);
            command.Parameters.AddWithValue("@tght", ph.TGHT);
            command.Parameters.AddWithValue("@dstt", ph.DonStt);
            command.Parameters.AddWithValue("@dm3", ph.DonM3);
            command.Parameters.AddWithValue("@dsttsim", ph.DonSttSim);
            command.Parameters.AddWithValue("@dm3sim", ph.DonM3Sim);

            await command.ExecuteNonQueryAsync();
        }
        public async Task HT_Phieu_UpdateUserInfoAsync(DHPhieuDO ph)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"UPDATE ht_phieu SET kepchi=@kepchi WHERE id={ph.Id};";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@kepchi", ph.KepChi);

            await command.ExecuteNonQueryAsync();

            await connection.CloseAsync();
        }

        public async Task<List<DHPhieuDO>> HT_Phieu_LoadByDH(MySqlConnection conn, int dhid)
        {
            string query = $"SELECT {DHPhieuDO.SelectedFields} FROM ht_phieu WHERE donhang_id={dhid} ORDER BY created_at;";
            using var command = new MySqlCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();

            List<DHPhieuDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(DHPhieuDO.FromDataReader(reader));
            }
            return lst;
        }

        public async Task<List<DHPhieuDO>> HT_Phieu_GetUnfinishedByDH(MySqlConnection conn, int dhid)
        {
            string query = $"SELECT {DHPhieuDO.SelectedFields} FROM ht_phieu WHERE donhang_id={dhid} AND (meht < medat AND trangthai = 0) ORDER BY created_at DESC;";
            using var command = new MySqlCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();

            List<DHPhieuDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(DHPhieuDO.FromDataReader(reader));
            }
            return lst;
        }
        #endregion

        #region Công thức
        public async Task HT_Phieu_CongThuc_ProcSaveAsync(MySqlConnection conn, BTCongThucDO ct, string dstp)
        {
            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("ht_upsert_congthuc", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_ma", ct.Ma);
                command.Parameters.AddWithValue("p_mac", ct.Mac);
                command.Parameters.AddWithValue("p_slump", ct.Slump);
                command.Parameters.AddWithValue("p_wcratio", ct.WCRatio);
                command.Parameters.AddWithValue("p_klnuoc", ct.KLNuoc);
                command.Parameters.AddWithValue("p_sotp", ct.SoTP);
                command.Parameters.AddWithValue("p_dstp", dstp);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                ct.Id = resultId;
            }
        }

        public async Task<BTCongThucDO?> HT_Phieu_CongThuc_LoadByIdAsync(MySqlConnection conn, int id)
        {
            string query = $"SELECT {BTCongThucDO.SelectFields} FROM ht_congthuc WHERE id={id};";
            using var command = new MySqlCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return BTCongThucDO.FromDataReader(reader);
            }
            return null;
        }

        public async Task<List<BTCongThucDO>> HT_Phieu_CongThuc_SelectAsync(string cond, string[]? vars)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = string.IsNullOrEmpty(cond)?
                $"SELECT {BTCongThucDO.SelectFields} FROM ht_congthuc;":
                $"SELECT {BTCongThucDO.SelectFields} FROM ht_congthuc WHERE {cond};";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            if (vars != null)
                for (int i = 0; i < vars.Length; i++)
                    command.Parameters.AddWithValue($"@p{i}", vars[i]);

            List<BTCongThucDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(BTCongThucDO.FromDataReader(reader));
            }

            await connection.CloseAsync();

            return lst;
        }

        public async Task HT_Phieu_CongThuc_ThanhPhan_SaveAsync(MySqlConnection conn, int ctid, DHThanhPhanDO tp)
        {
            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("ht_upsert_congthuc_thanhphan", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_ctid", ctid);
                command.Parameters.AddWithValue("p_ma", tp.NL_Ma);
                command.Parameters.AddWithValue("p_ten", tp.NL_Ten);
                command.Parameters.AddWithValue("p_pl", tp.NL_PhanLoai);
                command.Parameters.AddWithValue("p_silo", tp.NL_Silo);
                command.Parameters.AddWithValue("p_klcongthuc", tp.KLCongThuc);
                command.Parameters.AddWithValue("p_kltong", 0);
                command.Parameters.AddWithValue("p_klme", 0);

                // Thêm tham số đầu ra
                var idParam = new MySqlParameter("p_id", MySqlDbType.Int32)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(idParam);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();

                // Lấy giá trị id từ tham số đầu ra
                int resultId = Convert.ToInt32(idParam.Value);
                tp.Id = resultId;
            }
        }

        public async Task HT_CongThuc_ThanhPhan_SaveRelsAysnc(MySqlConnection conn, int ctid, int[] tpids)
        {
            using (var transaction = await conn.BeginTransactionAsync())
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "ht_upsert_ct_tp_rel";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ct_id", MySqlDbType.Int32);
                    cmd.Parameters.Add("p_tp_id", MySqlDbType.Int32);
                    cmd.Prepare();

                    foreach (var tpid in tpids)
                    {
                        cmd.Parameters["p_ct_id"].Value = ctid;
                        cmd.Parameters["p_tp_id"].Value = tpid;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                await transaction.CommitAsync();
            }
        }

        public async IAsyncEnumerable<DHThanhPhanDO> HT_Phieu_CongThuc_ThanhPhan_LoadAsync(MySqlConnection conn, int ctid)
        {
            string query = $"SELECT {DHThanhPhanDO.SelectFields} FROM ht_thanhphan WHERE id IN (SELECT tp_id FROM ht_congthuc_thanhphan WHERE ct_id = {ctid}) ORDER BY phanloai ASC;";
            using var command = new MySqlCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return DHThanhPhanDO.FromDataReader(reader);
            }
        }

        public async Task<List<DHThanhPhanDO>> HT_Phieu_CongThuc_ThanhPhan_LoadAsync(int ctid)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"SELECT {DHThanhPhanDO.SelectFields} FROM ht_thanhphan WHERE id IN (SELECT tp_id FROM ht_congthuc_thanhphan WHERE ct_id = {ctid}) ORDER BY phanloai ASC;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<DHThanhPhanDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(DHThanhPhanDO.FromDataReader(reader));
            }

            await connection.CloseAsync();

            return lst;
        }

        public async Task<string?> HT_Phieu_GetLastSoPhieuAsync(MySqlConnection conn)
        {
            string query = "SELECT sophieu FROM ht_phieu ORDER BY created_at DESC LIMIT 1;";
            using var command = new MySqlCommand(query, conn);
            object? ret = await command.ExecuteScalarAsync();
            if (ret != null) return ret as string;
            return null;
        }

        public async Task HT_Phieu_SetTrangThai(int id, int tt)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"UPDATE ht_phieu SET trangthai={tt} WHERE id={id};";
            using var command = new MySqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();

            await connection.CloseAsync();
        }

        public async Task<string?> HT_CongThuc_ByLastDonHang(int donid)
        {
            string? ma = null;

            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"SELECT ma FROM ht_congthuc WHERE id = (SELECT congthuc_id FROM ht_phieu WHERE donhang_id ={donid} ORDER BY updated_at DESC LIMIT 1);";
            using var command = new MySqlCommand(query, connection);
            var ret = await command.ExecuteScalarAsync();

            if (ret != null) ma = ret as string;

            await connection.CloseAsync();

            return ma;
        }
        #endregion

        #region Mẻ
        public async Task Me_SaveAsync(MySqlConnection conn, DHMeDO m)
        {
            if (m.Id > 0)
            {
                using var command = m.CreateUpdate(conn, "ht_me");
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                using var command = m.CreateInsert(conn, "ht_me");
                var ret = await command.ExecuteScalarAsync();
                if (ret != null) m.Id = Convert.ToInt32(ret);
            }
        }
        public async Task Me_SaveTTAsync(MySqlConnection conn, DHMeDO m)
        {
            if (m.Id > 0)
            {
                using var command = new MySqlCommand("UPDATE ht_me SET flags=@flags WHERE id=@id;", conn);
                command.Parameters.AddWithValue("@flags", m.Flags);
                command.Parameters.AddWithValue("@id", m.Id);
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task Me_SaveTTAsync(DHMeDO m)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            if (m.Id > 0)
            {
                using var command = new MySqlCommand("UPDATE ht_me SET flags=@flags WHERE id=@id;", connection);
                command.Parameters.AddWithValue("@flags", m.Flags);
                command.Parameters.AddWithValue("@id", m.Id);
                await command.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="pid">Id của phiếu</param>
        /// <param name="mophong">Load mẻ mô phỏng</param>
        /// <returns></returns>
        public async Task<List<DHMeDO>> HT_Me_LoadByPhieu(MySqlConnection conn, int pid, bool mophong)
        {
            string query = mophong?
                $"SELECT {DHMeDO.SelectedFields} FROM ht_me WHERE phieu_id={pid} ORDER BY created_at;":
                $"SELECT {DHMeDO.SelectedFields} FROM ht_me WHERE phieu_id={pid} AND (flags & 4) <> 4 ORDER BY created_at;";
            using var command = new MySqlCommand(query, conn);
            using var reader = await command.ExecuteReaderAsync();

            List<DHMeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(DHMeDO.FromDataReader(reader));
            }
            return lst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid">Id của phiếu</param>
        /// <param name="mophong">Load mẻ mô phỏng</param>
        /// <returns></returns>
        public async Task<List<DHMeDO>> HT_Me_LoadByPhieu(int pid, bool mophong)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = mophong ?
                $"SELECT {DHMeDO.SelectedFields} FROM ht_me WHERE phieu_id={pid} ORDER BY created_at;" :
                $"SELECT {DHMeDO.SelectedFields} FROM ht_me WHERE phieu_id={pid} AND (flags & 4) <> 4 ORDER BY created_at;";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            List<DHMeDO> lst = new();
            while (await reader.ReadAsync())
            {
                lst.Add(DHMeDO.FromDataReader(reader));
            }

            await connection.CloseAsync();

            return lst;
        }
        #endregion
        #endregion

        #region Thống kê
        public async Task<List<DHDonDO>> ThongKe_DonHangAsync(string cond, string[] vars)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = string.IsNullOrEmpty(cond) ?
                $"SELECT {DHDonDO.TKFields} FROM ht_donhang;" :
                $"SELECT {DHDonDO.TKFields} FROM ht_donhang WHERE {cond};";

            using var command = new MySqlCommand(query, connection);
            if (vars != null)
                for (int i = 0; i < vars.Length; i++)
                    command.Parameters.AddWithValue($"@p{i}", vars[i]);

            var reader = await command.ExecuteReaderAsync();

            List<DHDonDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(DHDonDO.FromTKDataReader(reader));
            }

            return lst;
        }

        public async Task<List<DHPhieuDO>> ThongKe_PhieuAsync(string cond, string[] vars)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = string.IsNullOrEmpty(cond) ?
                $"SELECT {DHPhieuDO.SelectedFields} FROM ht_phieu;" :
                $"SELECT {DHPhieuDO.SelectedFields} FROM ht_phieu WHERE {cond};";

            using var command = new MySqlCommand(query, connection);
            if (vars != null)
                for (int i = 0; i < vars.Length; i++)
                    command.Parameters.AddWithValue($"@p{i}", vars[i]);

            var reader = await command.ExecuteReaderAsync();

            List<DHPhieuDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(DHPhieuDO.FromDataReader(reader));
            }

            return lst;
        }

        public async Task<List<TKCtTpDO>> ThongKe_CongThuc_ThanhPhan_Rel_Async(int[] ctids)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"SELECT ct_id, tp_id FROM ht_congthuc_thanhphan WHERE ct_id IN ({string.Join(",", ctids)});";

            using var command = new MySqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            List<TKCtTpDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(new TKCtTpDO()
                {
                    CTId = reader.GetInt32(0),
                    TPId = reader.GetInt32(1)
                });
            }

            return lst;
        }

        public async Task<List<DHThanhPhanDO>> ThongKe_CongThuc_ThanhPhan_Async(int[] tpids)
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();

            string query = $"SELECT {DHThanhPhanDO.SelectFields} FROM ht_thanhphan WHERE id IN ({string.Join(",", tpids)}) ORDER BY ma ASC;";

            using var command = new MySqlCommand(query, connection);
            var reader = await command.ExecuteReaderAsync();

            List<DHThanhPhanDO> lst = [];
            while (await reader.ReadAsync())
            {
                lst.Add(DHThanhPhanDO.FromDataReader(reader));
            }

            return lst;
        }
        #endregion

        #region Đồng bộ server
        private async Task Sync_CreateTablesAsync(MySqlCommand command)
        {
            // pm_nhanvien: username, pw, ten, mobile, role_flags, login
            string query = @"CREATE TABLE IF NOT EXISTS sync_log (
                id INT AUTO_INCREMENT PRIMARY KEY,
                tblindex INT, row_id INT, sync_at TIMESTAMP,
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP);";
            command.CommandText = query;
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<int>> Sync_GetIdsByTable(MySqlConnection conn, string tblname, int tblindex)
        {
            List<int> ids = [];
            if (tblindex > 0)
            {
                string query = $"SELECT id FROM {tblname} WHERE updated_at > COALESCE((SELECT sync_at FROM sync_log WHERE tblindex = {tblindex}), '2000-01-01 00:00:00');";
                using MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }

                await reader.CloseAsync();
            }
            return ids;
        }

        public async Task<List<int>> Sync_UpdateSyncTime(MySqlConnection conn, string tblname)
        {
            int i = tblname switch
            {
                "pm_strings" => 1,
                _ => -1
            };

            List<int> ids = [];
            if (i > 0)
            {
                string query = $"SELECT id FROM {tblname} WHERE updated_at > COALESCE((SELECT sync_at FROM sync_log WHERE tblindex = {i}), '2000-01-01 00:00:00');";
                using MySqlCommand cmd = new MySqlCommand(query, conn);

                var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }
            return ids;
        }
        public async Task Sync_UpdateTimeAsync(MySqlConnection conn, int tblindex)
        {
            // Tạo command để gọi stored procedure
            using (var command = new MySqlCommand("sync_save_time", conn))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                // Thêm các tham số đầu vào
                command.Parameters.AddWithValue("p_index", tblindex);

                // Thực thi stored procedure
                await command.ExecuteNonQueryAsync();
            }
        }
        #endregion

        #region Utils
        public async Task<decimal> GetDbSizeAsync()
        {
            using var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            string query = @"SELECT SUM(data_length + index_length) AS `size` 
                             FROM information_schema.tables
                             WHERE table_schema = DATABASE();";
            using var command = new MySqlCommand(query, connection);
            var ret = command.ExecuteScalar();

            return ret == null? 0: (decimal)ret;
        }

        public async Task<MySqlConnection> OpenConnAsync()
        {
            var connection = new MySqlConnection(ConnStr);
            await connection.OpenAsync();
            return connection;
        }

        public async Task CloseConnAsync(MySqlConnection conn)
        {
            await conn.CloseAsync();
        }
        #endregion
    }
}
