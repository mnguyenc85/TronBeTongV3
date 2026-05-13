using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class DKThongKeDO
    {
        public string? MaDon { get; set; }
        public string? KhachHang { get; set; }
        public string? CongTrinh { get; set; }
        public string? SoPhieu { get; set; }
        public string? BienSoXe { get; set; }
        public string? LaiXe { get; set; }
        public string? TGBD { get; set; }
        public string? TGKT { get; set; }
        public bool MeTuDong { get; set; }
        public bool MeDungTay { get; set; }
        public bool MeMoPhong { get; set; }
    }
}
