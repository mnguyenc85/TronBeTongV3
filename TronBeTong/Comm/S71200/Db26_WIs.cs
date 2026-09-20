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

        #region Calibrations
        public PlcTag CalibCL1AI { get; private set; } = new PlcTag(TagTypes.Int16, 306);
        public PlcTag CalibCL1Zero { get; private set; } = new PlcTag(TagTypes.Int16, 308);
        public PlcTag CalibCL1Span { get; private set; } = new PlcTag(TagTypes.Int16, 310);
        public PlcTag CalibCL2AI { get; private set; } = new PlcTag(TagTypes.Int16, 314);
        public PlcTag CalibCL2Zero { get; private set; } = new PlcTag(TagTypes.Int16, 316);
        public PlcTag CalibCL2Span { get; private set; } = new PlcTag(TagTypes.Int16, 318);
        public PlcTag CalibCL3AI { get; private set; } = new PlcTag(TagTypes.Int16, 322);
        public PlcTag CalibCL3Zero { get; private set; } = new PlcTag(TagTypes.Int16, 324);
        public PlcTag CalibCL3Span { get; private set; } = new PlcTag(TagTypes.Int16, 326);
        public PlcTag CalibCL4AI { get; private set; } = new PlcTag(TagTypes.Int16, 330);
        public PlcTag CalibCL4Zero { get; private set; } = new PlcTag(TagTypes.Int16, 332);
        public PlcTag CalibCL4Span { get; private set; } = new PlcTag(TagTypes.Int16, 334);
        public PlcTag CalibCL5AI { get; private set; } = new PlcTag(TagTypes.Int16, 338);
        public PlcTag CalibCL5Zero { get; private set; } = new PlcTag(TagTypes.Int16, 340);
        public PlcTag CalibCL5Span { get; private set; } = new PlcTag(TagTypes.Int16, 342);

        public PlcTag CalibXM1AI { get; private set; } = new PlcTag(TagTypes.Int16, 346);
        public PlcTag CalibXM1Zero { get; private set; } = new PlcTag(TagTypes.Int16, 348);
        public PlcTag CalibXM1Span { get; private set; } = new PlcTag(TagTypes.Int16, 350);
        public PlcTag CalibXM2AI { get; private set; } = new PlcTag(TagTypes.Int16, 354);
        public PlcTag CalibXM2Zero { get; private set; } = new PlcTag(TagTypes.Int16, 356);
        public PlcTag CalibXM2Span { get; private set; } = new PlcTag(TagTypes.Int16, 358);


        public PlcTag CalibNuocAI { get; private set; } = new PlcTag(TagTypes.Int16, 362);
        public PlcTag CalibNuocZero { get; private set; } = new PlcTag(TagTypes.Int16, 364);
        public PlcTag CalibNuocSpan { get; private set; } = new PlcTag(TagTypes.Int16, 366);

        public PlcTag CalibPGAI { get; private set; } = new PlcTag(TagTypes.Int16, 370);
        public PlcTag CalibPGZero { get; private set; } = new PlcTag(TagTypes.Int16, 372);
        public PlcTag CalibPGSpan { get; private set; } = new PlcTag(TagTypes.Int16, 374);
        #endregion

        public Db26_WIs() : base(26, 134, 244)
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

            #region Hiệu chuẩn
            CalibCL1AI.ParseDb(_buf, StartByteAddr);
            CalibCL1Zero.ParseDb(_buf, StartByteAddr);
            CalibCL1Span.ParseDb(_buf, StartByteAddr);

            CalibXM1AI.ParseDb(_buf, StartByteAddr);
            CalibXM1Zero.ParseDb(_buf, StartByteAddr);
            CalibXM1Span.ParseDb(_buf, StartByteAddr);

            CalibNuocAI.ParseDb(_buf, StartByteAddr);
            CalibNuocZero.ParseDb(_buf, StartByteAddr);
            CalibNuocSpan.ParseDb(_buf, StartByteAddr);

            CalibPGAI.ParseDb(_buf, StartByteAddr);
            CalibPGZero.ParseDb(_buf, StartByteAddr);
            CalibPGSpan.ParseDb(_buf, StartByteAddr);
            #endregion

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
