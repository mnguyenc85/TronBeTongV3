using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_WIs2: PlcDb
    {
        public PlcTag CL2_TT { get; private set; } = new PlcTag(TagTypes.Int16, 378);
        public PlcTag CL2_KL { get; private set; } = new PlcTag(TagTypes.Real, 380);
        public PlcTag CL2_Me { get; private set; } = new PlcTag(TagTypes.Int16, 384);
        public PlcTag CL3_TT { get; private set; } = new PlcTag(TagTypes.Int16, 386);
        public PlcTag CL3_KL { get; private set; } = new PlcTag(TagTypes.Real, 388);
        public PlcTag CL3_Me { get; private set; } = new PlcTag(TagTypes.Int16, 392);

        public Db26_WIs2() : base(26, 394 - 378, 378)
        {
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            CL2_TT.ParseDb(_buf, StartByteAddr);
            CL2_KL.ParseDb(_buf, StartByteAddr);
            CL2_Me.ParseDb(_buf, StartByteAddr);

            CL3_TT.ParseDb(_buf, StartByteAddr);
            CL3_KL.ParseDb(_buf, StartByteAddr);
            CL3_Me.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
