using MySqlConnector;

namespace TronBeTongV3.Data.DO.DonHang
{
    public class DHThanhPhanDO
    {
        public int Id { get; set; }
        public string? NL_Ma { get; set; }
        public string? NL_Ten { get; set; }
        /// <summary>
        /// Xem Core.Enums.LoaiThanhPhan
        /// </summary>
        public int NL_PhanLoai { get; set; }
        public int NL_Silo { get; set; }

        public double KLCongThuc { get; set; }
        /// <summary>
        /// Không sử dụng
        /// </summary>
        public double KLTong { get; set; }
        /// <summary>
        /// Không sử dụng
        /// </summary>
        public double KLMe { get; set; }

        public int RoundDigit { get; set; } = 0;

        public const string SelectFields = "id,ma,ten,phanloai,silo,klcongthuc,kltong,klme";
        public static DHThanhPhanDO FromDataReader(MySqlDataReader rd)
        {
            var tp = new DHThanhPhanDO()
            {
                Id = rd.GetInt32(0),
                NL_Ma = rd.IsDBNull(1) ? null : rd.GetString(1),
                NL_Ten = rd.IsDBNull(2) ? null : rd.GetString(2),
                NL_PhanLoai = rd.IsDBNull(3)? 0: rd.GetInt32(3),
                NL_Silo = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                KLCongThuc = rd.IsDBNull(5) ? 0 : rd.GetDouble(5),
                KLTong = rd.IsDBNull(6) ? 0 : rd.GetDouble(6),
                KLMe = rd.IsDBNull(7) ? 0 : rd.GetDouble(7),
            };
            tp.RoundDigit = tp.NL_PhanLoai == 3 ? 1 : 0;
            return tp;
        }
    }
}
