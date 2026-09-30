using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_Khac: PlcDb
    {
        public PlcTag QuaTaiXeSkip { get; private set; } = new PlcTag(TagTypes.Real, 294);
        public PlcTag XM1VitTinh { get; private set; } = new PlcTag(TagTypes.Bool, 298, 5);
        public PlcTag WaterKeep { get; private set; } = new PlcTag(TagTypes.Real, 300);

        public Db26_Khac() : base(26, 304, 0)
        {
            Cycle = -1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            QuaTaiXeSkip.ParseDb(_buf, StartByteAddr);

            XM1VitTinh.ParseDb(_buf, StartByteAddr);

            WaterKeep.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
