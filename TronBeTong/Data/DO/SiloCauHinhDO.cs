using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class SiloCauHinhDO
    {
        private const string _tbl = "ct_silo";

        public int Id { get; set; }
        public int PhanLoai { get; set; }
        public int Index { get; set; }
        public int NLId { get; set; }
        public int TSId { get; set; }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $@"UPDATE {_tbl} SET 
                phanloai=@pl,stt=@stt,nl_id=@nlid,ts_id=@tsid
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@pl", PhanLoai);
            command.Parameters.AddWithValue("@stt", Index);
            command.Parameters.AddWithValue("@nlid", NLId);
            command.Parameters.AddWithValue("@tsid", TSId);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(phanloai,stt,nl_id,ts_id) 
                VALUES (@pl,@stt,@nlid,@tsid);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@pl", PhanLoai);
            command.Parameters.AddWithValue("@stt", Index);
            command.Parameters.AddWithValue("@nlid", NLId);
            command.Parameters.AddWithValue("@tsid", TSId);
            return command;
        }

        public const string SelectFields = "id,phanloai,stt,nl_id,ts_id";
        public static SiloCauHinhDO FromDataReader(MySqlDataReader rd)
        {
            return new SiloCauHinhDO()
            {
                Id          = rd.GetInt32(0),
                PhanLoai    = rd.IsDBNull(1) ? 0 : rd.GetInt32(1),
                Index       = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                NLId        = rd.IsDBNull(3) ? -1 : rd.GetInt32(3),
                TSId        = rd.IsDBNull(4) ? -1 : rd.GetInt32(4),
            };
        }
    }
}
