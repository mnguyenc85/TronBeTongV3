using NMComm.S71200;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_Calib: PlcDb
    {
        #region Calibrations
        public PlcTag CalibCL1AI { get; private set; } = new PlcTag(TagTypes.Int16, 306);
        public PlcTag CalibCL1Zero { get; private set; } = new PlcTag(TagTypes.Int16, 308);
        public PlcTag CalibCL1Span { get; private set; } = new PlcTag(TagTypes.Real, 310);
        public PlcTag CalibCL2AI { get; private set; } = new PlcTag(TagTypes.Int16, 314);
        public PlcTag CalibCL2Zero { get; private set; } = new PlcTag(TagTypes.Int16, 316);
        public PlcTag CalibCL2Span { get; private set; } = new PlcTag(TagTypes.Real, 318);
        public PlcTag CalibCL3AI { get; private set; } = new PlcTag(TagTypes.Int16, 322);
        public PlcTag CalibCL3Zero { get; private set; } = new PlcTag(TagTypes.Int16, 324);
        public PlcTag CalibCL3Span { get; private set; } = new PlcTag(TagTypes.Real, 326);
        public PlcTag CalibCL4AI { get; private set; } = new PlcTag(TagTypes.Int16, 330);
        public PlcTag CalibCL4Zero { get; private set; } = new PlcTag(TagTypes.Int16, 332);
        public PlcTag CalibCL4Span { get; private set; } = new PlcTag(TagTypes.Real, 334);
        public PlcTag CalibCL5AI { get; private set; } = new PlcTag(TagTypes.Int16, 338);
        public PlcTag CalibCL5Zero { get; private set; } = new PlcTag(TagTypes.Int16, 340);
        public PlcTag CalibCL5Span { get; private set; } = new PlcTag(TagTypes.Real, 342);

        public PlcTag CalibXM1AI { get; private set; } = new PlcTag(TagTypes.Int16, 346);
        public PlcTag CalibXM1Zero { get; private set; } = new PlcTag(TagTypes.Int16, 348);
        public PlcTag CalibXM1Span { get; private set; } = new PlcTag(TagTypes.Real, 350);
        public PlcTag CalibXM2AI { get; private set; } = new PlcTag(TagTypes.Int16, 354);
        public PlcTag CalibXM2Zero { get; private set; } = new PlcTag(TagTypes.Int16, 356);
        public PlcTag CalibXM2Span { get; private set; } = new PlcTag(TagTypes.Real, 358);


        public PlcTag CalibNuocAI { get; private set; } = new PlcTag(TagTypes.Int16, 362);
        public PlcTag CalibNuocZero { get; private set; } = new PlcTag(TagTypes.Int16, 364);
        public PlcTag CalibNuocSpan { get; private set; } = new PlcTag(TagTypes.Real, 366);

        public PlcTag CalibPGAI { get; private set; } = new PlcTag(TagTypes.Int16, 370);
        public PlcTag CalibPGZero { get; private set; } = new PlcTag(TagTypes.Int16, 372);
        public PlcTag CalibPGSpan { get; private set; } = new PlcTag(TagTypes.Real, 374);
        #endregion

        public Db26_Calib() : base(26, 72, 306)
        {
            Cycle = 0.5;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

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
