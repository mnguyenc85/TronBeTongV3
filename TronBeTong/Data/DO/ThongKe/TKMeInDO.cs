using System;
using System.Xml.Serialization;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class TKMeInDO
    {
        public int DonId { get; set; }
        public int PhieuId { get; set; }
        public int MeId { get; set; }

        public int STT { get; set; }

        public string? DonMa { get; set; }
        public string? DonKhachHang { get; set; }
        public string? DonDuAn { get; set; }
        public string? PhieuMa { get; set; }
        public string? PhieuBSX { get; set; }
        public string? PhieuLaiXe { get; set; }
        public string? PhieuCapPhoi { get; set; }
        public string? TGHT { get; set; }

        public string? M3Tron { get; set; }
        public string? SoMe { get; set; }

        public string? TP0 { get; set; }
        public string? TP1 { get; set; }
        public string? TP2 { get; set; }
        public string? TP3 { get; set; }
        public string? TP4 { get; set; }
        public string? TP5 { get; set; }
        public string? TP6 { get; set; }
        public string? TP7 { get; set; }
        public string? TP8 { get; set; }
        public string? TP9 { get; set; }
        public string? TP10 { get; set; }
        public string? TP11 { get; set; }
        public string? TP12 { get; set; }
        public string? TP13 { get; set; }
        public string? TP14 { get; set; }
        public string? TP15 { get; set; }

        public string? TrangThai { get; set; }
        public int Flags { get; set; }

        public string? GetTP(int i) => i switch
        {
            0 => TP0,
            1 => TP1,
            2 => TP2,
            3 => TP3,
            4 => TP4,
            5 => TP5,
            6 => TP6,
            7 => TP7,                
            8 => TP8,
            9 => TP9,                
            10 => TP10,
            11 => TP11,
            12 => TP12,
            13 => TP13,
            14 => TP14,
            15 => TP15,
            _ => null
        };

        public void SetTP(int i, string? v)
        {
            switch (i)
            {
                case 0: TP0 = v; break;
                case 1: TP1 = v; break;
                case 2: TP2 = v; break;
                case 3: TP3 = v; break;
                case 4: TP4 = v; break;
                case 5: TP5 = v; break;
                case 6: TP6 = v; break;
                case 7: TP7 = v; break;
                case 8: TP8 = v; break;
                case 9: TP9 = v; break;
                case 10: TP10 = v; break;
                case 11: TP11 = v; break;
                case 12: TP12 = v; break;
                case 13: TP13 = v; break;
                case 14: TP14 = v; break;
                case 15: TP15 = v; break;
            }
        }

        public void ClearTPs()
        {
            TP0 = null;
            TP1 = null;
            TP2 = null;
            TP3 = null;
            TP4 = null;
            TP5 = null;
            TP6 = null;
            TP7 = null;
            TP8 = null;
            TP9 = null;
            TP10 = null;
            TP11 = null;
            TP12 = null;
            TP13 = null;
            TP14 = null;
            TP15 = null;
        }

        public void Clone(TKMeInDO me)
        {
            DonId = me.DonId;
            PhieuId = me.PhieuId;
            MeId = me.MeId;

            STT = me.STT;

            DonMa = me.DonMa;
            DonKhachHang = me.DonKhachHang;
            DonDuAn = me.DonDuAn;
            PhieuMa = me.PhieuMa;
            PhieuBSX = me.PhieuBSX;
            PhieuLaiXe = me.PhieuLaiXe;
            PhieuCapPhoi = me.PhieuCapPhoi;
            TGHT = me.TGHT;
            M3Tron = me.M3Tron;
            SoMe = me.SoMe;
            for (int i = 0; i < 16; i++)
            {
                SetTP(i, me.GetTP(i));
            }
            TrangThai = me.TrangThai;
            Flags = me.Flags;
        }

        public TKMeInDO Clone()
        {
            var t = new TKMeInDO();
            t.Clone(this);
            return t;
        }

        public void From(TKFullMeDO_Obsolete me)
        {
            DonId = me.DonId;
            PhieuId = me.PhieuId;
            MeId = me.MeId;

            STT = 0;

            DonMa = me.DonMa;
            DonKhachHang = me.DonKhachHang;
            DonDuAn = me.DonDuAn;
            PhieuMa = me.PhieuMa;
            PhieuBSX = me.PhieuBSX;
            PhieuLaiXe = me.PhieuLaiXe;
            PhieuCapPhoi = me.PhieuCapPhoi;
            TGHT = me.TGHT;
            M3Tron = me.M3Tron;
            SoMe = me.SoMe;
            for (int i = 0; i < me.TPs.Length; i++)
            {
                SetTP(i, me.TPs[i]);
            }
            TrangThai = me.TrangThai;
            Flags = me.Flags;
        }
    }
}
