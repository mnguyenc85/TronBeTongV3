using TronBeTongV3.Data.DO.DonHang;

namespace TronBeTongV3.Data.DO.ThongKe
{
    public class TKCongThucDO: BTCongThucDO
    {
        public List<DHThanhPhanDO> DsThanhPhan { get; private set; } = [];
        public TKCongThucDO() { }
        public TKCongThucDO(BTCongThucDO o)
        {
            FromBTCongThucDO(o);
        }
        public void FromBTCongThucDO(BTCongThucDO o)
        {
            Id = o.Id;
            Ma = o.Ma;
            Mac = o.Mac;
            Slump = o.Slump;
            KLNuoc = o.KLNuoc;
            WCRatio = o.WCRatio;
            SoTP = o.SoTP;
        }
    }
}
