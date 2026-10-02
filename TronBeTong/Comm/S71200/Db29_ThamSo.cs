using NMComm.S71200;
using S7.Net;

namespace TronBeTongV3.Comm.S71200
{
    public class Db29_ThamSo : PlcDb
    {
        #region Trễ khởi động
        public PlcTag[] TreKhoiDongCL { get; private set; } = new PlcTag[5];
        public PlcTag[] TreKhoiDongXM { get; private set; } = new PlcTag[2];
        public PlcTag[] TreKhoiDongNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] TreKhoiDongPG { get; private set; } = new PlcTag[2];
        #endregion

        #region Trễ xả cân
        public PlcTag[] TreXaCanCL { get; private set; } = new PlcTag[5];
        public PlcTag[] TreXaCanXM { get; private set; } = new PlcTag[2];
        public PlcTag[] TreXaCanNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] TreXaCanPG { get; private set; } = new PlcTag[2];
        #endregion

        #region TG vào băng tải xiên
        public PlcTag[] TGTreXaCLXuongBangTai { get; private set; } = new PlcTag[5];
        #endregion

        #region Trễ đóng cửa xả
        public PlcTag[] TreDongCuaXaCL { get; private set; } = new PlcTag[5];
        public PlcTag[] TreDongCuaXaXM { get; private set; } = new PlcTag[2];
        public PlcTag[] TreDongCuaXaNuoc { get; private set; } = new PlcTag[2];
        public PlcTag[] TreDongCuaXaPG { get; private set; } = new PlcTag[2];
        #endregion

        #region TG đóng/mở nháy xả cốt liệu
        public PlcTag[] TGDongNhayCL { get; private set; } = new PlcTag[5];
        public PlcTag[] TGMoNhayCL { get; private set; } = new PlcTag[5];
        #endregion

        #region Rung & sục khí
        public PlcTag[] SucKhiTimerOn { get; private set; } = new PlcTag[4];
        public PlcTag[] SucKhiTimerOff { get; private set; } = new PlcTag[4];

        public PlcTag[] RungCLOn { get; private set; } = new PlcTag[5];
        public PlcTag[] RungCLCycle { get; private set; } = new PlcTag[5];

        public PlcTag[] RungTCXMOn { get; private set; } = new PlcTag[2];
        //public PlcTag[] RungTCXMCycle { get; private set; } = new PlcTag[2];

        public PlcTag RungTCXMCycle { get; private set; } = new PlcTag(TagTypes.Int16, 158);
        public PlcTag RungCLTGTre { get; private set; } = new PlcTag(TagTypes.Int16, 160);
        #endregion

        #region Read TG Trễ mở xả
        public PlcTag[] TGTreXaCL { get; private set; } = new PlcTag[5];
        public PlcTag[] TGTimeoutXaCL { get; private set; } = new PlcTag[5];
        public PlcTag[] AlarmTimeoutXaCL { get; private set; } = new PlcTag[5];
        #endregion

        #region Bơm mỡ
        public PlcTag BomMoTGOn { get; private set; } = new PlcTag(TagTypes.Int16, 162);
        public PlcTag BomMoTGOff { get; private set; } = new PlcTag(TagTypes.Int16, 164);
        #endregion


        #region Xe skip
        public PlcTag TGDungDoCLDT2 { get; private set; } = new PlcTag(TagTypes.Int16, 166);
        /// <summary>
        /// TG trễ dừng DTO
        /// </summary>
        public PlcTag TGTrungCap { get; private set; } = new PlcTag(TagTypes.Real, 168);
        public PlcTag TGXeSkipDT0_DT2 { get; private set; } = new PlcTag(TagTypes.Int16, 172);
        public PlcTag TGXeSkipDT0_DT1 { get; private set; } = new PlcTag(TagTypes.Int16, 174);
        public PlcTag GHXeSkipXuongDTO { get; private set; } = new PlcTag(TagTypes.Int16, 176);         // Chưa dùng
        #endregion


        #region Khác
        public PlcTag TGCotLieuLenCLTG_ET { get; private set; } = new PlcTag(TagTypes.Int16, 178);
        public PlcTag TGMoXaCLTG_ET { get; private set; } = new PlcTag(TagTypes.Int16, 180);
        public PlcTag TGTronBeTong_ET { get; private set; } = new PlcTag(TagTypes.Int16, 182);
        public PlcTag TGTreMoCoiTronHalf_ET { get; private set; } = new PlcTag(TagTypes.Int16, 184);
        public PlcTag TGTreMoCoiTron_ET { get; private set; } = new PlcTag(TagTypes.Int16, 186);
        public PlcTag TGTreXaXeSkip_ET { get; private set; } = new PlcTag(TagTypes.Int16, 188);

        public PlcTag TGCLDiQuaBTX { get; private set; } = new PlcTag(TagTypes.Int16, 190);
        public PlcTag TGMoThungCLTG { get; private set; } = new PlcTag(TagTypes.Int16, 192);

        public PlcTag TGTronBeTong { get; private set; } = new PlcTag(TagTypes.Int16, 194);
        public PlcTag TGTreMoCoiTronHalf { get; private set; } = new PlcTag(TagTypes.Int16, 196);
        public PlcTag TGTreMoCoiTron { get; private set; } = new PlcTag(TagTypes.Int16, 198);
        public PlcTag TGTreDungBTXMeCuoi { get; private set; } = new PlcTag(TagTypes.Int16, 200);
        public PlcTag TGChuTrinhDamRung { get; private set; } = new PlcTag(TagTypes.Int16, 202);
        public PlcTag TGBatDamRung { get; private set; } = new PlcTag(TagTypes.Int16, 204);
        #endregion

        public Db29_ThamSo() : base(29, 206, 0)
        {
            #region Trễ khởi động
            for (int i = 0; i < 5; i++)
            {
                TreKhoiDongCL[i] = new PlcTag(TagTypes.Int16, 0 + i * 2);
            }
            for (int i = 0; i < 2; i++)
            {
                TreKhoiDongXM[i] = new PlcTag(TagTypes.Int16, 10 + i * 2);
                TreKhoiDongNuoc[i] = new PlcTag(TagTypes.Int16, 14 + i * 2);
                TreKhoiDongPG[i] = new PlcTag(TagTypes.Int16, 18 + i * 2);
            }
            #endregion

            #region Trễ xả cân
            for (int i = 0; i < 5; i++)
            {
                TreXaCanCL[i] = new PlcTag(TagTypes.Int16, 22 + i * 2);
            }
            for (int i = 0; i < 2; i++)
            {
                TreXaCanXM[i] = new PlcTag(TagTypes.Int16, 32 + i * 2);
                TreXaCanNuoc[i] = new PlcTag(TagTypes.Int16, 36 + i * 2);
                TreXaCanPG[i] = new PlcTag(TagTypes.Int16, 40 + i * 2);
            }
            #endregion

            #region Thời gian trễ xả cốt liệu xuống băng tải
            for (int i = 0; i < 5; i++)
            {
                TGTreXaCLXuongBangTai[i] = new PlcTag(TagTypes.Int16, 44 + i * 2);
            }
            #endregion

            #region Trễ đóng cửa xả
            for (int i = 0; i < 5; i++)
            {
                TreDongCuaXaCL[i] = new PlcTag(TagTypes.Int16, 54 + i * 2);
            }
            for (int i = 0; i < 2; i++)
            {
                TreDongCuaXaXM[i] = new PlcTag(TagTypes.Int16, 64 + i * 2);
                TreDongCuaXaNuoc[i] = new PlcTag(TagTypes.Int16, 68 + i * 2);
                TreDongCuaXaPG[i] = new PlcTag(TagTypes.Int16, 72 + i * 2);
            }
            #endregion

            #region TG Chu trình xả cốt liệu
            for (int i = 0; i < 5; i++)
            {
                TGDongNhayCL[i] = new PlcTag(TagTypes.Int16, 76 + i * 2);
                TGMoNhayCL[i] = new PlcTag(TagTypes.Int16, 86 + i * 2);
            }
            #endregion

            #region Rung và sục khí
            for (int i = 0; i < 4; i++)
            {
                SucKhiTimerOn[i] = new PlcTag(TagTypes.Int16, 96 + i * 2);
                SucKhiTimerOff[i] = new PlcTag(TagTypes.Int16, 104 + i * 2);
            }

            for (int i = 0; i < 5; i++)
            {
                RungCLOn[i] = new PlcTag(TagTypes.Int16, 112 + i * 2);
                RungCLCycle[i] = new PlcTag(TagTypes.Int16, 122 + i * 2);
            }

            for (int i = 0; i < 2; i++)
            {
                RungTCXMOn[i] = new PlcTag(TagTypes.Int16, 132 + i * 2);
                //RungTCXMCycle[i] = new PlcTag(TagTypes.Int16, 132 + i * 2);       // Dùng chung RungTCXMCycle
            }
            #endregion

            #region Read TG Trễ mở xả cl
            for (int i = 0; i < 5; i++)
            {
                TGTreXaCL[i] = new PlcTag(TagTypes.Int16, 136 + i * 2);
                TGTimeoutXaCL[i] = new PlcTag(TagTypes.Int16, 146 + i * 2);
                AlarmTimeoutXaCL[i] = new PlcTag(TagTypes.Bool, 156, i);
            }
            #endregion
        }

        public override async Task ReadAsync(Plc plc, double delta)
        {
            await plc.ReadBytesAsync(_buf, DataType.DataBlock, _dbNo, StartByteAddr);
            IsParsingData = true;

            #region Trễ khởi động
            for (int i = 0; i < 5; i++)
            {
                TreKhoiDongCL[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < 2; i++)
            {
                TreKhoiDongXM[i].ParseDb(_buf, StartByteAddr);
                TreKhoiDongNuoc[i].ParseDb(_buf, StartByteAddr);
                TreKhoiDongPG[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            #region Trễ xả
            for (int i = 0; i < 5; i++)
            {
                TreXaCanCL[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < 2; i++)
            {
                TreXaCanXM[i].ParseDb(_buf, StartByteAddr);
                TreXaCanNuoc[i].ParseDb(_buf, StartByteAddr);
                TreXaCanPG[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            #region TG trễ xả cốt liệu xuống băng tải
            for (int i = 0; i < 5; i++)
            {
                TGTreXaCLXuongBangTai[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            #region Trễ đóng cửa xả
            for (int i = 0; i < 5; i++)
            {
                TreDongCuaXaCL[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < 2; i++)
            {
                TreDongCuaXaXM[i].ParseDb(_buf, StartByteAddr);
                TreDongCuaXaNuoc[i].ParseDb(_buf, StartByteAddr);
                TreDongCuaXaPG[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            #region TG Chu trình xả cốt liệu
            for (int i = 0; i < 5; i++)
            {
                TGDongNhayCL[i].ParseDb(_buf, StartByteAddr);
                TGMoNhayCL[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion


            #region Rung & sục khí
            for (int i = 0; i < 4; i++)
            {
                SucKhiTimerOn[i].ParseDb(_buf, StartByteAddr);
                SucKhiTimerOff[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < 5; i++)
            {
                RungCLOn[i].ParseDb(_buf, StartByteAddr);
                RungCLCycle[i].ParseDb(_buf, StartByteAddr);
            }
            for (int i = 0; i < 2; i++)
            {
                RungTCXMOn[i].ParseDb(_buf, StartByteAddr);
            }
            RungTCXMCycle.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Read TG Tre Mo Xa
            for (int i = 0; i < 5; i++)
            {
                TGTreXaCL[i].ParseDb(_buf, StartByteAddr);
                TGTimeoutXaCL[i].ParseDb(_buf, StartByteAddr);
                AlarmTimeoutXaCL[i].ParseDb(_buf, StartByteAddr);
            }
            #endregion

            RungCLTGTre.ParseDb(_buf, StartByteAddr);

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

            #region Bơm mỡ
            BomMoTGOn.ParseDb(_buf, StartByteAddr);
            BomMoTGOff.ParseDb(_buf, StartByteAddr);
            #endregion

            #region Xe skip
            TGDungDoCLDT2.ParseDb(_buf, StartByteAddr);
            TGTrungCap.ParseDb(_buf, StartByteAddr);
            TGXeSkipDT0_DT2.ParseDb(_buf, StartByteAddr);
            TGXeSkipDT0_DT1.ParseDb(_buf, StartByteAddr);
            #endregion

            //TGCotLieuLenCLTG_ET.ParseDb(_buf, StartByteAddr);
            //TGMoXaCLTG_ET.ParseDb(_buf, StartByteAddr);
            //TGTronBeTong_ET.ParseDb(_buf, StartByteAddr);
            //TGTreMoCoiTronHalf_ET.ParseDb(_buf, StartByteAddr);
            //TGTreMoCoiTron_ET.ParseDb(_buf, StartByteAddr);
            //TGTreXaXeSkip_ET.ParseDb(_buf, StartByteAddr);

            T = DateTime.Now.Ticks;

            IsParsingData = false;
        }
    }
}
