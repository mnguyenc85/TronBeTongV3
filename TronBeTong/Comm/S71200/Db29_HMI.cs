using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    /// <summary>
    /// Đọc 1 số tag của db29 để hiển thị HMI
    /// </summary>
    public class Db29_HMI: PlcDb
    {
        public PlcTag TGCotLieuLenCLTG_ET { get; private set; } = new PlcTag(TagTypes.Int16, 178);
        public PlcTag TGMoXaCLTG_ET { get; private set; } = new PlcTag(TagTypes.Int16, 180);
        public PlcTag TGTronBeTong_ET { get; private set; } = new PlcTag(TagTypes.Int16, 182);
        public PlcTag TGTreMoCoiTronHalf_ET { get; private set; } = new PlcTag(TagTypes.Int16, 184);
        public PlcTag TGTreMoCoiTron_ET { get; private set; } = new PlcTag(TagTypes.Int16, 186);
        public PlcTag TGTreXaXeSkip_ET { get; private set; } = new PlcTag(TagTypes.Int16, 188);
        
        public PlcTag TGTronBeTong { get; private set; } = new PlcTag(TagTypes.Int16, 194);
        public PlcTag TGTreMoCoiTronHalf { get; private set; } = new PlcTag(TagTypes.Int16, 196);
        public PlcTag TGTreMoCoiTron { get; private set; } = new PlcTag(TagTypes.Int16, 198);

        public Db29_HMI() : base(29, 22, 178)
        {
            Cycle = 0.4;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            TGCotLieuLenCLTG_ET.ParseDb(_buf, StartByteAddr);
            TGMoXaCLTG_ET.ParseDb(_buf, StartByteAddr);
            TGTronBeTong_ET.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTronHalf_ET.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTron_ET.ParseDb(_buf, StartByteAddr);
            TGTreXaXeSkip_ET.ParseDb(_buf, StartByteAddr);

            TGTronBeTong.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTronHalf.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTron.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
