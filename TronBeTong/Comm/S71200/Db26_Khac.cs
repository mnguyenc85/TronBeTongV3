using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_Khac: PlcDb
    {
        public PlcTag TGTron { get; private set; } = new PlcTag(TagTypes.Int16, 430);
        public PlcTag TGXa { get; private set; } = new PlcTag(TagTypes.Int16, 432);
        public PlcTag TGXaNua { get; private set; } = new PlcTag(TagTypes.Int16, 434);
        public PlcTag CoiTronMeHt { get; private set; } = new PlcTag(TagTypes.Int16, 436);
        /// <summary>
        /// Time discharge delay et skip
        /// </summary>
        public PlcTag TGXaCLDT2 { get; private set; } = new PlcTag(TagTypes.Int16, 438);
        public PlcTag QuaTaiXeSkip { get; private set; } = new PlcTag(TagTypes.Real, 440);
        public PlcTag XM1VitTinh { get; private set; } = new PlcTag(TagTypes.Bool, 444, 0);
        public PlcTag WaterKeep { get; private set; } = new PlcTag(TagTypes.Real, 446);
        public PlcTag DieuKienSoMeTruocKhiXaVaoCoi { get; private set; } = new PlcTag(TagTypes.Bool, 450, 0);

        public Db26_Khac() : base(26, 22, 430)
        {
            Cycle = -1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            TGTron.ParseDb(_buf, StartByteAddr);
            TGXaNua.ParseDb(_buf, StartByteAddr);
            TGXa.ParseDb(_buf, StartByteAddr);
            CoiTronMeHt.ParseDb(_buf, StartByteAddr);
            TGXaCLDT2.ParseDb(_buf, StartByteAddr);
            QuaTaiXeSkip.ParseDb(_buf, StartByteAddr);
            XM1VitTinh.ParseDb(_buf, StartByteAddr);
            WaterKeep.ParseDb(_buf, StartByteAddr);
            DieuKienSoMeTruocKhiXaVaoCoi.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
