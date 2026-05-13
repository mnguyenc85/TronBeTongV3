using MySqlConnector;
using System.Text;
using TronBeTongV3.Comm;

namespace TronBeTongV3.Data.DO.DonHang
{
    public class DHMeDO
    {
        public int Id { get; set; }
        public int PhieuId { get; set; }
        public int STT { get; set; }
        public double M3Tron { get; set; }

        public double[] KLCL { get; set; } = new double[6];
        public double[] HuCL { get; set; } = new double[6];
        public double[] KLXi { get; set; } = new double[5];
        public double[] KLPG { get; set; } = new double[8];
        public double KLNuoc { get; set; }

        /// <summary>
        /// 1: Lưu khi phát hiện cân đủ; 2: lưu khi ấn "Dừng trộn"; 4: Simulation; 8: Cân PG ngoài
        /// </summary>
        public int Flags { get; set; }

        public DateTime UpdateAt { get; set; }
        public DateTime? CreateAt { get; set; }

        public bool[] ChotCotLieus { get; set; } = new bool[ModelHeThong.SoCanCLMax];
        public bool[] ChotXMs { get; set; } = new bool[ModelHeThong.SoCanXMMax];
        public bool[] ChotPGs { get; set; } = new bool[ModelHeThong.SoCanPGMax];
        /// <summary>
        /// Đã chốt nước
        /// </summary>
        public bool ChotNuoc { get; set; } = false;
        /// <summary>
        /// Đã chốt bất kỳ 1 thành phần cốt liệu
        /// </summary>
        public bool ChotCotLieu { get
            {
                foreach (var c in ChotCotLieus) if (c) return true;
                return false;
            } 
        }
        /// <summary>
        /// Đã chốt bất kỳ 1 thành phần xi
        /// </summary>
        public bool ChotXi
        {
            get
            {
                foreach (var c in ChotXMs) if (c) return true;
                return false;
            }
        }

        public double KLDaLuuPhieu { get; set; } = 0;

        public MySqlCommand CreateInsert(MySqlConnection conn, string tbl)
        {
            string query = $@"INSERT INTO {tbl}(
                    phieu_id,stt,m3tron,
                    cl1,cl2,cl3,cl4,cl5,cl6,
                    xi1,xi2,xi3,xi4,xi5,
                    pg1,pg2,pg3,pg4,pg5,pg6,pg7,pg8,
                    hu1,hu2,hu3,hu4,hu5,hu6,
                    nuoc,flags) 
                VALUES (@pid,@stt,@m3tron,
                    @cl1,@cl2,@cl3,@cl4,@cl5,@cl6,
                    @xi1,@xi2,@xi3,@xi4,@xi5,
                    @pg1,@pg2,@pg3,@pg4,@pg5,@pg6,@pg7,@pg8,
                    @hu1,@hu2,@hu3,@hu4,@hu5,@hu6,
                    @nuoc,@flags);
                SELECT LAST_INSERT_ID();";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@pid", PhieuId);
            command.Parameters.AddWithValue("@stt", STT);
            command.Parameters.AddWithValue("@m3tron", M3Tron);

            command.Parameters.AddWithValue("@cl1", KLCL[0]);
            command.Parameters.AddWithValue("@cl2", KLCL[1]);
            command.Parameters.AddWithValue("@cl3", KLCL[2]);
            command.Parameters.AddWithValue("@cl4", KLCL[3]);
            command.Parameters.AddWithValue("@cl5", KLCL[4]);
            command.Parameters.AddWithValue("@cl6", KLCL[5]);

            command.Parameters.AddWithValue("@xi1", KLXi[0]);
            command.Parameters.AddWithValue("@xi2", KLXi[1]);
            command.Parameters.AddWithValue("@xi3", KLXi[2]);
            command.Parameters.AddWithValue("@xi4", KLXi[3]);
            command.Parameters.AddWithValue("@xi5", KLXi[4]);

            command.Parameters.AddWithValue("@pg1", KLPG[0]);
            command.Parameters.AddWithValue("@pg2", KLPG[1]);
            command.Parameters.AddWithValue("@pg3", KLPG[2]);
            command.Parameters.AddWithValue("@pg4", KLPG[3]);
            command.Parameters.AddWithValue("@pg5", KLPG[4]);
            command.Parameters.AddWithValue("@pg6", KLPG[5]);
            command.Parameters.AddWithValue("@pg7", KLPG[6]);
            command.Parameters.AddWithValue("@pg8", KLPG[7]);

            command.Parameters.AddWithValue("@hu1", HuCL[0]);
            command.Parameters.AddWithValue("@hu2", HuCL[1]);
            command.Parameters.AddWithValue("@hu3", HuCL[2]);
            command.Parameters.AddWithValue("@hu4", HuCL[3]);
            command.Parameters.AddWithValue("@hu5", HuCL[4]);
            command.Parameters.AddWithValue("@hu6", HuCL[5]);

            command.Parameters.AddWithValue("@nuoc", KLNuoc);

            command.Parameters.AddWithValue("@flags", Flags);

            return command;
        }

        public MySqlCommand CreateUpdate(MySqlConnection conn, string tbl)
        {
            string query = $@"UPDATE {tbl} SET 
                    cl1=@cl1,cl2=@cl2,cl3=@cl3,cl4=@cl4,cl5=@cl5,cl6=@cl6,
                    xi1=@xi1,xi2=@xi2,xi3=@xi3,xi4=@xi4,xi5=@xi5,
                    pg1=@pg1,pg2=@pg2,pg3=@pg3,pg4=@pg4,pg5=@pg5,pg6=@pg6,pg7=@pg7,pg8=@pg8,
                    nuoc=@nuoc WHERE id=@id;";
            var command = new MySqlCommand(query, conn);
            command.Parameters.AddWithValue("@cl1", KLCL[0]);
            command.Parameters.AddWithValue("@cl2", KLCL[1]);
            command.Parameters.AddWithValue("@cl3", KLCL[2]);
            command.Parameters.AddWithValue("@cl4", KLCL[3]);
            command.Parameters.AddWithValue("@cl5", KLCL[4]);
            command.Parameters.AddWithValue("@cl6", KLCL[5]);

            command.Parameters.AddWithValue("@xi1", KLXi[0]);
            command.Parameters.AddWithValue("@xi2", KLXi[1]);
            command.Parameters.AddWithValue("@xi3", KLXi[2]);
            command.Parameters.AddWithValue("@xi4", KLXi[3]);
            command.Parameters.AddWithValue("@xi5", KLXi[4]);

            command.Parameters.AddWithValue("@pg1", KLPG[0]);
            command.Parameters.AddWithValue("@pg2", KLPG[1]);
            command.Parameters.AddWithValue("@pg3", KLPG[2]);
            command.Parameters.AddWithValue("@pg4", KLPG[3]);
            command.Parameters.AddWithValue("@pg5", KLPG[4]);
            command.Parameters.AddWithValue("@pg6", KLPG[5]);
            command.Parameters.AddWithValue("@pg7", KLPG[6]);
            command.Parameters.AddWithValue("@pg8", KLPG[7]);

            command.Parameters.AddWithValue("@nuoc", KLNuoc);
            command.Parameters.AddWithValue("@id", Id);

            return command;
        }

        // 0, 5, 11, 16, 24, 30
        public static string SelectedFields =
            @"id,phieu_id,stt,m3tron,nuoc,
            cl1,cl2,cl3,cl4,cl5,cl6,
            xi1,xi2,xi3,xi4,xi5,
            pg1,pg2,pg3,pg4,pg5,pg6,pg7,pg8,
            hu1,hu2,hu3,hu4,hu5,hu6,
            flags,created_at";

        public static DHMeDO FromDataReader(MySqlDataReader rd)
        {
            var o = new DHMeDO()
            {
                Id = rd.GetInt32(0),
                PhieuId = rd.IsDBNull(1) ? 0 : rd.GetInt32(1),
                STT = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                M3Tron = rd.IsDBNull(3) ? 0 : rd.GetDouble(3),
                KLNuoc = rd.IsDBNull(4) ? 0 : rd.GetDouble(4),
                Flags = rd.IsDBNull(30) ? 0 : rd.GetInt32(30),
                CreateAt = rd.IsDBNull(31) ? null: rd.GetDateTime(31)
            };

            for (int i = 0; i < o.KLCL.Length; i++)
                o.KLCL[i] = rd.IsDBNull(5 + i) ? 0 : rd.GetDouble(5 + i);
            for (int i = 0; i < o.KLXi.Length; i++)
                o.KLXi[i] = rd.IsDBNull(11 + i) ? 0 : rd.GetDouble(11 + i);
            for (int i = 0; i < o.KLPG.Length; i++)
                o.KLPG[i] = rd.IsDBNull(16 + i) ? 0 : rd.GetDouble(16 + i);
            for (int i = 0; i < o.HuCL.Length; i++)
                o.HuCL[i] = rd.IsDBNull(24 + i) ? 0 : rd.GetDouble(24 + i);

            return o;
        }

        public double TongKL()
        {
            double t = KLNuoc;
            for (int i = 0; i < KLCL.Length; i++)
                t += KLCL[i];
            for (int i = 0; i < KLXi.Length; i++)
                t += KLXi[i];
            for (int i = 0; i < KLPG.Length; i++)
                t += KLPG[i];
            return t;
        }

        public bool CheckEmpty(double eps)
        {
            if (KLNuoc > eps) return false;
            for (int i = 0; i < KLCL.Length; i++)
                if (KLCL[i] > eps) return false;
            for (int i = 0; i < KLXi.Length; i++)
                if (KLXi[i] > eps) return false;
            for (int i = 0; i < KLPG.Length; i++)
                if (KLPG[i] > eps) return false;
            return true;
        }

        public string GetDesc(int sotpcl, int sotpxi, int sotppg)
        {
            StringBuilder sb = new();
            sb.Append($"STT: {STT} -> {M3Tron:F3}m3");
            sb.Append("\r\nCL: ");
            for (int i = 0; i < sotpcl; i++)
                sb.Append($"{KLCL[i]}; ");
            sb.Append("\r\nXi: ");
            for (int i = 0; i < sotpxi; i++)
                sb.Append($"{KLXi[i]}; ");
            sb.Append("\r\nPG: ");
            for (int i = 0; i < sotppg; i++)
                sb.Append($"{KLPG[i]}; ");
            sb.Append($"\r\nNuoc: {KLNuoc}");

            return sb.ToString();
        }

        public string GetChotInfo()
        {
            StringBuilder sb = new();

            sb.Append($"mẻ Id={Id}: CL: ");
            for (int i = 0; i < ChotCotLieus.Length; i++)
                if (ChotCotLieus[i]) sb.Append(i);
            sb.Append($" Xi: ");
            for (int i = 0; i < ChotXMs.Length; i++)
                if (ChotXMs[i]) sb.Append(i);
            if (ChotNuoc) sb.Append(" N");
            sb.Append($" PG: ");
            for (int i = 0; i < ChotPGs.Length; i++)
                if (ChotPGs[i]) sb.Append(i);

            return sb.ToString();
        }
    }
}
