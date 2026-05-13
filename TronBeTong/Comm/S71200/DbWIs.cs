using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    [Obsolete("Thay bằng Db26_WIs")]
    public class Db16_CanCl1_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db16_CanCl1_TT(): base(16, 6, 62)
        {
            KL = new PlcTag(TagTypes.Real, 62);
            TT = new PlcTag(TagTypes.Int16, 66);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db28_CanCl1_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db28_CanCl1_Me() : base(28, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db19_CanCl2_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db19_CanCl2_TT() : base(19, 6, 62)
        {
            KL = new PlcTag(TagTypes.Real, 62);
            TT = new PlcTag(TagTypes.Int16, 66);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db30_CanCl2_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db30_CanCl2_Me() : base(30, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db21_CanCl3_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db21_CanCl3_TT() : base(21, 6, 62)
        {
            KL = new PlcTag(TagTypes.Real, 62);
            TT = new PlcTag(TagTypes.Int16, 66);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }
    
    [Obsolete("Thay bằng Db26_WIs")]
    public class Db31_CanCl3_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db31_CanCl3_Me() : base(31, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db36_CanXM1_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db36_CanXM1_TT() : base(36, 6, 86)
        {
            KL = new PlcTag(TagTypes.Real, 86);
            TT = new PlcTag(TagTypes.Int16, 90);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db34_CanXM1_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db34_CanXM1_Me() : base(34, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db90_CanXM2_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db90_CanXM2_TT() : base(90, 6, 86)
        {
            KL = new PlcTag(TagTypes.Real, 86);
            TT = new PlcTag(TagTypes.Int16, 90);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db68_CanXM2_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db68_CanXM2_Me() : base(68, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db24_CanNuoc_TT : PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public Db24_CanNuoc_TT() : base(24, 6, 62)
        {
            KL = new PlcTag(TagTypes.Real, 62);
            TT = new PlcTag(TagTypes.Int16, 66);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db33_CanNuoc_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db33_CanNuoc_Me() : base(33, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class DB35_CanPG_TT: PlcDb
    {
        public PlcTag KL { get; private set; }
        public PlcTag TT { get; private set; }

        public DB35_CanPG_TT() : base(35, 6, 86)
        {
            KL = new PlcTag(TagTypes.Real, 86);
            TT = new PlcTag(TagTypes.Int16, 90);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            KL.ParseDb(_buf, StartByteAddr);
            TT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;

        }
    }

    [Obsolete("Thay bằng Db26_WIs")]
    public class Db23_CanPG_Me : PlcDb
    {
        public PlcTag MeHT { get; private set; }

        public Db23_CanPG_Me() : base(23, 2, 24)
        {
            MeHT = new PlcTag(TagTypes.Int16, 24);
            Cycle = 0.1;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            MeHT.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;
            IsParsingData = false;
        }
    }

}
