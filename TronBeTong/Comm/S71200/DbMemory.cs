using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class DbMemory1000 : PlcDb
    {
        public PlcTag SysRunning { get; private set; } = new PlcTag(TagTypes.Bool, 1000, 0);

        public DbMemory1000(): base(0, 1, 1000) {
            DbType = PlcDbTypes.M;
            Cycle = 0.2;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.Memory, _dbNo, StartByteAddr);
            IsParsingData = true;

            SysRunning.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
    public class DbMemory11 : PlcDb
    {
        public PlcTag SysStartSim { get; private set; } = new PlcTag(TagTypes.Bool, 11, 0);
        public PlcTag SysReset { get; private set; } = new PlcTag(TagTypes.Bool, 11, 1);
        public PlcTag SysWashMixer { get; private set; } = new PlcTag(TagTypes.Bool, 11, 2);

        public DbMemory11() : base(0, 1, 11)
        {
            DbType = PlcDbTypes.M;
            Cycle = 0.2;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.Memory, _dbNo, StartByteAddr);
            IsParsingData = true;

            SysStartSim.ParseDb(_buf, StartByteAddr);
            SysReset.ParseDb(_buf, StartByteAddr);
            SysWashMixer.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }

    public class DbMemory100 : PlcDb
    {
        public PlcTag MixerRunning { get; private set; } = new PlcTag(TagTypes.Bool, 100, 1);
        public PlcTag SensorMixerClose { get; private set; } = new PlcTag(TagTypes.Bool, 102, 5);
        public PlcTag SensorMixerOpen { get; private set; } = new PlcTag(TagTypes.Bool, 102, 6);
        public PlcTag SensorMixerOpenHalf { get; private set; } = new PlcTag(TagTypes.Bool, 102, 7);

        /// <summary>
        /// Chế độ xả
        /// </summary>
        public PlcTag ModeDischarge { get; private set; } = new PlcTag(TagTypes.Bool, 103, 2);
        /// <summary>
        /// Chế độ xả bê tông
        /// </summary>
        public PlcTag ModeDischargeConcrete { get; private set; } = new PlcTag(TagTypes.Bool, 102, 4);
        /// <summary>
        /// Chế độ cân
        /// </summary>
        public PlcTag ModeWeight { get; private set; } = new PlcTag(TagTypes.Bool, 103, 1);
        /// <summary>
        /// Chế độ xe skip
        /// </summary>
        public PlcTag ModeSkip { get; private set; } = new PlcTag(TagTypes.Bool, 107, 4);

        [Obsolete("Không dùng")]
        public PlcTag SensorBaoMoCLTG { get; private set; } = new PlcTag(TagTypes.Bool, 104, 6);
        [Obsolete("Không dùng")]
        public PlcTag SensorBaoDongCLTG { get; private set; } = new PlcTag(TagTypes.Bool, 101, 5);

        public PlcTag XeSkipDT0 { get; private set; } = new PlcTag(TagTypes.Bool, 107, 1);
        public PlcTag XeSkipDT1 { get; private set; } = new PlcTag(TagTypes.Bool, 107, 2);
        public PlcTag XeSkipDT2 { get; private set; } = new PlcTag(TagTypes.Bool, 107, 3);

        public PlcTag XeSkipDown { get; private set; } = new PlcTag(TagTypes.Bool, 107, 5);
        public PlcTag XeSkipUp { get; private set; } = new PlcTag(TagTypes.Bool, 107, 6);
        public PlcTag XeSkipEMC { get; private set; } = new PlcTag(TagTypes.Bool, 107, 7);

        public DbMemory100() : base(0, 8, 100)
        {
            DbType = PlcDbTypes.M;
            Cycle = 0.2;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.Memory, _dbNo, StartByteAddr);
            IsParsingData = true;

            ModeDischarge.ParseDb(_buf, StartByteAddr);
            ModeDischargeConcrete.ParseDb(_buf, StartByteAddr);
            ModeWeight.ParseDb(_buf, StartByteAddr);
            ModeSkip.ParseDb(_buf, StartByteAddr);

            MixerRunning.ParseDb(_buf, StartByteAddr);
            SensorMixerClose.ParseDb(_buf, StartByteAddr);
            SensorMixerOpen.ParseDb(_buf, StartByteAddr);
            SensorMixerOpenHalf.ParseDb(_buf, StartByteAddr);

            XeSkipDT0.ParseDb(_buf, StartByteAddr);
            XeSkipDT1.ParseDb(_buf, StartByteAddr);
            XeSkipDT2.ParseDb(_buf, StartByteAddr);
            XeSkipDown.ParseDb(_buf, StartByteAddr);
            XeSkipUp.ParseDb(_buf, StartByteAddr);
            XeSkipEMC.ParseDb(_buf, StartByteAddr);

            //SensorBaoMoCLTG.ParseDb(_buf, StartByteAddr);
            //SensorBaoDongCLTG.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }

    public class DbMemory200 : PlcDb
    {
        public PlcTag VanCL1 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 4);
        public PlcTag VanCL2 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 5);
        public PlcTag VanCL3 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 6);
        public PlcTag VanCL4 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 7);
        public PlcTag VanCL1Tinh { get; private set; } = new PlcTag(TagTypes.Bool, 201, 1);
        public PlcTag VanCL2Tinh { get; private set; } = new PlcTag(TagTypes.Bool, 203, 2);
        public PlcTag VanCL3Tinh { get; private set; } = new PlcTag(TagTypes.Bool, 203, 3);
        public PlcTag VanCL4Tinh { get; private set; } = new PlcTag(TagTypes.Bool, 203, 4);

        public PlcTag VanXM1 { get; private set; } = new PlcTag(TagTypes.Bool, 200, 4);
        public PlcTag VanXM2 { get; private set; } = new PlcTag(TagTypes.Bool, 203, 6);
        public PlcTag VanXM3 { get; private set; } = new PlcTag(TagTypes.Bool, 203, 7);
        public PlcTag VanXM4 { get; private set; } = new PlcTag(TagTypes.Bool, 204, 0);

        public PlcTag VanNuoc { get; private set; } = new PlcTag(TagTypes.Bool, 200, 6);

        public PlcTag VanCanCL1 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 0);
        public PlcTag VanCanCL2 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 1);
        public PlcTag VanCanCL3 { get; private set; } = new PlcTag(TagTypes.Bool, 202, 2);
        public PlcTag VanCanXM1 { get; private set; } = new PlcTag(TagTypes.Bool, 200, 5);
        public PlcTag VanCanXM2 { get; private set; } = new PlcTag(TagTypes.Bool, 205, 3);
        public PlcTag VanCanNuoc { get; private set; } = new PlcTag(TagTypes.Bool, 200, 7);
        
        /// <summary>
        /// Van cốt liệu trung gian
        /// </summary>
        public PlcTag VanCLTG { get; private set; } = new PlcTag(TagTypes.Bool, 200, 2);

        public PlcTag VanCanPG { get; private set; } = new PlcTag(TagTypes.Bool, 203, 1);
        public PlcTag VanPhuGia1 { get; private set; } = new PlcTag(TagTypes.Bool, 205, 5);
        public PlcTag VanPhuGia2 { get; private set; } = new PlcTag(TagTypes.Bool, 205, 6);

        public PlcTag MixerDischarge { get; private set; } = new PlcTag(TagTypes.Bool, 201, 0);
        
        [Obsolete ("Không dùng")]                
        public PlcTag BangTaiNgang { get; private set; } = new PlcTag(TagTypes.Bool, 200, 1);
        [Obsolete("Không dùng")]
        public PlcTag BangTaiXien { get; private set; } = new PlcTag(TagTypes.Bool, 200, 3);

        public DbMemory200() : base(0, 6, 200)
        {
            DbType = PlcDbTypes.M;
            Cycle = 0.2;
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.Memory, _dbNo, StartByteAddr);
            IsParsingData = true;

            VanCL1.ParseDb(_buf, StartByteAddr);
            VanCL2.ParseDb(_buf, StartByteAddr);
            VanCL3.ParseDb(_buf, StartByteAddr);
            VanCL4.ParseDb(_buf, StartByteAddr);
            VanXM1.ParseDb(_buf, StartByteAddr);
            VanXM2.ParseDb(_buf, StartByteAddr);
            VanXM3.ParseDb(_buf, StartByteAddr);
            VanXM4.ParseDb(_buf, StartByteAddr);
            VanNuoc.ParseDb(_buf, StartByteAddr);
            VanCL1Tinh.ParseDb(_buf, StartByteAddr);
            VanCL2Tinh.ParseDb(_buf, StartByteAddr);
            VanCL3Tinh.ParseDb(_buf, StartByteAddr);
            VanCL4Tinh.ParseDb(_buf, StartByteAddr);
            VanPhuGia1.ParseDb(_buf, StartByteAddr);
            VanPhuGia2.ParseDb(_buf, StartByteAddr);

            VanCanCL1.ParseDb(_buf, StartByteAddr);
            VanCanCL2.ParseDb(_buf, StartByteAddr);
            VanCanCL3.ParseDb(_buf, StartByteAddr);
            VanCanXM1.ParseDb(_buf, StartByteAddr);
            VanCanXM2.ParseDb(_buf, StartByteAddr);
            VanCanNuoc.ParseDb(_buf, StartByteAddr);
            VanCanPG.ParseDb(_buf, StartByteAddr);

            VanCLTG.ParseDb(_buf, StartByteAddr);
            MixerDischarge.ParseDb(_buf, StartByteAddr);
            //BangTaiNgang.ParseDb(_buf, StartByteAddr);
            //BangTaiXien.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
