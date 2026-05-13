using MySqlConnector;

namespace TronBeTongV3.Data.DO.DonHang
{
    public class DHPhieuDO
    {
        public int Id { get; set; }
        public int DonHangId { get; set; }

        public string? SoPhieu { get; set; }
        public int XeId { get; set; }
        public int LaiXeId { get; set; }
        public int CongThucId { get; set; }

        public int MeDat { get; set; }
        public int MeHT { get; set; }
        public double TheTichDat { get; set; }
        public double TheTichHT { get; set; }
        public double KLDat { get; set; }
        public double KLHT { get; set; }

        public DateTime? TGBD { get; set; }
        public DateTime? TGHT { get; set; }

        public double DonGia { get; set; }
        
        /// <summary>
        /// Số phiếu theo đơn
        /// </summary>
        public int DonStt { get; set; }
        /// <summary>
        /// Thể tích tích lũy theo đơn
        /// </summary>
        public double DonM3 { get; set; }
        public int DonSttSim { get; set; }
        public double DonM3Sim { get; set; }

        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }

        public string? KepChi { get; set; }

        public MySqlCommand CreateInsert(MySqlConnection conn, string tbl)
        {
            string query = $@"INSERT INTO {tbl}(
                    sophieu,donhang_id,xe_id,lx_id,congthuc_id,
                    thetichdat,kldat,medat,                    
                    thetichht,klht,meht,tgbd,tght,
                    dongia,trangthai,ghichu,kepchi) 
                VALUES (@sp,@dhid,@xeid,@lxid,@ctid,
                    @ttdat,@kldat,@medat,
                    @ttht, @klht, @meht, @tgbd, @tght,
                    @dg,@trangthai,@ghichu,@kepchi);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@sp", SoPhieu);

            command.Parameters.AddWithValue("@dhid", DonHangId);
            command.Parameters.AddWithValue("@xeid", XeId);
            command.Parameters.AddWithValue("@lxid", LaiXeId);

            command.Parameters.AddWithValue("@ctid", CongThucId);

            command.Parameters.AddWithValue("@ttdat", TheTichDat);
            command.Parameters.AddWithValue("@kldat", KLDat);
            command.Parameters.AddWithValue("@medat", MeDat);

            command.Parameters.AddWithValue("@ttht", TheTichHT);
            command.Parameters.AddWithValue("@klht", KLHT);
            command.Parameters.AddWithValue("@meht", MeHT);
            command.Parameters.AddWithValue("@tgbd", TGBD);
            command.Parameters.AddWithValue("@tght", null);

            command.Parameters.AddWithValue("@dg", DonGia);
            command.Parameters.AddWithValue("@trangthai", TrangThai);
            command.Parameters.AddWithValue("@ghichu", null);

            command.Parameters.AddWithValue("@kepchi", KepChi);
            return command;
        }

        public const string SelectedFields = "id,sophieu,xe_id,lx_id,congthuc_id,thetichdat,kldat,medat,thetichht,klht,meht,tgbd,tght,dongia,don_stt,don_tt,don_stt_sim,don_tt_sim,trangthai,ghichu,kepchi,donhang_id";
        public static DHPhieuDO FromDataReader(MySqlDataReader rd)
        {
            return new DHPhieuDO()
            {
                Id = rd.GetInt32(0),
                SoPhieu = rd.IsDBNull(1) ? null : rd.GetString(1),
                XeId        = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                LaiXeId     = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                CongThucId  = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),

                TheTichDat  = rd.IsDBNull(5) ? 0 : rd.GetDouble(5),
                KLDat       = rd.IsDBNull(6) ? 0 : rd.GetDouble(6),
                MeDat       = rd.IsDBNull(7) ? 0 : rd.GetInt32(7),

                TheTichHT   = rd.IsDBNull(8) ? 0 : rd.GetDouble(8),
                KLHT        = rd.IsDBNull(9) ? 0 : rd.GetDouble(9),
                MeHT        = rd.IsDBNull(10) ? 0 : rd.GetInt32(10),

                TGBD        = rd.IsDBNull(11) ? null : rd.GetDateTime(11),
                TGHT        = rd.IsDBNull(12) ? null: rd.GetDateTime(12),

                DonGia      = rd.IsDBNull(13) ? 0: rd.GetDouble(13),

                DonStt      = rd.IsDBNull(14) ? 0: rd.GetInt32(14),
                DonM3       = rd.IsDBNull(15) ? 0: rd.GetDouble(15),
                DonSttSim   = rd.IsDBNull(16) ? 0: rd.GetInt32(16),
                DonM3Sim    = rd.IsDBNull(17) ? 0 : rd.GetDouble(17),

                TrangThai   = rd.IsDBNull(18) ? 0 : rd.GetInt32(18),
                GhiChu      = rd.IsDBNull(19) ? null : rd.GetString(19),

                KepChi      = rd.IsDBNull(20) ? null : rd.GetString(20),

                DonHangId   = rd.IsDBNull(21) ? 0: rd.GetInt32(21),
            };
        }
    }
}
