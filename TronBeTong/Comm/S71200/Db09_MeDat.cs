using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db09_MeDat : PlcDb
    {
        public PlcTag M3Dat { get; private set; } = new PlcTag(TagTypes.Real, 0);
        public PlcTag SoMeDat { get; private set; } = new PlcTag(TagTypes.Int16, 4);

        public Db09_MeDat(): base(9, 6, 0) { }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            M3Dat.ParseDb(_buf);
            SoMeDat.ParseDb(_buf);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
