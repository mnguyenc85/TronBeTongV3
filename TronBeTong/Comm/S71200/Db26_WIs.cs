using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_WIs : PlcDb
    {
        public PlcTag CL_TT { get; private set; } = new PlcTag(TagTypes.Int16, 244);
        public PlcTag CL_KL { get; private set; } = new PlcTag(TagTypes.Real, 246);
        public PlcTag CL_Me { get; private set; } = new PlcTag(TagTypes.Int16, 250);

        public PlcTag XM_TT { get; private set; } = new PlcTag(TagTypes.Int16, 252);
        public PlcTag XM_KL { get; private set; } = new PlcTag(TagTypes.Real, 254);
        public PlcTag XM_Me { get; private set; } = new PlcTag(TagTypes.Int16, 258);

        public PlcTag Nuoc_TT { get; private set; } = new PlcTag(TagTypes.Int16, 268);
        public PlcTag Nuoc_KL { get; private set; } = new PlcTag(TagTypes.Real, 270);
        public PlcTag Nuoc_Me { get; private set; } = new PlcTag(TagTypes.Int16, 274);

        public PlcTag PG_TT { get; private set; } = new PlcTag(TagTypes.Int16, 276);
        public PlcTag PG_KL { get; private set; } = new PlcTag(TagTypes.Real, 278);
        public PlcTag PG_Me { get; private set; } = new PlcTag(TagTypes.Int16, 282);

        public PlcTag TGTron { get; private set; } = new PlcTag(TagTypes.Int16, 284);
        public PlcTag TGXa { get; private set; } = new PlcTag(TagTypes.Int16, 286);
        public PlcTag TGXaNua { get; private set; } = new PlcTag(TagTypes.Int16, 288);

        public PlcTag CoiTronMeHt { get; private set; } = new PlcTag(TagTypes.Int16, 290);

        public PlcTag TGXaCLDT2 { get; private set; } = new PlcTag(TagTypes.Int16, 292);

        public Db26_WIs() : base(26, 50, 244)
        { 

        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            CL_TT.ParseDb(_buf, StartByteAddr);
            CL_KL.ParseDb(_buf, StartByteAddr);
            CL_Me.ParseDb(_buf, StartByteAddr);

            XM_TT.ParseDb(_buf, StartByteAddr);
            XM_KL.ParseDb(_buf, StartByteAddr);
            XM_Me.ParseDb(_buf, StartByteAddr);

            Nuoc_TT.ParseDb(_buf, StartByteAddr);
            Nuoc_KL.ParseDb(_buf, StartByteAddr);
            Nuoc_Me.ParseDb(_buf, StartByteAddr);

            PG_TT.ParseDb(_buf, StartByteAddr);
            PG_KL.ParseDb(_buf, StartByteAddr);
            PG_Me.ParseDb(_buf, StartByteAddr);

            TGTron.ParseDb(_buf, StartByteAddr);
            TGXaNua.ParseDb(_buf, StartByteAddr);
            TGXa.ParseDb(_buf, StartByteAddr);
            CoiTronMeHt.ParseDb(_buf, StartByteAddr);

            TGXaCLDT2.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
