using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db29_SetMixerTime: PlcDb
    {
        public PlcTag TGTron { get; private set; } = new PlcTag(TagTypes.Int16, 76);
        public PlcTag TGXaNua { get; private set; } = new PlcTag(TagTypes.Int16, 78);
        public PlcTag TGXa { get; private set; } = new PlcTag(TagTypes.Int16, 80);

        public Db29_SetMixerTime() : base(29, 6, 76) { }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            TGTron.ParseDb(_buf, StartByteAddr);
            TGXaNua.ParseDb(_buf, StartByteAddr);
            TGXa.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
