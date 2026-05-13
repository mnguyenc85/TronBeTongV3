using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class KDXeDO
    {
        private const string _tbl = "kd_xe";

        public int Id { get; set; }
        public string? BSX { get; set; }
        public double DungTich { get; set; }
        public double DungTichMin { get; set; }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $@"UPDATE {_tbl} SET 
                bsx=@bsx,dungtich=@dt,dungtichmin=@dtm
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@bsx", BSX);
            command.Parameters.AddWithValue("@dt", DungTich);
            command.Parameters.AddWithValue("@dtm", DungTichMin);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                bsx,dungtich,dungtichmin) 
                VALUES (@bsx,@dt,@dtm);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@bsx", BSX);
            command.Parameters.AddWithValue("@dt", DungTich);
            command.Parameters.AddWithValue("@dtm", DungTichMin);
            return command;
        }

        public const string SelectFields = "id,bsx,dungtich,dungtichmin";
        public static KDXeDO FromDataReader(MySqlDataReader rd)
        {
            return new KDXeDO()
            {
                Id          = rd.GetInt32(0),
                BSX         = rd.IsDBNull(1) ? null : rd.GetString(1),
                DungTich    = rd.IsDBNull(2) ? 0 : rd.GetDouble(2),
                DungTichMin = rd.IsDBNull(3) ? 0 : rd.GetDouble(3),
            };
        }
    }
}
