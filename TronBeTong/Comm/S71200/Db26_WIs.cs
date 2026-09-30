using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db26_WIs : PlcDb
    {
        public PlcTag[] CL_TT { get; private set; } = new PlcTag[5];
        public PlcTag[] CL_KL { get; private set; } = new PlcTag[5];
        public PlcTag[] CL_Me { get; private set; } = new PlcTag[5];

        public PlcTag[] XM_TT { get; private set; } = new PlcTag[2];
        public PlcTag[] XM_KL { get; private set; } = new PlcTag[2];
        public PlcTag[] XM_Me { get; private set; } = new PlcTag[2];

        public PlcTag[] Nuoc_TT { get; private set; } = new PlcTag[2];
        public PlcTag[] Nuoc_KL { get; private set; } = new PlcTag[2];
        public PlcTag[] Nuoc_Me { get; private set; } = new PlcTag[2];

        public PlcTag[] PG_TT { get; private set; } = new PlcTag[2];
        public PlcTag[] PG_KL { get; private set; } = new PlcTag[2];
        public PlcTag[] PG_Me { get; private set; } = new PlcTag[2];

        public Db26_WIs() : base(26, 430 - 342, 342)
        {
            for (int i = 0; i < 5; i++)
            {
                CL_TT[i] = new PlcTag(TagTypes.Int16, 342 + i * 2);
                CL_KL[i] = new PlcTag(TagTypes.Real, 352 + i * 4);
                CL_Me[i] = new PlcTag(TagTypes.Int16, 372 + i * 2);
            }

            for (int i = 0; i < 2; i++)
            {
                XM_TT[i] = new PlcTag(TagTypes.Int16, 382 + i * 2);
                XM_KL[i] = new PlcTag(TagTypes.Int16, 386 + i * 4);
                XM_Me[i] = new PlcTag(TagTypes.Int16, 394 + i * 2);

                Nuoc_TT[i] = new PlcTag(TagTypes.Int16, 398 + i * 2);
                Nuoc_KL[i] = new PlcTag(TagTypes.Int16, 402 + i * 4);
                Nuoc_Me[i] = new PlcTag(TagTypes.Int16, 410 + i * 2);

                PG_TT[i] = new PlcTag(TagTypes.Int16, 414 + i * 2);
                PG_KL[i] = new PlcTag(TagTypes.Int16, 418 + i * 4);
                PG_Me[i] = new PlcTag(TagTypes.Int16, 426 + i * 2);
            }
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            for (int i = 0; i < 5; i++)
            {
                CL_TT[i].ParseDb(_buf, StartByteAddr);
                CL_KL[i].ParseDb(_buf, StartByteAddr);
                CL_Me[i].ParseDb(_buf, StartByteAddr);
            }

            for (int i = 0; i < 2; i++)
            {
                XM_TT[i].ParseDb(_buf, StartByteAddr);
                XM_KL[i].ParseDb(_buf, StartByteAddr);
                XM_Me[i].ParseDb(_buf, StartByteAddr);

                Nuoc_TT[i].ParseDb(_buf, StartByteAddr);
                Nuoc_KL[i].ParseDb(_buf, StartByteAddr);
                Nuoc_Me[i].ParseDb(_buf, StartByteAddr);

                PG_TT[i].ParseDb(_buf, StartByteAddr);
                PG_KL[i].ParseDb(_buf, StartByteAddr);
                PG_Me[i].ParseDb(_buf, StartByteAddr);
            }

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }
}
