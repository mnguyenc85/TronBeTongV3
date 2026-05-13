using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class SiloNguyenLieuDO
    {
        private const string _tbl = "ct_nguyenlieu";

        public int Id { get; set; }
        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public int PhanLoai { get; set; }
        public double DoAm { get; set; }

        /// <summary>
        /// Kích thước hạt cốt liệu
        /// </summary>
        public double KichThuocHat { get; set; }
        
        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $@"UPDATE {_tbl} SET 
                ma=@ma,ten=@ten,phanloai=@pl,doam=@doam
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@pl", PhanLoai);
            command.Parameters.AddWithValue("@doam", DoAm);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                ma,ten,phanloai,doam) 
                VALUES (@ma,@ten,@pl,@doam);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@pl", PhanLoai);
            command.Parameters.AddWithValue("@doam", DoAm);
            return command;
        }

        public const string SelectFields = "id,ma,ten,phanloai,doam";
        public static SiloNguyenLieuDO FromDataReader(MySqlDataReader rd)
        {
            return new SiloNguyenLieuDO()
            {
                Id          = rd.GetInt32(0),
                Ma          = rd.IsDBNull(1) ? null : rd.GetString(1),
                Ten         = rd.IsDBNull(2) ? null : rd.GetString(2),
                PhanLoai    = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                DoAm        = rd.IsDBNull(4) ? 0 : rd.GetDouble(4),
            };
        }
    }
}
