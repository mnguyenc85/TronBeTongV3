
namespace TronBeTongV3.Comm
{
    public class WeightIndicatorState
    {
        private string[] TTCanCL { get; set; }
        private string[] TTCanXM { get; set; }
        private string[] TTCanPG { get; set; }
        private string[] TTCanNuoc { get; set; }

        /// <summary>
        /// Giá trị trạng thái để chốt cốt liệu
        /// </summary>
        public int ChotCoLieu { get; set; } = -1;
        /// <summary>
        /// Giá trị trạng thái để chốt xi măng
        /// </summary>
        public int ChotXM { get; set; } = -1;
        /// <summary>
        /// Giá trị trạng thái để chốt phụ gia
        /// </summary>
        public int ChotPG { get; set; } = -1;
        /// <summary>
        /// Giá trị trạng thái để chốt nước
        /// </summary>
        public int ChotNuoc { get; set; } = -1;

        public int DayCotLieu { get; set; } = -1;
        public int DayXM { get; set; } = -1;
        public int DayPG { get; set; } = -1;
        public int DayNuoc { get; set; } = -1;

        public WeightIndicatorState()
        {
            TTCanCL = ["Error"];
            TTCanXM = ["Error"];
            TTCanPG = ["Error"];
            TTCanNuoc = ["Error"];
        }

        public WeightIndicatorState(string[] ttcl, string[] ttxm, string[] ttpg, string[] ttw)
        {
            TTCanCL = ttcl;
            TTCanXM = ttxm;
            TTCanPG = ttpg;
            TTCanNuoc = ttw;
        }

        public string GetStrTTCan(int i, int pl)
        {
            switch (pl)
            {
                case 0: return GetTTCanXMStr(i);
                case 1: return GetTTCanCLStr(i);
                case 2: return GetTTCanNuocStr(i);
                case 3: return GetTTCanPGStr(i);
                default: return "Error";
            }
        }

        public string GetTTCanCLStr(int i)
        {
            if (i >= 0 && i < TTCanCL.Length) return TTCanCL[i];
            return "Error";
        }
        public string GetTTCanXMStr(int i)
        {
            if (i >= 0 && i < TTCanXM.Length) return TTCanXM[i];
            return "Error";
        }
        public string GetTTCanPGStr(int i)
        {
            if (i >= 0 && i < TTCanPG.Length) return TTCanPG[i];
            return "Error";
        }
        public string GetTTCanNuocStr(int i)
        {
            if (i >= 0 && i < TTCanNuoc.Length) return TTCanNuoc[i];
            return "Error";
        }

        /// <summary>
        /// Cho trạm trộn Thanh Trì 2025-11-07. S7-1200
        /// </summary>
        /// <returns></returns>
        public static WeightIndicatorState GetWIS_ThanhTri()
        {
            string[] ttcl =
            [
                "Cân rỗng",
                "Cân thô",
                "Cân tinh",
                "Chờ",
                "Cân đủ",
                "Cân đầy"
            ];
            
            string[] ttxm =
            [
                "Cân rỗng",
                "Cân thô xi 1",
                "Cân tinh xi 1",
                "Chờ xi 1",
                "Cân thô xi 2",
                "Cân tinh xi 2",
                "Chờ xi 2",
                "Cân đủ",
                "Cân đầy",
            ];

            string[] ttw =
            [
                "Cân rỗng",
                "Cân thô",
                "Cân tinh",
                "Chờ",
                "Cân đủ",
                "Cân đầy",
            ];

            string[] ttpg = [
                "Cân rỗng",
                "Cân thô pg 1",
                "Cân tinh pg 1",
                "Chờ pg 1",
                "Cân thô pg 2",
                "Cân tinh pg 2",
                "Chờ pg 2",
                "Cân đủ",
                "Cân đầy",
            ];


            var wis = new WeightIndicatorState(ttcl, ttxm, ttpg, ttw);
            wis.ChotCoLieu = 4;
            wis.ChotXM = 7;
            wis.ChotPG = 7;
            wis.ChotNuoc = 4;

            wis.DayCotLieu = 5;
            wis.DayXM = 8;
            wis.DayPG = 8;
            wis.DayNuoc = 5;

            return wis;
        }

        /// <summary>
        /// Cho trạm 60m3: 3 CL, 2 Xi, 1 Pg
        /// </summary>
        /// <returns></returns>
        public static WeightIndicatorState GetWIS_60m3()
        {
            string[] ttcl =
            [
                "Cân rỗng",
                "Cân thô 1",
                "Chờ 1",
                "Cân thô 2",
                "Chờ 2",
                "Cân thô 3",
                "Chờ 3",
                "Cân thô 4",
                "Chờ 4",
                "Cân đủ",
                "Cân đầy"
            ];

            string[] ttxm =
            [
                "Cân rỗng",
                "Cân thô xi 1",
                "Cân tinh xi 1",
                "Chờ xi 1",
                "Cân thô xi 2",
                "Cân tinh xi 2",
                "Chờ xi 2",
                "Cân đủ",
                "Cân đầy",
            ];

            string[] ttw =
            [
                "Cân rỗng",
                "Cân thô",
                "Cân tinh",
                "Chờ",
                "Cân đủ",
                "Cân đầy",
            ];

            string[] ttpg = [
                "Cân rỗng",
                "Cân thô pg 1",
                "Cân tinh pg 1",
                "Chờ pg 1",
                "Cân thô pg 2",
                "Cân tinh pg 2",
                "Chờ pg 2",
                "Cân đủ",
                "Cân đầy",
            ];


            var wis = new WeightIndicatorState(ttcl, ttxm, ttpg, ttw);
            wis.ChotCoLieu = 9;
            wis.ChotXM = 7;
            wis.ChotPG = 7;
            wis.ChotNuoc = 4;

            wis.DayCotLieu = 10;
            wis.DayXM = 8;
            wis.DayPG = 8;
            wis.DayNuoc = 5;

            return wis;
        }


        /// <summary>
        /// Cho trạm trộn Hoàng Hải 2025/09/26. S7-200
        /// </summary>
        /// <returns></returns>
        public static WeightIndicatorState GetWIS_HoangHai()
        {
            string[] ttcl =
            [
                "Empty",
                "Full",
                "PE",                       // Chốt
                "Fine1",
                "Coars1",
                "Pause1",
                "Fine2",
                "Coars2",
                "Pause2",
                "Fine3",
                "Coars3",
                "Pause3",
                "Fine4",
                "Coars4",
                "Pause4"
            ];

            string[] ttxm =
            [
                "Empty",
                "Full",
                "PE",                       // Chốt
                "Fine1",
                "Coars1",
                "Pause1",
                "Fine2",
                "Coars2",
                "Pause2",
            ];

            string[] ttw =
            [
                "Empty",
                "Full",
                "PE",                       // Chốt
                "Water",
                "Pause"
            ];

            string[] ttpg = ["Error"];

            var wis = new WeightIndicatorState(ttcl, ttxm, ttpg, ttw);
            wis.ChotCoLieu = 2;
            wis.ChotXM = 2;
            wis.ChotNuoc = 2;
            wis.ChotPG = -1;

            return wis;
        }
    }
}
