using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class KDLaiXeDO
    {
        private const string _tbl = "kd_laixe";

        public int Id { get; set; }

        public string? Ten { get; set; }
        public string? Sdt { get; set; }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $"UPDATE {_tbl} SET ten=@ten,sdt=@sdt WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@sdt", Sdt);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(ten,sdt)  VALUES (@ten,@sdt);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@sdt", Sdt);
            return command;
        }

        public const string SelectFields = "id,ten,sdt";
        public static KDLaiXeDO FromDataReader(MySqlDataReader rd)
        {
            return new KDLaiXeDO()
            {
                Id = rd.GetInt32(0),
                Ten = rd.IsDBNull(1) ? null : rd.GetString(1),
                Sdt = rd.IsDBNull(2) ? null : rd.GetString(2),
            };
        }
    }
}
