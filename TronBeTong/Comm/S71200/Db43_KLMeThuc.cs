using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db43_KLMeThuc: PlcDb {
        public PlcTag CL1 { get; private set; } = new PlcTag(TagTypes.Real, 240);
        public PlcTag CL2 { get; private set; } = new PlcTag(TagTypes.Real, 244);
        public PlcTag CL3 { get; private set; } = new PlcTag(TagTypes.Real, 248);
        public PlcTag CL4 { get; private set; } = new PlcTag(TagTypes.Real, 252);
        public PlcTag CL5 { get; private set; } = new PlcTag(TagTypes.Real, 256);
        public PlcTag CL6 { get; private set; } = new PlcTag(TagTypes.Real, 260);

        public PlcTag XM1 { get; private set; } = new PlcTag(TagTypes.Real, 264);
        public PlcTag XM2 { get; private set; } = new PlcTag(TagTypes.Real, 268);
        public PlcTag XM3 { get; private set; } = new PlcTag(TagTypes.Real, 272);
        public PlcTag XM4 { get; private set; } = new PlcTag(TagTypes.Real, 276);

        public PlcTag Nuoc { get; private set; } = new PlcTag(TagTypes.Real, 280);

        public PlcTag PG1 { get; private set; } = new PlcTag(TagTypes.Real, 284);
        public PlcTag PG2 { get; private set; } = new PlcTag(TagTypes.Real, 288);

        public Db43_KLMeThuc() : base(43, 52, 240)
        {
            Cycle = 0.3;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            CL1.ParseDb(_buf, StartByteAddr);
            CL2.ParseDb(_buf, StartByteAddr);
            CL3.ParseDb(_buf, StartByteAddr);
            //CL4.ParseDb(_buf);
            //CL5.ParseDb(_buf);
            //CL6.ParseDb(_buf);

            XM1.ParseDb(_buf, StartByteAddr);
            XM2.ParseDb(_buf, StartByteAddr);
            XM3.ParseDb(_buf, StartByteAddr);
            XM4.ParseDb(_buf, StartByteAddr);

            Nuoc.ParseDb(_buf, StartByteAddr);

            PG1.ParseDb(_buf, StartByteAddr);
            PG2.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }

    }
}