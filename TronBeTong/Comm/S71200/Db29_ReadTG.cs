using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_ReadTG : PlcDb
    {
        public PlcTag TGLenPheuCLTG { get; private set; } = new PlcTag(TagTypes.Int16, 202);
        public PlcTag TGTreMoXaCLTG { get; private set; } = new PlcTag(TagTypes.Int16, 204);

        public Db26_ReadTG() : base(29, 4, 202) { }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            TGLenPheuCLTG.ParseDb(_buf, StartByteAddr);
            TGTreMoXaCLTG.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
