using MySqlConnector;
using System;

namespace TronBeTongV3.Data.DO.DonHang
{
    public class DHDonDO
    {
        public int Id { get; set; }

        public string? Ma { get; set; }

        public int KhachHangId { get; set; }
        
        public int DuAnId { get; set; }
        public int CongTrinhId { get; set; }
        public int HangMucId { get; set; }
        public int DiaChiId { get; set; }
        public string? DuAn { get; set; }
        public string? CongTrinh { get; set; }
        public string? HangMuc { get; set; }
        public string? DiaChi { get; set; }

        public double TheTichDH { get; set; }
        public double TheTichHT { get; set; }
        public double KLHT { get; set; }
        public int MeHT { get; set; }
        public DateTime? TGHT { get; set; }

        public int TrangThai { get; set; }
        public string? GhiChu { get; set; }

        public DateTime CreatedAt { get; set; }
        public int TongSoPhieu { get; set; }

        public MySqlCommand CreateInsert(MySqlConnection conn, string tbl)
        {
            string query = $@"INSERT INTO {tbl}(
                    ma,kh_id,da_id,ct_id,hm_id,diachi_id,
                    thetichdh,thetichht,klht,meht,tght,
                    trangthai,ghichu) 
                VALUES (@ma,@khid,@daid,@ctid,@hmid,@dcid,
                    @ttdh,@thetichht,@klht,@meht,@tght,
                    @trangthai,@ghichu);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@ma", Ma);

            command.Parameters.AddWithValue("@khid", KhachHangId);
            command.Parameters.AddWithValue("@daid", DuAnId);
            command.Parameters.AddWithValue("@ctid", CongTrinhId);
            command.Parameters.AddWithValue("@hmid", HangMucId);
            command.Parameters.AddWithValue("@dcid", DiaChiId);
            command.Parameters.AddWithValue("@ttdh", TheTichDH);

            command.Parameters.AddWithValue("@thetichht", 0);
            command.Parameters.AddWithValue("@klht", 0);
            command.Parameters.AddWithValue("@meht", 0);
            command.Parameters.AddWithValue("@tght", null);
            command.Parameters.AddWithValue("@trangthai", 0);
            command.Parameters.AddWithValue("@ghichu", null);
            return command;
        }

        // 0 2 8 12
        public static string SelectedFields =
            //@"id,ma,
            //kh_id,da_id,ct_id,hm_id,diachi_id,
            //thetichht,klht,meht,tght,trangthai,ghichu,created_at FROM ht_donhang";
            @"a.id, a.ma,
            a.kh_id, a.da_id, a.ct_id, a.hm_id, a.diachi_id,a.thetichdh,
            a.thetichht, a.klht, a.meht, a.tght,
            a.trangthai, a.ghichu, a.created_at,
            COUNT(b.id) AS sophieu, SUM(b.medat) AS tmedat, SUM(b.thetichdat) AS tttdat, SUM(b.kldat) AS tkldat,
            SUM(b.meht) AS tmeht, SUM(b.thetichht) AS tttht, SUM(b.klht) AS tklht
            FROM ht_donhang AS a LEFT JOIN ht_phieu AS b ON a.id=b.donhang_id";

        public static DHDonDO FromDataReader(MySqlDataReader rd)
        {
            return new DHDonDO()
            {
                Id          = rd.GetInt32(0),
                Ma          = rd.IsDBNull(1) ? null : rd.GetString(1),
                
                KhachHangId = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                DuAnId      = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                CongTrinhId = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                HangMucId   = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                DiaChiId    = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),

                TheTichDH   = rd.IsDBNull(7) ? 0 : rd.GetDouble(7),

                //TheTichHT   = rd.IsDBNull(8) ? 0 : rd.GetDouble(8),
                //KLHT        = rd.IsDBNull(9) ? 0 : rd.GetDouble(9),
                //MeHT        = rd.IsDBNull(10) ? 0 : rd.GetInt32(10),
                //TGHT        = rd.IsDBNull(11) ? null : rd.GetDateTime(11),

                TrangThai   = rd.IsDBNull(12) ? 0 : rd.GetInt32(12),
                GhiChu      = rd.IsDBNull(13) ? null : rd.GetString(13),
                CreatedAt   = rd.IsDBNull(14)? DateTime.Now : rd.GetDateTime(14),
                
                TongSoPhieu = rd.IsDBNull(15)? 0: rd.GetInt32(15),
                MeHT        = rd.IsDBNull(19)? 0: rd.GetInt32(19),
                TheTichHT   = rd.IsDBNull(20)? 0: rd.GetDouble(20),
                KLHT        = rd.IsDBNull(21)? 0: rd.GetDouble(21)
            };
        }

        public static string TKFields = "id, ma, kh_id, da_id, ct_id, hm_id, diachi_id, thetichdh, trangthai, ghichu, created_at";
        public static DHDonDO FromTKDataReader(MySqlDataReader rd)
        {
            return new DHDonDO()
            {
                Id = rd.GetInt32(0),
                Ma = rd.IsDBNull(1) ? null : rd.GetString(1),

                KhachHangId = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                DuAnId = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                CongTrinhId = rd.IsDBNull(4) ? 0 : rd.GetInt32(4),
                HangMucId = rd.IsDBNull(5) ? 0 : rd.GetInt32(5),
                DiaChiId = rd.IsDBNull(6) ? 0 : rd.GetInt32(6),

                TheTichDH = rd.IsDBNull(7) ? 0 : rd.GetDouble(7),

                //TheTichHT   = rd.IsDBNull(8) ? 0 : rd.GetDouble(8),
                //KLHT        = rd.IsDBNull(9) ? 0 : rd.GetDouble(9),
                //MeHT        = rd.IsDBNull(10) ? 0 : rd.GetInt32(10),
                //TGHT        = rd.IsDBNull(11) ? null : rd.GetDateTime(11),

                TrangThai = rd.IsDBNull(8) ? 0 : rd.GetInt32(8),
                GhiChu = rd.IsDBNull(9) ? null : rd.GetString(9),
                CreatedAt = rd.IsDBNull(10) ? DateTime.Now : rd.GetDateTime(10),
            };
        }
    }
}
