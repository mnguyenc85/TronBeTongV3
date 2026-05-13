using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class KDDuAnDO
    {
        private const string _tbl = "kd_duan";

        public int Id { get; set; }

        public int DuAnId { get; set; }
        public int CongTrinhId { get; set; }
        public int HangMucId { get; set; }
        public int DiaChiId { get; set; }
        
        public string? DuAn { get; set; }
        public string? CongTrinh { get; set; }
        public string? HangMuc { get; set; }
        public string? DiaChi { get; set; }

        public string? GhiChu { get; set; }

        public int KHId { get; set; }


        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $@"UPDATE {_tbl} SET 
                duan=@duan,congtrinh=@ct,hangmuc=@hm,diachi=@dc,ghichu=@gc,kh_id=@khid
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@duan", DuAnId);
            command.Parameters.AddWithValue("@ct", CongTrinhId);
            command.Parameters.AddWithValue("@hm", HangMucId);
            command.Parameters.AddWithValue("@dc", DiaChiId);
            command.Parameters.AddWithValue("@gc", GhiChu);
            command.Parameters.AddWithValue("@khid", KHId);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                duan,congtrinh,hangmuc,diachi,ghichu,kh_id) 
                VALUES (@duan,@ct,@hm,@dc,@gc,@khid);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@duan", DuAnId);
            command.Parameters.AddWithValue("@ct", CongTrinhId);
            command.Parameters.AddWithValue("@hm", HangMucId);
            command.Parameters.AddWithValue("@dc", DiaChiId);
            command.Parameters.AddWithValue("@gc", GhiChu);
            command.Parameters.AddWithValue("@khid", KHId);
            return command;
        }

        public const string SelectFields = "id,duan,congtrinh,hangmuc,diachi,ghichu,kh_id";
        public static KDDuAnDO FromDataReader(MySqlDataReader rd)
        {
            return new KDDuAnDO()
            {
                Id =            rd.GetInt32(0),
                DuAnId =          rd.IsDBNull(1) ? -1 : rd.GetInt32(1),
                CongTrinhId =     rd.IsDBNull(2) ? -1 : rd.GetInt32(2),
                HangMucId =       rd.IsDBNull(3) ? -1 : rd.GetInt32(3),
                DiaChiId =        rd.IsDBNull(4) ? -1 : rd.GetInt32(4),
                GhiChu =        rd.IsDBNull(5) ? null : rd.GetString(5),
                KHId =          rd.IsDBNull(6) ? -1 : rd.GetInt32(6),
            };
        }
    }
}
