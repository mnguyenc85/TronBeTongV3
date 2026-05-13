using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db43_RTVAR: PlcDb
    {
        /// <summary>
        /// Chỉ có 3 tpcl nhưng vẫn để 4
        /// </summary>
        public const int SoTPCL = 4;
        /// <summary>
        /// Chỉ có 2 tpxm nhưng vẫn để 4
        /// </summary>
        public const int SoTPXM = 4;
        public const int SoTPPG = 2;

        #region Cốt liệu
        /// <summary>
        /// Real
        /// </summary>
        public PlcTag[] CLs { get; private set; } = new PlcTag[SoTPCL];
        /// <summary>
        /// Addition
        /// </summary>
        public PlcTag[] CLAdds { get; private set; } = new PlcTag[SoTPCL];
        /// <summary>
        /// Humidity
        /// </summary>
        public PlcTag[] CLHus { get; private set; } = new PlcTag[SoTPCL];
        /// <summary>
        /// Close
        /// </summary>
        public PlcTag[] CLChots { get; private set; } = new PlcTag[SoTPCL];
        #endregion

        #region Xi măng
        public PlcTag[] XMs { get; private set; } = new PlcTag[SoTPXM];
        public PlcTag[] XMAdds { get; private set; } = new PlcTag[SoTPXM];
        public PlcTag[] XMChots { get; private set; } = new PlcTag[SoTPXM];
        #endregion

        #region Phụ gia
        public PlcTag[] PGs { get; private set; } = new PlcTag[SoTPPG];
        public PlcTag[] PGAdds { get; private set; } = new PlcTag[SoTPPG];
        public PlcTag[] PGChots { get; private set; } = new PlcTag[SoTPPG];
        #endregion

        #region Nước
        public PlcTag Water { get; private set; } 
        public PlcTag WaterAdd { get; private set; }
        public PlcTag WaterClose { get; private set; }
        #endregion

        public PlcTag M3Me { get; private set; }

        public Db43_RTVAR(): base(43, 186, 0)
        {
            for (int i = 0; i < CLs.Length; i++)
            {
                CLs[i] = new PlcTag(TagTypes.Real, i * 4);
                CLAdds[i] = new PlcTag(TagTypes.Real, 66 + i * 8);
                CLHus[i] = new PlcTag(TagTypes.Real, 166 + i * 4);
                CLChots[i] = new PlcTag(TagTypes.Real, 62 + i * 8);
            }

            for (int i = 0; i < XMs.Length; i++)
            {
                XMs[i] = new PlcTag(TagTypes.Real, 24 + i * 4);
                XMAdds[i] = new PlcTag(TagTypes.Real, 114 + i * 8);
                XMChots[i] = new PlcTag(TagTypes.Real, 110 + i * 8);
            }

            for (int i = 0; i < PGs.Length; i++)
            {
                PGs[i] = new PlcTag(TagTypes.Real, 44 + i * 4);
                PGAdds[i] = new PlcTag(TagTypes.Real, 154 + i * 8);
                PGChots[i] = new PlcTag(TagTypes.Real, 150 + i * 8);
            }

            Water = new PlcTag(TagTypes.Real, 40);
            WaterAdd = new PlcTag(TagTypes.Real, 146);
            WaterClose = new PlcTag(TagTypes.Real, 142);

            M3Me = new PlcTag(TagTypes.Real, 58);

            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            for (int i = 0; i < CLs.Length; i++)
            {
                CLs[i].ParseDb(_buf, StartByteAddr);
                CLAdds[i].ParseDb(_buf, StartByteAddr);
                CLHus[i].ParseDb(_buf, StartByteAddr);
                CLChots[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < XMs.Length; i++)
            {
                XMs[i].ParseDb(_buf, StartByteAddr);
                XMAdds[i].ParseDb(_buf, StartByteAddr);
                XMChots[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < PGs.Length; i++)
            {
                PGs[i].ParseDb(_buf, StartByteAddr);
                PGAdds[i].ParseDb(_buf, StartByteAddr);
                PGChots[i].ParseDb( _buf, StartByteAddr);
            }
            Water.ParseDb(_buf, StartByteAddr);
            WaterAdd.ParseDb(_buf, StartByteAddr);
            WaterClose.ParseDb(_buf, StartByteAddr);

            M3Me.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
