using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db29_ThamSo : PlcDb
    {
        #region Trễ khởi động
        public PlcTag TreKhoiDongCL1 { get; private set; }
        public PlcTag TreKhoiDongCL2 { get; private set; }
        public PlcTag TreKhoiDongCL3 { get; private set; }
        public PlcTag TreKhoiDongCL4 { get; private set; }
        public PlcTag TreKhoiDongCL5 { get; private set; }
        public PlcTag TreKhoiDongCL6 { get; private set; }
        public PlcTag TreKhoiDongXM1 { get; private set; }
        public PlcTag TreKhoiDongXM2 { get; private set; }
        public PlcTag TreKhoiDongNuoc { get; private set; }
        public PlcTag TreKhoiDongPG1 { get; private set; }
        #endregion

        #region Trễ xả cân
        public PlcTag TreXaCanCL1 { get; private set; }
        public PlcTag TreXaCanCL2 { get; private set; }
        public PlcTag TreXaCanCL3 { get; private set; }
        public PlcTag TreXaCanCL4 { get; private set; }
        public PlcTag TreXaCanCL5 { get; private set; }
        public PlcTag TreXaCanCL6 { get; private set; }
        public PlcTag TreXaCanXM1 { get; private set; }
        public PlcTag TreXaCanXM2 { get; private set; }
        public PlcTag TreXaCanNuoc { get; private set; }
        public PlcTag TreXaCanPG1 { get; private set; }
        #endregion

        #region TG vào băng tải xiên
        public PlcTag TGVaoBTXCL1 { get; private set; }
        public PlcTag TGVaoBTXCL2 { get; private set; }
        public PlcTag TGVaoBTXCL3 { get; private set; }
        public PlcTag TGVaoBTXCL4 { get; private set; }
        public PlcTag TGVaoBTXCL5 { get; private set; }
        public PlcTag TGVaoBTXCL6 { get; private set; }
        #endregion

        #region Trễ đóng cửa xả
        public PlcTag TreDongCuaXaCl1 { get; private set; }
        public PlcTag TreDongCuaXaCl2 { get; private set; }
        public PlcTag TreDongCuaXaCl3 { get; private set; }
        public PlcTag TreDongCuaXaCl4 { get; private set; }
        public PlcTag TreDongCuaXaCl5 { get; private set; }
        public PlcTag TreDongCuaXaCl6 { get; private set; }
        public PlcTag TreDongCuaXaXM1 { get; private set; }
        public PlcTag TreDongCuaXaXM2 { get; private set; }
        public PlcTag TreDongCuaXaNuoc { get; private set; }
        public PlcTag TreDongCuaXaPG1 { get; private set; }
        #endregion

        #region TG Chu trình xả cốt liệu
        public PlcTag TGChuTrinhXaCL1 { get; private set; }
        public PlcTag TGChuTrinhXaCL2 { get; private set; }
        public PlcTag TGChuTrinhXaCL3 { get; private set; }
        public PlcTag TGChuTrinhXaCL4 { get; private set; }
        public PlcTag TGChuTrinhXaCL5 { get; private set; }
        public PlcTag TGChuTrinhXaCL6 { get; private set; }
        #endregion

        #region TG Mở xả cốt liệu
        public PlcTag TGMoXaCL1 { get; private set; }
        public PlcTag TGMoXaCL2 { get; private set; }
        public PlcTag TGMoXaCL3 { get; private set; }
        public PlcTag TGMoXaCL4 { get; private set; }
        public PlcTag TGMoXaCL5 { get; private set; }
        public PlcTag TGMoXaCL6 { get; private set; }
        #endregion

        #region Khác
        public PlcTag TGCLDiQuaBTX { get; private set; }
        public PlcTag TGMoThungCLTG { get; private set; }
        public PlcTag TGTronBeTong { get; private set; }
        public PlcTag TGTreMoCoiTronHalf { get; private set; }
        public PlcTag TGTreMoCoiTron { get; private set; }
        public PlcTag TGTreDungBTXMeCuoi { get; private set; }
        public PlcTag TGChuTrinhDamRung { get; private set; }
        public PlcTag TGBatDamRung { get; private set; }
        #endregion

        #region Rung & sục khí
        public PlcTag SucKhiTimerOn { get; private set; }
        public PlcTag SucKhiCycle { get; private set; }
        public PlcTag RungCL1On { get; private set; }
        public PlcTag RungCL1Cycle { get; private set; }
        public PlcTag RungCL2On { get; private set; }
        public PlcTag RungCL2Cycle { get; private set; }
        public PlcTag RungCL3On { get; private set; }
        public PlcTag RungCL3Cycle { get; private set; }
        public PlcTag RungCL4On { get; private set; }
        public PlcTag RungCL4Cycle { get; private set; }
        public PlcTag RungCL5On { get; private set; }
        public PlcTag RungCL5Cycle { get; private set; }
        public PlcTag RungCL6On { get; private set; }
        public PlcTag RungCL6Cycle { get; private set; }

        public PlcTag RungTCXM1On { get; private set; }
        public PlcTag RungTCXM1Cycle { get; private set; }
        public PlcTag RungTCXM2On { get; private set; }
        public PlcTag RungTCXM2Cycle { get; private set; }
        public PlcTag RungCLTGTre { get; private set; }

        #endregion

        #region Bơm mỡ
        public PlcTag BomMoTGOn { get; private set; }
        public PlcTag BomMoTGOff { get; private set; }
        #endregion

        #region Read TG Trễ mở xả
        public PlcTag ReadTGTreMoXaCL1 { get; private set; }
        public PlcTag ReadTGTreMoXaCL2 { get; private set; }
        public PlcTag ReadTGTreMoXaCL3 { get; private set; }
        public PlcTag ReadTGTreMoXaCL4 { get; private set; }
        public PlcTag ReadTGTreMoXaCL5 { get; private set; }
        public PlcTag ReadTGTreMoXaCL6 { get; private set; }
        #endregion

        #region Xe skip
        public PlcTag TGDungDoCLDT2 { get; private set; } = new PlcTag(TagTypes.Int16, 190);
        public PlcTag TGTrungCap { get; private set; } = new PlcTag(TagTypes.Real, 192);
        public PlcTag TGXeSkipDT0_DT2 { get; private set; } = new PlcTag(TagTypes.Int16, 196);
        public PlcTag TGXeSkipDT0_DT1 { get; private set; } = new PlcTag(TagTypes.Int16, 198);
        #endregion

        public Db29_ThamSo() : base(29, 200, 0)
        {
            #region Trễ khởi động
            TreKhoiDongCL1 = new PlcTag(TagTypes.Int16, 0);
            TreKhoiDongCL2 = new PlcTag(TagTypes.Int16, 2);
            TreKhoiDongCL3 = new PlcTag(TagTypes.Int16, 4);
            TreKhoiDongCL4 = new PlcTag(TagTypes.Int16, 6);
            TreKhoiDongCL5 = new PlcTag(TagTypes.Int16, 8);
            TreKhoiDongCL6 = new PlcTag(TagTypes.Int16, 10);
            TreKhoiDongXM1 = new PlcTag(TagTypes.Int16, 12);
            TreKhoiDongXM2 = new PlcTag(TagTypes.Int16, 14);
            TreKhoiDongNuoc = new PlcTag(TagTypes.Int16, 16);
            TreKhoiDongPG1 = new PlcTag(TagTypes.Int16, 18);
            #endregion

            #region Trễ xả cân
            TreXaCanCL1 = new PlcTag(TagTypes.Int16, 20);
            TreXaCanCL2 = new PlcTag(TagTypes.Int16, 22);
            TreXaCanCL3 = new PlcTag(TagTypes.Int16, 24);
            TreXaCanCL4 = new PlcTag(TagTypes.Int16, 26);
            TreXaCanCL5 = new PlcTag(TagTypes.Int16, 28);
            TreXaCanCL6 = new PlcTag(TagTypes.Int16, 30);
            TreXaCanXM1 = new PlcTag(TagTypes.Int16, 32);
            TreXaCanXM2 = new PlcTag(TagTypes.Int16, 34);
            TreXaCanNuoc = new PlcTag(TagTypes.Int16, 36);
            TreXaCanPG1 = new PlcTag(TagTypes.Int16, 38);
            #endregion

            #region Thời gian vào băng tải xiên
            TGVaoBTXCL1 = new PlcTag(TagTypes.Int16, 40);
            TGVaoBTXCL2 = new PlcTag(TagTypes.Int16, 42);
            TGVaoBTXCL3 = new PlcTag(TagTypes.Int16, 44);
            TGVaoBTXCL4 = new PlcTag(TagTypes.Int16, 46);
            TGVaoBTXCL5 = new PlcTag(TagTypes.Int16, 48);
            TGVaoBTXCL6 = new PlcTag(TagTypes.Int16, 50);
            #endregion

            #region Trễ đóng cửa xả
            TreDongCuaXaCl1 = new PlcTag(TagTypes.Int16, 52);
            TreDongCuaXaCl2 = new PlcTag(TagTypes.Int16, 54);
            TreDongCuaXaCl3 = new PlcTag(TagTypes.Int16, 56);
            TreDongCuaXaCl4 = new PlcTag(TagTypes.Int16, 58);
            TreDongCuaXaCl5 = new PlcTag(TagTypes.Int16, 60);
            TreDongCuaXaCl6 = new PlcTag(TagTypes.Int16, 62);
            TreDongCuaXaXM1 = new PlcTag(TagTypes.Int16, 64);
            TreDongCuaXaXM2 = new PlcTag(TagTypes.Int16, 66);
            TreDongCuaXaNuoc = new PlcTag(TagTypes.Int16, 68);
            TreDongCuaXaPG1 = new PlcTag(TagTypes.Int16, 70);
            #endregion

            #region TG Chu trình xả cốt liệu
            TGChuTrinhXaCL1 = new PlcTag(TagTypes.Int16, 88);
            TGChuTrinhXaCL2 = new PlcTag(TagTypes.Int16, 94);
            TGChuTrinhXaCL3 = new PlcTag(TagTypes.Int16, 100);
            TGChuTrinhXaCL4 = new PlcTag(TagTypes.Int16, 106);
            TGChuTrinhXaCL5 = new PlcTag(TagTypes.Int16, 112);
            TGChuTrinhXaCL6 = new PlcTag(TagTypes.Int16, 118);
            #endregion
            
            #region TG mở xả cốt liệu
            TGMoXaCL1 = new PlcTag(TagTypes.Int16, 178);
            TGMoXaCL2 = new PlcTag(TagTypes.Int16, 180);
            TGMoXaCL3 = new PlcTag(TagTypes.Int16, 182);
            TGMoXaCL4 = new PlcTag(TagTypes.Int16, 184);
            TGMoXaCL5 = new PlcTag(TagTypes.Int16, 186);
            TGMoXaCL6 = new PlcTag(TagTypes.Int16, 188);
            #endregion

            #region Khác
            TGCLDiQuaBTX = new PlcTag(TagTypes.Int16, 72);
            TGMoThungCLTG = new PlcTag(TagTypes.Int16, 74);
            TGTronBeTong = new PlcTag(TagTypes.Int16, 76);
            TGTreMoCoiTronHalf = new PlcTag(TagTypes.Int16, 78);
            TGTreMoCoiTron = new PlcTag(TagTypes.Int16, 80);

            TGTreDungBTXMeCuoi = new PlcTag(TagTypes.Int16, 82);
            TGChuTrinhDamRung = new PlcTag(TagTypes.Int16, 84);
            TGBatDamRung = new PlcTag(TagTypes.Int16, 86);
            #endregion

            #region Rung và sục khí
            SucKhiTimerOn = new PlcTag(TagTypes.Int16, 124);
            SucKhiCycle = new PlcTag(TagTypes.Int16, 126);

            RungCL1On = new PlcTag(TagTypes.Int16, 128);
            RungCL1Cycle = new PlcTag(TagTypes.Int16, 130);
            RungCL2On = new PlcTag(TagTypes.Int16, 132);
            RungCL2Cycle = new PlcTag(TagTypes.Int16, 134);
            RungCL3On = new PlcTag(TagTypes.Int16, 136);
            RungCL3Cycle = new PlcTag(TagTypes.Int16, 138);
            RungCL4On = new PlcTag(TagTypes.Int16, 140);
            RungCL4Cycle = new PlcTag(TagTypes.Int16, 142);
            RungCL5On = new PlcTag(TagTypes.Int16, 144);
            RungCL5Cycle = new PlcTag(TagTypes.Int16, 146);
            RungCL6On = new PlcTag(TagTypes.Int16, 148);
            RungCL6Cycle = new PlcTag(TagTypes.Int16, 150);

            RungTCXM1On = new PlcTag(TagTypes.Int16, 152);
            RungTCXM1Cycle = new PlcTag(TagTypes.Int16, 154);
            RungTCXM2On = new PlcTag(TagTypes.Int16, 156);
            RungTCXM2Cycle = new PlcTag(TagTypes.Int16, 158);

            RungCLTGTre = new PlcTag(TagTypes.Int16, 160);
            #endregion

            #region Bơm mỡ
            BomMoTGOn = new PlcTag(TagTypes.Int16, 162);
            BomMoTGOff = new PlcTag(TagTypes.Int16, 164);
            #endregion

            #region Read TG Trễ mở xả
            ReadTGTreMoXaCL1 = new PlcTag(TagTypes.Int16, 166);
            ReadTGTreMoXaCL2 = new PlcTag(TagTypes.Int16, 168);
            ReadTGTreMoXaCL3 = new PlcTag(TagTypes.Int16, 170);
            ReadTGTreMoXaCL4 = new PlcTag(TagTypes.Int16, 172);
            ReadTGTreMoXaCL5 = new PlcTag(TagTypes.Int16, 174);
            ReadTGTreMoXaCL6 = new PlcTag(TagTypes.Int16, 176);
            #endregion
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            #region Trễ khởi động
            TreKhoiDongCL1.ParseDb(_buf, StartByteAddr);
            TreKhoiDongCL2.ParseDb(_buf, StartByteAddr);
            TreKhoiDongCL3.ParseDb(_buf, StartByteAddr);
            TreKhoiDongCL4.ParseDb(_buf, StartByteAddr);
            //TreKhoiDongCL5.ParseDb(_buf, StartByteAddr);
            //TreKhoiDongCL6.ParseDb(_buf, StartByteAddr);
            TreKhoiDongXM1.ParseDb(_buf, StartByteAddr);
            TreKhoiDongXM2.ParseDb(_buf, StartByteAddr);
            TreKhoiDongNuoc.ParseDb(_buf, StartByteAddr);
            TreKhoiDongPG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Trễ xả
            TreXaCanCL1.ParseDb(_buf, StartByteAddr);
            TreXaCanCL2.ParseDb(_buf, StartByteAddr);
            TreXaCanCL3.ParseDb(_buf, StartByteAddr);
            TreXaCanCL4.ParseDb(_buf, StartByteAddr);
            //TreXaCanCL5.ParseDb(_buf, StartByteAddr);
            //TreXaCanCL6.ParseDb(_buf, StartByteAddr);
            TreXaCanXM1.ParseDb(_buf, StartByteAddr);
            TreXaCanXM2.ParseDb(_buf, StartByteAddr);
            TreXaCanNuoc.ParseDb(_buf, StartByteAddr);
            TreXaCanPG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region TG vào BTX
            TGVaoBTXCL1.ParseDb(_buf, StartByteAddr);
            TGVaoBTXCL2.ParseDb(_buf, StartByteAddr);
            TGVaoBTXCL3.ParseDb(_buf, StartByteAddr);
            TGVaoBTXCL4.ParseDb(_buf, StartByteAddr);
            //TGVaoBTXCL5.ParseDb(_buf, StartByteAddr);
            //TGVaoBTXCL6.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Trễ đóng cửa xả
            TreDongCuaXaCl1.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaCl2.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaCl3.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaCl4.ParseDb(_buf, StartByteAddr);
            //TreDongCuaXaCl5.ParseDb(_buf, StartByteAddr);
            //TreDongCuaXaCl6.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaXM1.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaXM2.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaNuoc.ParseDb(_buf, StartByteAddr);
            TreDongCuaXaPG1.ParseDb(_buf, StartByteAddr);
            #endregion

            #region TG Chu trình xả cốt liệu
            TGChuTrinhXaCL1.ParseDb(_buf, StartByteAddr);
            TGChuTrinhXaCL2.ParseDb(_buf, StartByteAddr);
            TGChuTrinhXaCL3.ParseDb(_buf, StartByteAddr);
            TGChuTrinhXaCL4.ParseDb(_buf, StartByteAddr);
            TGChuTrinhXaCL5.ParseDb(_buf, StartByteAddr);
            TGChuTrinhXaCL6.ParseDb(_buf, StartByteAddr);
            #endregion
            
            #region TG mở xả cốt liệu
            TGMoXaCL1.ParseDb(_buf, StartByteAddr);
            TGMoXaCL2.ParseDb(_buf, StartByteAddr);
            TGMoXaCL3.ParseDb(_buf, StartByteAddr);
            TGMoXaCL4.ParseDb(_buf, StartByteAddr);
            TGMoXaCL5.ParseDb(_buf, StartByteAddr);
            TGMoXaCL6.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Khác
            TGCLDiQuaBTX.ParseDb(_buf, StartByteAddr);
            TGMoThungCLTG.ParseDb(_buf, StartByteAddr);
            TGTronBeTong.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTronHalf.ParseDb(_buf, StartByteAddr);
            TGTreMoCoiTron.ParseDb(_buf, StartByteAddr);
            TGTreDungBTXMeCuoi.ParseDb(_buf, StartByteAddr);
            TGChuTrinhDamRung.ParseDb(_buf, StartByteAddr);
            TGBatDamRung.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Rung & sục khí
            SucKhiTimerOn.ParseDb(_buf, StartByteAddr);
            SucKhiCycle.ParseDb(_buf, StartByteAddr);

            RungCL1On.ParseDb(_buf, StartByteAddr);
            RungCL1Cycle.ParseDb(_buf, StartByteAddr);
            RungCL2On.ParseDb(_buf, StartByteAddr);
            RungCL2Cycle.ParseDb(_buf, StartByteAddr);
            RungCL3On.ParseDb(_buf, StartByteAddr);
            RungCL3Cycle.ParseDb(_buf, StartByteAddr);
            //RungCL4On.ParseDb(_buf, StartByteAddr);
            //RungCL4Cycle.ParseDb(_buf, StartByteAddr);
            //RungCL5On.ParseDb(_buf, StartByteAddr);
            //RungCL5Cycle.ParseDb(_buf, StartByteAddr);
            //RungCL6On.ParseDb(_buf, StartByteAddr);
            //RungCL6Cycle.ParseDb(_buf, StartByteAddr);

            RungTCXM1On.ParseDb(_buf, StartByteAddr);
            RungTCXM1Cycle.ParseDb(_buf, StartByteAddr);
            RungTCXM2On.ParseDb(_buf, StartByteAddr);
            RungTCXM2Cycle.ParseDb(_buf, StartByteAddr);

            RungCLTGTre.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Bơm mỡ
            BomMoTGOn.ParseDb(_buf, StartByteAddr);
            BomMoTGOff.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Read TG Tre Mo Xa
            //ReadTGTreMoXaCL1.ParseDb(_buf, StartByteAddr);
            //ReadTGTreMoXaCL2.ParseDb(_buf, StartByteAddr);
            //ReadTGTreMoXaCL3.ParseDb(_buf, StartByteAddr);
            //ReadTGTreMoXaCL4.ParseDb(_buf, StartByteAddr);
            //ReadTGTreMoXaCL5.ParseDb(_buf, StartByteAddr);
            //ReadTGTreMoXaCL6.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Xe skip
            TGDungDoCLDT2.ParseDb(_buf, StartByteAddr);
            TGTrungCap.ParseDb(_buf, StartByteAddr);
            TGXeSkipDT0_DT2.ParseDb(_buf, StartByteAddr);
            TGXeSkipDT0_DT1.ParseDb(_buf, StartByteAddr);
            #endregion

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
