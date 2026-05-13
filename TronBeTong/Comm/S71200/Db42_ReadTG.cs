using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    [Obsolete("Không có thùng cốt liệu trung gian")]
    public class Db42_ReadTG : PlcDb
    {
        public PlcTag TGLenPheuCLTG { get; private set; } = new PlcTag(TagTypes.Int16, 12);
        public PlcTag TGTreMoXaCLTG { get; private set; } = new PlcTag(TagTypes.Int16, 14);

        public Db42_ReadTG() : base(42, 4, 12) { }

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
