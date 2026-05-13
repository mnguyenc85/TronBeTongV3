using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class BTCongThucDO
    {
        public int Id { get; set; }
        public string? Ma { get; set; }
        public string? Mac { get; set; }
        public string? Slump { get; set; }
        public double KLNuoc { get; set; }
        public double WCRatio { get; set; }

        public double KTHat { get; set; }

        public int SoTP { get; set; }

        public MySqlCommand CreateUpdate(MySqlConnection conn, string tbl)
        {
            string query = $@"UPDATE {tbl} SET 
                ma=@ma,mac=@mac,slump=@slump,klnuoc=@klnuoc,wcratio=@wc,kthat=@kthat,sotp=@sotp
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@mac", Mac);
            command.Parameters.AddWithValue("@slump", Slump);
            command.Parameters.AddWithValue("@klnuoc", KLNuoc);
            command.Parameters.AddWithValue("@wc", WCRatio);
            command.Parameters.AddWithValue("@kthat", KTHat);
            command.Parameters.AddWithValue("@sotp", SoTP);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn, string tbl)
        {
            string query = $@"INSERT INTO {tbl}(
                ma,mac,slump,klnuoc,wcratio,kthat,sotp) 
                VALUES (@ma,@mac,@slump,@klnuoc,@wc,@kthat,@sotp);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@mac", Mac);
            command.Parameters.AddWithValue("@slump", Slump);
            command.Parameters.AddWithValue("@klnuoc", KLNuoc);
            command.Parameters.AddWithValue("@wc", WCRatio);
            command.Parameters.AddWithValue("@kthat", KTHat);
            command.Parameters.AddWithValue("@sotp", SoTP);
            return command;
        }

        public const string SelectFields = "id,ma,mac,slump,klnuoc,wcratio,kthat,sotp";
        public static BTCongThucDO FromDataReader(MySqlDataReader rd, int i0 = 0)
        {
            return new BTCongThucDO()
            {
                Id      = rd.GetInt32(0 + i0),
                Ma      = rd.IsDBNull(1 + i0) ? null : rd.GetString(1 + i0),
                Mac     = rd.IsDBNull(2 + i0) ? null : rd.GetString(2 + i0),
                Slump   = rd.IsDBNull(3 + i0) ? null : rd.GetString(3 + i0),
                KLNuoc  = rd.IsDBNull(4 + i0) ? 0 : rd.GetDouble(4 + i0),
                WCRatio = rd.IsDBNull(5 + i0) ? 0 : rd.GetDouble(5 + i0),
                KTHat   = rd.IsDBNull(6 + i0) ? 0 : rd.GetDouble(6 + i0),
                SoTP    = rd.IsDBNull(7 + i0) ? 0 : rd.GetInt32(7 + i0),
            };
        }
    }
}
