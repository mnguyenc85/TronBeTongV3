using MySqlConnector;

namespace TronBeTongV3.Data.DO
{
    public class KDKhachHangDO
    {
        private const string _tbl = "kd_khachhang";

        public int Id { get; set; }

        public string? Ma { get; set; }
        public string? Ten { get; set; }
        public string? DiaChi { get; set; }
        public string? Sdt { get; set; }
        public string? Email { get; set; }
        public string? MaSoThue { get; set; }
        public string? LienHe { get; set; }
        public string? GhiChu { get; set; }

        public DateTime UpdatedAt { get; set; }

        public MySqlCommand CreateUpdate(MySqlConnection conn)
        {
            string query = $@"UPDATE {_tbl} SET 
                ten=@ten,ma=@ma,diachi=@diachi,sdt=@sdt,email=@email,mst=@mst,lienhe=@lienhe,ghichu=@ghichu
                WHERE id={Id}";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@diachi", DiaChi);
            command.Parameters.AddWithValue("@sdt", Sdt);
            command.Parameters.AddWithValue("@email", Email);
            command.Parameters.AddWithValue("@mst", MaSoThue);
            command.Parameters.AddWithValue("@lienhe", LienHe);
            command.Parameters.AddWithValue("@ghichu", GhiChu);
            return command;
        }

        public MySqlCommand CreateInsert(MySqlConnection conn)
        {
            string query = $@"INSERT INTO {_tbl}(
                ten,ma,diachi,sdt,email,mst,lienhe,ghichu) 
                VALUES (@ten,@ma,@diachi,@sdt,@email,@mst,@lienhe,@ghichu);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ten", Ten);
            command.Parameters.AddWithValue("@ma", Ma);
            command.Parameters.AddWithValue("@diachi", DiaChi);
            command.Parameters.AddWithValue("@sdt", Sdt);
            command.Parameters.AddWithValue("@email", Email);
            command.Parameters.AddWithValue("@mst", MaSoThue);
            command.Parameters.AddWithValue("@lienhe", LienHe);
            command.Parameters.AddWithValue("@ghichu", GhiChu);
            return command;
        }

        public const string SelectFields = "id,ten,ma,diachi,sdt,email,mst,lienhe,ghichu,updated_at";
        public static KDKhachHangDO FromDataReader(MySqlDataReader rd)
        {
            return new KDKhachHangDO()
            {
                Id =        rd.GetInt32(0),
                Ten =       rd.IsDBNull(1) ? null : rd.GetString(1),
                Ma =        rd.IsDBNull(2) ? null : rd.GetString(2),
                DiaChi =    rd.IsDBNull(3) ? null : rd.GetString(3),
                Sdt =       rd.IsDBNull(4) ? null : rd.GetString(4),
                Email =     rd.IsDBNull(5) ? null : rd.GetString(5),
                MaSoThue =  rd.IsDBNull(6) ? null : rd.GetString(6),
                LienHe =    rd.IsDBNull(7) ? null : rd.GetString(7),
                GhiChu =    rd.IsDBNull(8) ? null : rd.GetString(8),
                UpdatedAt = rd.IsDBNull(9) ? DateTime.MinValue: rd.GetDateTime(9),
            };
        }
    }
}
