using System;
using NMComm;
using NMComm.S71200;
using Serilog;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.ViewModel.DonHang;

namespace TronBeTongV3.Comm
{
    public enum ConnectionTypes { None, S7Net, KEPServer }

    public abstract class ModelHeThong
    {
        private readonly DbRepository _r = DbRepository.Instance;

        #region Cấu hình (Số thành phần)
        public const int SoCLMax = 6;
        public const int SoXMMax = 5;
        public const int SoPGMax = 8;
        public const int SoCanXMMax = 2;
        public const int SoCanPGMax = 2;
        public const int SoCanCLMax = 4;
        /// <summary>
        /// Số thành phần cốt liệu thực tế
        /// </summary>
        public static int SoCLReal = 3;
        /// <summary>
        /// Số thành phần xi thực tế (CHÚ Ý: THAY ĐỔI THEO TRẠM)
        /// </summary>
        public static int SoXMReal = 1;
        /// <summary>
        /// Số thành phần phụ gia thực tế
        /// </summary>
        public static int SoPGReal = 2;
        /// <summary>
        /// Số cân cốt liệu thực tế
        /// </summary>
        public static int SoCanCLReal = 3;
        /// <summary>
        /// Số cân xi măng thực tế
        /// </summary>
        public static int SoCanXMReal = 2;
        /// <summary>
        /// Số cân phụ gia thực tế
        /// </summary>
        public static int SoCanPGReal = 3;
        #endregion

        #region Tags
        #region Thành phần
        public ModelTag[] CLCapPhois { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] ClAdds { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] ClDoAms { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] ClChots { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] CLCPMes { get; private set; } = new ModelTag[SoCLMax];

        public ModelTag[] XMCapPhois { get; private set; } = new ModelTag[SoXMMax];
        public ModelTag[] XMAdds { get; private set; } = new ModelTag[SoXMMax];
        public ModelTag[] XMChots { get; private set; } = new ModelTag[SoXMMax];
        public ModelTag[] XMCPMes { get; private set; } = new ModelTag[SoXMMax];

        public ModelTag[] PGCapPhois { get; private set; } = new ModelTag[SoPGMax];
        public ModelTag[] PGAdds { get; private set; } = new ModelTag[SoPGMax];
        public ModelTag[] PGChots { get; private set; } = new ModelTag[SoPGMax];
        public ModelTag[] PGCPMes { get; private set; } = new ModelTag[SoPGMax];

        public ModelTag NuocCapPhoi { get; private set; } = new ModelTag("Nuoc.CP");
        public ModelTag NuocAdd { get; private set; } = new ModelTag("Nuoc.Add");
        public ModelTag NuocChot { get; private set; } = new ModelTag("Nuoc.Chot");
        public ModelTag NuocCPMe { get; private set; } = new ModelTag("Nuoc.CP.Me");
        #endregion

        #region Mẻ
        /// <summary>
        /// Số m3 đặt
        /// </summary>
        public ModelTag MeM3Dat { get; private set; } = new ModelTag("Me.M3Dat");
        /// <summary>
        /// Tổng số mẻ đặt
        /// </summary>
        public ModelTag MeSoMe { get; private set; } = new ModelTag("Me.SoMe");
        /// <summary>
        /// m3/mẻ
        /// </summary>
        public ModelTag MeM3Me { get; private set; } = new ModelTag("Me.M3Me");
        #endregion

        #region Cân
        public ModelWeightTagGroup[] CanCLs { get; private set; } = new ModelWeightTagGroup[SoCLMax];
        public ModelWeightTagGroup[] CanXMs { get; private set; } = new ModelWeightTagGroup[SoCanXMMax];
        public ModelWeightTagGroup[] CanPGs { get; private set; } = new ModelWeightTagGroup[SoCanPGMax];

        public ModelWeightTagGroup CanNuoc { get; private set; } = new ModelWeightTagGroup("Water");
        #endregion

        #region Hệ thống
        public ModelTag SysRunning { get; private set; } = new ModelTag("Sys.Running");
        /// <summary>
        /// Bật tắt chế độ mô phỏng
        /// </summary>
        public ModelTag SysStartSimulation { get; private set; } = new ModelTag("Sys.Sim.Start");
        /// <summary>
        /// Báo đang ở chế độ mô phỏng (S7-200)
        /// </summary>
        public ModelTag SysSimulation { get; private set; } = new ModelTag("Sys.Sim");
        public ModelTag SysReset { get; private set; } = new ModelTag("Sys.Reset");

        public ModelTag SysWashMixer { get; private set; } = new ModelTag("Sys.WashMixer");
        public ModelTag NapNguyenLieu { get; private set; } = new ModelTag("Sys.NapNL");
        public ModelTag PCDangKetNoi { get; private set; } = new ModelTag("Sys.PC.KetNoi");
        #endregion

        #region Chế độ
        public ModelTag CheDoXa { get; private set; } = new ModelTag("Sys.Mode.Discharge");
        public ModelTag CheDoXaBeTong { get; private set; } = new ModelTag("Sys.Mode.DischargeConcrete");
        public ModelTag CheDoCan { get; private set; } = new ModelTag("Sys.Mode.Weight");
        public ModelTag CheDoSkip { get; private set; } = new ModelTag("Sys.Mode.Skip");
        #endregion

        #region Valves
        public ModelTag[] VanCLs { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] VanTinhCLs { get; private set; } = new ModelTag[SoCLMax];
        public ModelTag[] VanXMs { get; private set; } = new ModelTag[SoXMMax];
        public ModelTag[] VanPGs { get; private set; } = new ModelTag[SoPGMax];

        public ModelTag VanNuoc { get; private set; } = new ModelTag("Van.Nuoc");

        public ModelTag VanCLTG { get; private set; } = new ModelTag("Van.CLTG");

        public ModelTag SensorMoCLTG { get; private set; } = new ModelTag("Sensor.CLTG.Open");
        public ModelTag SensorDongCLTG { get; private set; } = new ModelTag("Sensor.CLTG.Close");
        #endregion

        #region Cối trộn
        public ModelTag MixerTGTron { get; private set; } = new ModelTag("CT.TGTron");
        public ModelTag MixerTGXa { get; private set; } = new ModelTag("CT.TGXa");
        public ModelTag MixerTGXaNua { get; private set; } = new ModelTag("CT.TGXaNua");
        /// <summary>
        /// Thời gian trộn đặt
        /// </summary>
        public ModelTag MixerSetTGTron { get; private set; } = new ModelTag("CT.Dat.TGTron");
        /// <summary>
        /// Thời gian xả nửa đặt
        /// </summary>
        public ModelTag MixerSetTGXaNua { get; private set; } = new ModelTag("CT.Dat.TGXaNua");
        /// <summary>
        /// Thời gian xả đặt
        /// </summary>
        public ModelTag MixerSetTGXa { get; private set; } = new ModelTag("CT.Dat.TGXa");


        /// <summary>
        /// Cảm biến đóng cối trộn
        /// </summary>
        public ModelTag SensorMixerClose { get; private set; } = new ModelTag("Sensor.CT.Close");
        /// <summary>
        /// Cảm biến mở cối trộn
        /// </summary>
        public ModelTag SensorMixerOpen { get; private set; } = new ModelTag("Sensor.CT.Open");
        public ModelTag SensorMixerOpenHalf { get; private set; } = new ModelTag("Sensor.CT.OpenHalf");
        public ModelTag MixerRunning { get; private set; } = new ModelTag("CT.DangChay");
        public ModelTag MixerDischarge { get; private set; } = new ModelTag("CT.DangXa");
        
        public ModelTag MixerStart { get; private set; } = new ModelTag("CT.BatDauTron");
        public ModelTag MixerCompleted { get; private set; } = new ModelTag("CT.TronXong");
        /// <summary>
        /// Tín hiệu đóng cửa xả bê tông
        /// </summary>
        public ModelTag MixerCloseDischarge { get; private set; } = new ModelTag("CT.DongCuaXa");

        public ModelTag MixerMeHt { get; private set; } = new ModelTag("CT.MeHT");
        #endregion

        #region Băng tải & Xe skip
        public ModelTag BangTaiNgang { get; private set; } = new ModelTag("BTNgang");
        public ModelTag BangTaiXien { get; private set; } = new ModelTag("BTXien");

        public ModelTag TGLenPheuCLTG { get; private set; } = new ModelTag("CLTG.TGLen");
        public ModelTag TGTreMoXaCLTG { get; private set; } = new ModelTag("CLTG.TreMoXa");

        public ModelTag XeSkipDT0 { get; private set; } = new ModelTag("XeSkip.DT0");
        public ModelTag XeSkipDT1 { get; private set; } = new ModelTag("XeSkip.DT1");
        public ModelTag XeSkipDT2 { get; private set; } = new ModelTag("XeSkip.DT2");

        public ModelTag XeSkipDown { get; private set; } = new ModelTag("XeSkip.Down");
        public ModelTag XeSkipUp { get; private set; } = new ModelTag("XeSkip.Up");
        public ModelTag XeSkipEMC { get; private set; } = new ModelTag("XeSkip.EMC");

        public ModelTag XeSkipTGDungDoCLDT2 { get; private set; } = new ModelTag("XeSkip.TGDungDoCL_DT2");
        public ModelTag XeSkipTGTrungCap { get; private set; } = new ModelTag("XeSkip.TGTrungCap");
        public ModelTag XeSkipDT0_DT2 { get; private set; } = new ModelTag("XeSkip.TG_DT0_DT2");
        public ModelTag XeSkipDT0_DT1 { get; private set; } = new ModelTag("XeSkip.TG_DT0_DT1");
        public ModelTag XeSkipTGXaCLDT2 { get; private set; } = new ModelTag("XeSkip.TGXaCL_DT2");
        #endregion

        #region Calibration
        public ModelTag CLCanAI { get; private set; } = new ModelTag("CanCL.AI");
        public ModelTag CLCanKL { get; private set; } = new ModelTag("CanCL.KL");
        public ModelTag CLCanZero { get; private set; } = new ModelTag("CanCL.Zero");
        public ModelTag CLCanSpan { get; private set; } = new ModelTag("CanCL.Span");

        public ModelTag XiCanAI { get; private set; } = new ModelTag("CanXi.AI");
        public ModelTag XiCanKL { get; private set; } = new ModelTag("CanXi.KL");
        public ModelTag XiCanZero { get; private set; } = new ModelTag("CanXi.Zero");
        public ModelTag XiCanSpan { get; private set; } = new ModelTag("CanXi.Span");

        public ModelTag NuocCanAI { get; private set; } = new ModelTag("CanNuoc.AI");
        public ModelTag NuocCanKL { get; private set; } = new ModelTag("CanNuoc.KL");
        public ModelTag NuocCanZero { get; private set; } = new ModelTag("CanNuoc.Zero");
        public ModelTag NuocCanSpan { get; private set; } = new ModelTag("CanNuoc.Span");
        #endregion

        #region Tham số
        public ModelTag EmptyLevelCL1 { get; private set; } = new ModelTag("Pr.EmptyLevel.CL1");
        public ModelTag EmptyLevelCL2 { get; private set; } = new ModelTag("Pr.EmptyLevel.CL2");
        public ModelTag EmptyLevelCL3 { get; private set; } = new ModelTag("Pr.EmptyLevel.CL3");
        public ModelTag EmptyLevelCL4 { get; private set; } = new ModelTag("Pr.EmptyLevel.CL4");
        public ModelTag EmptyLevelXM1 { get; private set; } = new ModelTag("Pr.EmptyLevel.XM1");
        public ModelTag EmptyLevelXM2 { get; private set; } = new ModelTag("Pr.EmptyLevel.XM2");
        public ModelTag EmptyLevelNuoc { get; private set; } = new ModelTag("Pr.EmptyLevel.Nuoc");
        public ModelTag EmptyLevelPG1 { get; private set; } = new ModelTag("Pr.EmptyLevel.PG1");

        public ModelTag CutOffLevelCL1 { get; private set; } = new ModelTag("Pr.CutOffLevel.CL1");
        public ModelTag CutOffLevelCL2 { get; private set; } = new ModelTag("Pr.CutOffLevel.CL2");
        public ModelTag CutOffLevelCL3 { get; private set; } = new ModelTag("Pr.CutOffLevel.CL3");
        public ModelTag CutOffLevelCL4 { get; private set; } = new ModelTag("Pr.CutOffLevel.CL4");
        public ModelTag CutOffLevelXM1 { get; private set; } = new ModelTag("Pr.CutOffLevel.XM1");
        public ModelTag CutOffLevelXM2 { get; private set; } = new ModelTag("Pr.CutOffLevel.XM2");
        public ModelTag CutOffLevelXM3 { get; private set; } = new ModelTag("Pr.CutOffLevel.XM3");
        public ModelTag CutOffLevelXM4 { get; private set; } = new ModelTag("Pr.CutOffLevel.XM4");
        public ModelTag CutOffLevelNuoc { get; private set; } = new ModelTag("Pr.CutOffLevel.Nuoc");
        public ModelTag CutOffLevelPG1 { get; private set; } = new ModelTag("Pr.CutOffLevel.PG1");
        public ModelTag CutOffLevelPG2 { get; private set; } = new ModelTag("Pr.CutOffLevel.PG2");

        public ModelTag EnableCutOffLevelCL1 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL1");
        public ModelTag EnableCutOffLevelCL2 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL2");
        public ModelTag EnableCutOffLevelCL3 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL3");
        public ModelTag EnableCutOffLevelCL4 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL4");
        public ModelTag EnableCutOffLevelCL5 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL5");
        public ModelTag EnableCutOffLevelCL6 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.CL6");
        public ModelTag EnableCutOffLevelXM1 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.XM1");
        public ModelTag EnableCutOffLevelXM2 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.XM2");
        public ModelTag EnableCutOffLevelXM3 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.XM3");
        public ModelTag EnableCutOffLevelXM4 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.XM4");
        public ModelTag EnableCutOffLevelNuoc { get; private set; } = new ModelTag("Pr.En.CutOffLevel.Nuoc");
        public ModelTag EnableCutOffLevelPG1 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.PG1");
        public ModelTag EnableCutOffLevelPG2 { get; private set; } = new ModelTag("Pr.En.CutOffLevel.PG2");

        public ModelTag CoarsFineCL1 { get; private set; } = new ModelTag("Pr.CoarsFine.CL1");
        public ModelTag CoarsFineCL2 { get; private set; } = new ModelTag("Pr.CoarsFine.CL2");
        public ModelTag CoarsFineCL3 { get; private set; } = new ModelTag("Pr.CoarsFine.CL3");
        public ModelTag CoarsFineCL4 { get; private set; } = new ModelTag("Pr.CoarsFine.CL4");
        public ModelTag CoarsFineXM1 { get; private set; } = new ModelTag("Pr.CoarsFine.XM1");
        public ModelTag CoarsFineXM2 { get; private set; } = new ModelTag("Pr.CoarsFine.XM2");
        public ModelTag CoarsFineXM3 { get; private set; } = new ModelTag("Pr.CoarsFine.XM3");
        public ModelTag CoarsFineXM4 { get; private set; } = new ModelTag("Pr.CoarsFine.XM4");
        public ModelTag CoarsFineNuoc { get; private set; } = new ModelTag("Pr.CoarsFine.Nuoc");
        public ModelTag CoarsFinePG1 { get; private set; } = new ModelTag("Pr.CoarsFine.PG1");
        public ModelTag CoarsFinePG2 { get; private set; } = new ModelTag("Pr.CoarsFine.PG2");

        public ModelTag PauseTimeCl1 { get; private set; } = new ModelTag("Pr.PauseTime.CL1");
        public ModelTag PauseTimeCl2 { get; private set; } = new ModelTag("Pr.PauseTime.CL2");
        public ModelTag PauseTimeCl3 { get; private set; } = new ModelTag("Pr.PauseTime.CL3");
        public ModelTag PauseTimeCl4 { get; private set; } = new ModelTag("Pr.PauseTime.CL4");
        public ModelTag PauseTimeXM1 { get; private set; } = new ModelTag("Pr.PauseTime.XM1");
        public ModelTag PauseTimeXM2 { get; private set; } = new ModelTag("Pr.PauseTime.XM2");
        public ModelTag PauseTimeNuoc { get; private set; } = new ModelTag("Pr.PauseTime.Nuoc");
        public ModelTag PauseTimePG1 { get; private set; } = new ModelTag("Pr.PauseTime.PG1");

        public ModelTag FineFactorCL1 { get; private set; } = new ModelTag("Pr.FineFactor.CL1");
        public ModelTag FineFactorCL2 { get; private set; } = new ModelTag("Pr.FineFactor.CL2");
        public ModelTag FineFactorCL3 { get; private set; } = new ModelTag("Pr.FineFactor.CL3");
        public ModelTag FineFactorCL4 { get; private set; } = new ModelTag("Pr.FineFactor.CL4");
        public ModelTag FineFactorXM1 { get; private set; } = new ModelTag("Pr.FineFactor.XM1");
        public ModelTag FineFactorXM2 { get; private set; } = new ModelTag("Pr.FineFactor.XM2");
        public ModelTag FineFactorNuoc { get; private set; } = new ModelTag("Pr.FineFactor.Nuoc");
        public ModelTag FineFactorPG1 { get; private set; } = new ModelTag("Pr.FineFactor.PG1");

        public ModelTag EnablePulseCL1 { get; private set; } = new ModelTag("Pr.EnPulse.CL1");
        public ModelTag EnablePulseCL2 { get; private set; } = new ModelTag("Pr.EnPulse.CL2");
        public ModelTag EnablePulseCL3 { get; private set; } = new ModelTag("Pr.EnPulse.CL3");
        public ModelTag EnablePulseCL4 { get; private set; } = new ModelTag("Pr.EnPulse.CL4");
        public ModelTag EnablePulseCL5 { get; private set; } = new ModelTag("Pr.EnPulse.CL5");
        public ModelTag EnablePulseCL6 { get; private set; } = new ModelTag("Pr.EnPulse.CL6");
        //public ModelTag ChonVitTinh { get; private set; } = new ModelTag();
        #endregion

        #region Tham số thời gian

        #region Trễ khởi động
        public ModelTag TreKhoiDongCL1 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL1");
        public ModelTag TreKhoiDongCL2 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL2");
        public ModelTag TreKhoiDongCL3 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL3");
        public ModelTag TreKhoiDongCL4 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL4");
        public ModelTag TreKhoiDongCL5 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL5");
        public ModelTag TreKhoiDongCL6 { get; private set; } = new ModelTag("Pr.TreKhoiDong.CL6");
        public ModelTag TreKhoiDongXM1 { get; private set; } = new ModelTag("Pr.TreKhoiDong.XM1");
        public ModelTag TreKhoiDongXM2 { get; private set; } = new ModelTag("Pr.TreKhoiDong.XM2");
        public ModelTag TreKhoiDongNuoc { get; private set; } = new ModelTag("Pr.TreKhoiDong.Nuoc");
        public ModelTag TreKhoiDongPG1 { get; private set; } = new ModelTag("Pr.TreKhoiDong.PG1");
        #endregion

        #region Trễ xả cân
        public ModelTag TreXaCanCL1 { get; private set; } = new ModelTag("Pr.TreXaCan.CL1");
        public ModelTag TreXaCanCL2 { get; private set; } = new ModelTag("Pr.TreXaCan.CL2");
        public ModelTag TreXaCanCL3 { get; private set; } = new ModelTag("Pr.TreXaCan.CL3");
        public ModelTag TreXaCanCL4 { get; private set; } = new ModelTag("Pr.TreXaCan.CL4");
        public ModelTag TreXaCanCL5 { get; private set; } = new ModelTag("Pr.TreXaCan.CL5");
        public ModelTag TreXaCanCL6 { get; private set; } = new ModelTag("Pr.TreXaCan.CL6");
        public ModelTag TreXaCanXM1 { get; private set; } = new ModelTag("Pr.TreXaCan.XM1");
        public ModelTag TreXaCanXM2 { get; private set; } = new ModelTag("Pr.TreXaCan.XM2");
        public ModelTag TreXaCanNuoc { get; private set; } = new ModelTag("Pr.TreXaCan.Nuoc");
        public ModelTag TreXaCanPG1 { get; private set; } = new ModelTag("Pr.TreXaCan.PG1");
        #endregion

        #region TG vào BTX
        public ModelTag TGVaoBTXCL1 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL1");
        public ModelTag TGVaoBTXCL2 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL2");
        public ModelTag TGVaoBTXCL3 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL3");
        public ModelTag TGVaoBTXCL4 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL4");
        public ModelTag TGVaoBTXCL5 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL5");
        public ModelTag TGVaoBTXCL6 { get; private set; } = new ModelTag("Pr.TGVaoBTX.CL6");
        #endregion

        #region Trễ đóng cửa xả
        public ModelTag TreDongCuaXaCL1 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL1");
        public ModelTag TreDongCuaXaCL2 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL2");
        public ModelTag TreDongCuaXaCL3 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL3");
        public ModelTag TreDongCuaXaCL4 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL4");
        public ModelTag TreDongCuaXaCL5 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL5");
        public ModelTag TreDongCuaXaCL6 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.CL6");
        public ModelTag TreDongCuaXaXM1 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.XM2");
        public ModelTag TreDongCuaXaXM2 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.XM1");
        public ModelTag TreDongCuaXaNuoc { get; private set; } = new ModelTag("Pr.TreDongCuaXa.Nuoc");
        public ModelTag TreDongCuaXaPG1 { get; private set; } = new ModelTag("Pr.TreDongCuaXa.PG1");
        #endregion

        #region TGChuTrinhXaCL
        public ModelTag TGChuTrinhXaCL1 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL1");
        public ModelTag TGChuTrinhXaCL2 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL2");
        public ModelTag TGChuTrinhXaCL3 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL3");
        public ModelTag TGChuTrinhXaCL4 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL4");
        public ModelTag TGChuTrinhXaCL5 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL5");
        public ModelTag TGChuTrinhXaCL6 { get; private set; } = new ModelTag("Pr.TGChuTrinhXa.CL6");
        #endregion

        #region TG Mở xả cốt liệu
        public ModelTag TGMoXaCL1 { get; private set; } = new ModelTag("Pr.TGMoXa.CL1");
        public ModelTag TGMoXaCL2 { get; private set; } = new ModelTag("Pr.TGMoXa.CL2");
        public ModelTag TGMoXaCL3 { get; private set; } = new ModelTag("Pr.TGMoXa.CL3");
        public ModelTag TGMoXaCL4 { get; private set; } = new ModelTag("Pr.TGMoXa.CL4");
        public ModelTag TGMoXaCL5 { get; private set; } = new ModelTag("Pr.TGMoXa.CL5");
        public ModelTag TGMoXaCL6 { get; private set; } = new ModelTag("Pr.TGMoXa.CL6");

        #endregion
        
        public ModelTag TGTreDungBTXMeCuoi { get; private set; } = new ModelTag("Pr.TGTreDungBTXMeCuoi");
        public ModelTag SetTGLenPheuCLTG { get; private set; } = new ModelTag("Pr.TGLenPheuCLTG");
        public ModelTag SetTGTreMoXaCLTG { get; private set; } = new ModelTag("Pr.TGMoThungCLTG");
        public ModelTag PrTGTronBeTong { get; private set; } = new ModelTag("Pr.TGTronBeTong");
        public ModelTag PrTGTreMoCoiTronHalf { get; private set; } = new ModelTag("Pr.TGTreMoCoiTronHalf");
        public ModelTag PrTGTreMoCoiTron { get; private set; } = new ModelTag("Pr.TGTreMoCoiTron");
        public ModelTag TGChuTrinhDamRung { get; private set; } = new ModelTag("Pr.DamRung.Cycle");
        public ModelTag TGBatDamRung { get; private set; } = new ModelTag("Pr.DamRung.Bat");
        #endregion

        #region Rung & sục khí
        public ModelTag SucKhiTimerOn { get; private set; } = new ModelTag("Pr.SucKhi.Bat");
        public ModelTag SucKhiTimerCycle { get; private set; } = new ModelTag("Pr.SucKhi.Cycle");

        public ModelTag RungCL1On { get; private set; } = new ModelTag("Pr.Rung.CL1.On");
        public ModelTag RungCL1Cycle { get; private set; } = new ModelTag("Pr.Rung.CL1.Cycle");
        public ModelTag RungCL2On { get; private set; } = new ModelTag("Pr.Rung.CL2.On");
        public ModelTag RungCL2Cycle { get; private set; } = new ModelTag("Pr.Rung.CL2.Cycle");
        public ModelTag RungCL3On { get; private set; } = new ModelTag("Pr.Rung.CL3.On");
        public ModelTag RungCL3Cycle { get; private set; } = new ModelTag("Pr.Rung.CL3.Cycle");
        public ModelTag RungCL4On { get; private set; } = new ModelTag("Pr.Rung.CL4.On");
        public ModelTag RungCL4Cycle { get; private set; } = new ModelTag("Pr.Rung.CL4.Cycle");
        public ModelTag RungCL5On { get; private set; } = new ModelTag("Pr.Rung.CL5.On");
        public ModelTag RungCL5Cycle { get; private set; } = new ModelTag("Pr.Rung.CL5.Cycle");
        public ModelTag RungCL6On { get; private set; } = new ModelTag("Pr.Rung.CL6.On");
        public ModelTag RungCL6Cycle { get; private set; } = new ModelTag("Pr.Rung.CL6.Cycle");

        public ModelTag RungTCXM1On { get; private set; } = new ModelTag("Pr.Rung.TCXM1.On");
        public ModelTag RungTCXM1Cycle { get; private set; } = new ModelTag("Pr.Rung.TCXM1.Cycle");
        public ModelTag RungTCXM2On { get; private set; } = new ModelTag("Pr.Rung.TCXM2.On");
        public ModelTag RungTCXM2Cycle { get; private set; } = new ModelTag("Pr.Rung.TCXM2.Cycle");

        public ModelTag RungCLTGTre { get; private set; } = new ModelTag("Pr.Rung.CLTG.Tre");
        #endregion

        #region KL0 rung xả cân
        public ModelTag KL0RungXaCanCL1 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL1");
        public ModelTag KL0RungXaCanCL2 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL2");
        public ModelTag KL0RungXaCanCL3 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL3");
        public ModelTag KL0RungXaCanCL4 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL4");
        public ModelTag KL0RungXaCanCL5 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL5");
        public ModelTag KL0RungXaCanCL6 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.CL6");
        public ModelTag KL0RungXaCanXM1 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.XM1");
        public ModelTag KL0RungXaCanXM2 { get; private set; } = new ModelTag("Pr.KL0RungXaCan.XM2");
        #endregion

        #region Bơm mỡ
        public ModelTag BomMoTGOn { get; private set; } = new ModelTag("Pr.BomMo.Rung.On");
        public ModelTag BomMoTGOff { get; private set; } = new ModelTag("Pr.BomMo.Rung.Off");
        #endregion

        #region Mức cân nháy
        public ModelTag MucCanNhayCL1 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL1");
        public ModelTag MucCanNhayCL2 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL2");
        public ModelTag MucCanNhayCL3 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL3");
        public ModelTag MucCanNhayCL4 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL4");
        public ModelTag MucCanNhayCL5 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL5");
        public ModelTag MucCanNhayCL6 { get; private set; } = new ModelTag("Pr.MucCanNhay.CL6");
        public ModelTag MucCanNhayXM1 { get; private set; } = new ModelTag("Pr.MucCanNhay.XM1");
        public ModelTag MucCanNhayXM2 { get; private set; } = new ModelTag("Pr.MucCanNhay.XM2");
        public ModelTag MucCanNhayNuoc { get; private set; } = new ModelTag("Pr.MucCanNhay.Nuoc");
        public ModelTag MucCanNhayPG1 { get; private set; } = new ModelTag("Pr.MucCanNhay.PG1");
        public ModelTag MucCanNhayPG2 { get; private set; } = new ModelTag("Pr.MucCanNhay.PG2");
        #endregion

        #endregion
        protected readonly Dictionary<string, TagLink> _allLinks = [];
        public Dictionary<string, TagLink> AllLinks { get { return _allLinks; } }

        public bool IsSim
        {
            get { if (ConnType == ConnectionTypes.S7Net) return SysStartSimulation.GetBool();
                else if (ConnType == ConnectionTypes.KEPServer) return SysSimulation.GetBool();
                else return false;
            }
        }

        #region S7-1200
        protected readonly TramTronS71200 _plc = new();
        #endregion

        #region KEPServer
        /// <summary>
        /// KEPServer: Channel.Device
        /// </summary>
        protected string _kSrvPath = "PPI.TBT01";
        protected readonly KEPServerCommunicator _ksrv = new();
        #endregion

        #region Communication
        private ConnectionTypes _openedConnectionType = ConnectionTypes.None;
        public ConnectionTypes ConnType { get; set; } = ConnectionTypes.None;

        private CommStates _commState0 = CommStates.None;
        public CommStates CommState { get { if (CanCommunicate) return _commState0; else return CommStates.None; } }
        public bool CanCommunicate { get; private set; }

        private DateTime _lastUpdateTag = DateTime.MinValue;
        private double _updateTagsCycle = 0.05;
        public double RealCycle { get { return _plc.RealCycleTime; } }

        public bool IsRunning { get { return _plc.IsRunning || _ksrv.Running; } }

        public DeviceConnectionInfo? ConnInfo
        {
            get
            {
                if (ConnType == ConnectionTypes.KEPServer) return _ksrv.ConnInfo;
                else return null;
            }
        }
        #endregion


        public virtual WeightIndicatorState WIState { get; protected set; } = new();

        public ModelHeThong() {
            #region Init array of tag
            for (int i = 0; i < SoCLMax; i++)
            {
                CLCapPhois[i] = new ModelTag($"CL.CP.{i}");
                ClAdds[i] = new ModelTag($"CL.Add.{i}");
                ClDoAms[i] = new ModelTag($"CL.DoAm.{i}");
                ClChots[i] = new ModelTag($"CL.Chot.{i}");
                CLCPMes[i] = new ModelTag($"CL.CP.Me.{i}");

                VanCLs[i] = new ModelTag($"CL.Van.{i}");
                VanTinhCLs[i] = new ModelTag($"CL.Van.Tinh.{i}");
            }
            for (int i = 0; i < SoXMMax; i++)
            {
                XMCapPhois[i] = new ModelTag($"XM.CP.{i}");
                XMAdds[i] = new ModelTag($"XM.Add.{i}");
                XMChots[i] = new ModelTag($"XM.Chot.{i}");
                XMCPMes[i] = new ModelTag($"XM.CP.Me.{i}");

                VanXMs[i] = new ModelTag($"XM.Van.{i}");
            }
            for (int i = 0; i < SoPGMax; i++)
            {
                PGCapPhois[i] = new ModelTag($"PG.CP.{i}");
                PGAdds[i] = new ModelTag($"PG.Add.{i}");
                PGChots[i] = new ModelTag($"PG.Chot.{i}");
                PGCPMes[i] = new ModelTag($"PG.CP.Me.{i}");

                VanPGs[i] = new ModelTag($"PG.Van.{i}");
            }

            for (int i = 0; i < SoCanCLMax; i++)
            {
                CanCLs[i] = new ModelWeightTagGroup($"CanCL{i}");
            }
            for (int i = 0; i < SoCanXMMax; i++)
            {
                CanXMs[i] = new ModelWeightTagGroup($"CanXM{i}");
            }
            for (int i = 0; i < SoCanPGMax; i++)
            {
                CanPGs[i] = new ModelWeightTagGroup($"CanPG{i}");
            }
            #endregion
        }
        public void MarkUpdateView()
        {
            foreach (var t in _allLinks.Values)
                t.Tag.IsChanged = false;
            _ksrv.ConnInfo.IsChanged = false;
        }

        /// <summary>
        /// Khởi tạo link giữa ModelTag và PlcTag (S7-1200), address (S7-200)
        /// và các tham số khác của ModelTag: Name, ValueType
        /// </summary>
        protected abstract void InitLinks();

        public void CreateKEPServerNodes()
        {
            if (_kSrvPath == null) return;

            List<string> tagnames = [];
            foreach (var (key, link) in _allLinks)
            {
                if (link.S7200Addr != null)
                    tagnames.Add(key);
            }
            _ksrv.CreateNodes(_kSrvPath, tagnames);
        }

        public async void StartConnection(bool instance = false)
        {
            try
            {
                if (ConnType == ConnectionTypes.S7Net)
                {
                    if (instance) _plc.DelayConnect = 0;
                    _plc.Start();
                    _openedConnectionType = ConnectionTypes.S7Net;
                }
                else if (ConnType == ConnectionTypes.KEPServer)
                {
                    if (await _ksrv.Start())
                    {
                        _ksrv.SkipRead = 0;
                        _openedConnectionType = ConnectionTypes.KEPServer;
                    }
                }
                CanCommunicate = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public async void StopConnection()
        {
            CanCommunicate = false;
            if (_openedConnectionType == ConnectionTypes.S7Net)
            {
                _plc.Stop();
            }
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
            {
                await _ksrv.Stop();
            }
            _openedConnectionType = ConnectionTypes.None;
            _commState0 = CommStates.None;
        }
        public bool CheckCommState()
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
            {
                if (_plc.State != _commState0)
                {
                    _commState0 = _plc.State;
                    if (CommState == CommStates.Opened)
                    {
                        // Bắt đầu có kết nối
                        _plc.ClearWriteCmds();       // Xóa hết cmd cũ đang có, tránh lỗi
                        _plc.Reset();
                    }
                    return true;
                }
            }
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
            {
                if (_ksrv.State != _commState0)
                {
                    _commState0 = _ksrv.State;
                    return true;
                }
            }
            return false;
        }

        public virtual void UpdateTags()
        {
            var i = DateTime.Now - _lastUpdateTag;
            if (i.TotalSeconds > _updateTagsCycle)
            {
                if (_openedConnectionType == ConnectionTypes.S7Net) UpdateTagsFromS7Net();
                else if (_openedConnectionType == ConnectionTypes.KEPServer)
                {
                    UpdateTagsFromKEPServer();
                }
                _lastUpdateTag = DateTime.Now;
            }
        }

        private void UpdateTagsFromS7Net()
        {
            foreach (var link in _allLinks.Values)
            {
                if (link.S71200Db != null && link.S71200Tag != null)
                {
                    link.Tag.SetValue(link.S71200Tag.Value, link.S71200Db.T);
                }
            }
        }
        private async void UpdateTagsFromKEPServer()
        {
            try
            {
                long t = DateTime.Now.Ticks;
                var values = await _ksrv.ReadTags();
                int nobad = 0;
                foreach (var (k, v) in values)
                {
                    if (v.IsGood)
                    {
                        if (_allLinks.ContainsKey(k))
                        {
                            var link = _allLinks[k];
                            if (k == "TGTronBeTong" || k == "TGTreMoCoiTron")
                                link.Tag.SetValue(v.Value / 10, t);
                            else link.Tag.SetValue(v.Value, t);

                        }
                    }
                    else nobad++;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                System.Diagnostics.Debug.WriteLine($"UpdateTagsFromKEPServer: {ex.Message}");
            }
        }

        public async void WriteTag(ModelTag tag, double v, double delay = 0)
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
            {
                tag.Link?.WriteToS71200(v, delay);
            }
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
            {
                if (tag.Name != null)
                {
                    await _ksrv.WriteTag(_kSrvPath, tag.Name, tag.ConvertValue(v), delay);
                }
            }
        }

        public string SetComm()
        {
            int commtype = _r.Settings.GetIntValue("plc.comm");
            var kSrvPath = _r.Settings.GetValue("plc.s7200.ksrv.path");
            _kSrvPath = string.IsNullOrWhiteSpace(kSrvPath) ? "PPI.TBT01" : kSrvPath;
            switch (commtype)
            {
                case 1:
                    ConnType = ConnectionTypes.S7Net;
                    return S71200_SetPLCAddr();
                case 2:
                    ConnType = ConnectionTypes.KEPServer;
                    return "Local";
            }
            return "None";
        }

        #region Write
        public void WriteCapPhoi(DHCongThucVM ct, bool datPG = true)
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
                S71200_WriteCapPhoi(ct, datPG);
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
                KEPServer_WriteCapPhoi(ct, datPG);
        }

        public virtual void WriteMixerTG(int tgtron, int tgxa, int tgxanua)
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
                S71200_WriteMixerTG(tgtron, tgxa, tgxanua);
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
                KEPServer_WriteMixerTG(tgtron, tgxa, tgxanua);
        }

        public void WriteSysReset()
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
                S71200_WriteReset();
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
                KEPServer_WriteReset();
        }

        public async void WriteSysSim(double v)
        {
            if (_openedConnectionType == ConnectionTypes.S7Net)
                WriteTag(SysStartSimulation, v);
            else if (_openedConnectionType == ConnectionTypes.KEPServer)
            {
                if (SysStartSimulation.Name != null)
                    await _ksrv.WriteTag(_kSrvPath, SysStartSimulation.Name, (float)v);
            }    
        }

        #region S7-1200
        public string S71200_SetPLCAddr()
        {

            string? plcaddr = _r.Settings.GetValue("plc.s71200.addr");
            plcaddr ??= "127.0.0.1:5102";
            _r.PlcAddr.FromStr(plcaddr);
            _plc.IPAddress = _r.PlcAddr.Addr;
            _plc.IPPort = _r.PlcAddr.Port;
            return _r.PlcAddr.ToString();
        }

        private void S71200_WriteCapPhoi(DHCongThucVM ct, bool datPG)
        {
            var db = _plc.Db43CP;
            WriteBytesCmd cmd = new();
            for (int i = 0; i < SoCLReal; i++)
            {
                var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.CotLieu && x.NL_SiloIndex == i);
                if (tp != null) cmd.AddTag(db.CLs[i], tp.KLCongThuc);
                else cmd.AddTag(db.CLs[i], 0);
            }
            for (int i = 0; i < SoXMReal; i++)
            {
                var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.XiMang && x.NL_SiloIndex == i);
                if (tp != null) cmd.AddTag(db.XMs[i], tp.KLCongThuc);
                else cmd.AddTag(db.XMs[i], 0);
            }
            if (datPG)
            {
                for (int i = 0; i < SoPGReal; i++)
                {
                    var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.PhuGia && x.NL_SiloIndex == i);
                    if (tp != null) cmd.AddTag(db.PGs[i], tp.KLCongThuc);
                    else cmd.AddTag(db.PGs[i], 0);
                }
            }
            else
            {
                for (int i = 0; i < SoPGReal; i++)
                {
                    cmd.AddTag(db.PGs[i], 0);
                }
            }
            cmd.AddTag(db.Water, ct.KLNuoc);
            db.AddWriteBytesCmd(cmd);
        }

        private void S71200_WriteM3(double m3dat)
        {
            var db6 = _plc.Db09MeDat;
            WriteBytesCmd cmd = new();
            cmd.AddTag(db6.M3Dat, m3dat);
            db6.AddWriteBytesCmd(cmd);
        }

        private void S71200_WriteSoMe(int some)
        {
            var db6 = _plc.Db09MeDat;
            WriteBytesCmd cmd = new();
            cmd.AddTag(db6.SoMeDat, some);
            db6.AddWriteBytesCmd(cmd);
        }

        private void S71200_WriteMixerTG(int tgtron, int tgxa, int tgxanua)
        {
            WriteBytesCmd cmd = new();
            cmd.AddTag(_plc.Db29SetMixerTime.TGTron, tgtron);
            cmd.AddTag(_plc.Db29SetMixerTime.TGXaNua, tgxanua);
            cmd.AddTag(_plc.Db29SetMixerTime.TGXa, tgxa);
            _plc.Db29SetMixerTime.AddWriteBytesCmd(cmd);
        }

        public void S71200_WritePCChoPhepCan()
        {
        }

        public void S71200_WriteStart(int v = 1)
        {
            WriteBytesCmd cmd = new() { DbType = S7.Net.DataType.Memory };
            cmd.AddTag(_plc.M1000.SysRunning, v);
            _plc.M1000.AddWriteBytesCmd(cmd);
        }

        private void S71200_WriteReset()
        {
            WriteBytesCmd cmd = new() { DbType = S7.Net.DataType.Memory };
            cmd.AddTag(_plc.M11.SysReset, 1);
            _plc.M11.AddWriteBytesCmd(cmd);

            cmd = new() { DbType = S7.Net.DataType.Memory, Delay = 0.5 };
            cmd.AddTag(_plc.M11.SysReset, 0);
            _plc.M11.AddWriteBytesCmd(cmd);
        }

        private void S71200_WriteSimulation(double v)
        {
            WriteBytesCmd cmd = new() { DbType = S7.Net.DataType.Memory };
            cmd.AddTag(_plc.M11.SysStartSim, v);
            _plc.M11.AddWriteBytesCmd(cmd);
        }

        public void S71200_WriteCLAdd(int i, double v)
        {
            WriteBytesCmd cmd = new();
            cmd.AddTag(_plc.Db43CP.CLAdds[i], v);
            _plc.Db43CP.AddWriteBytesCmd(cmd);
        }
        public void S71200_WriteCLHum(int i, double v)
        {
            WriteBytesCmd cmd = new();
            cmd.AddTag(_plc.Db43CP.CLHus[i], v);
            _plc.Db43CP.AddWriteBytesCmd(cmd);
        }
        public void S71200_WriteXMAdd(int i, double v)
        {
            WriteBytesCmd cmd = new();
            cmd.AddTag(_plc.Db43CP.XMAdds[i], v);
            _plc.Db43CP.AddWriteBytesCmd(cmd);
        }
        public void S71200_WriteNuocAdd(double v)
        {
            WriteBytesCmd cmd = new();
            cmd.AddTag(_plc.Db43CP.WaterAdd, v);
            _plc.Db43CP.AddWriteBytesCmd(cmd);
        }

        public void S71200_WriteMixerWash(double v)
        {
            WriteBytesCmd cmd = new() { DbType = S7.Net.DataType.Memory };
            cmd.AddTag(_plc.M11.SysWashMixer, v);
            _plc.M11.AddWriteBytesCmd(cmd);
        }

        /// <summary>
        /// Thay đổi số lượng tag đọc/ghi
        /// </summary>
        /// <param name="mode"></param>
        public abstract void EnableRead(int mode);
        #endregion

        #region KEPServer
        private Dictionary<string, object> _tagsToWrite = [];
#pragma warning disable CS8604 // Không bao giờ xay ra, lỗi VSStudio
        private void KEPServer_WriteCapPhoi(DHCongThucVM ct, bool datPG)
        {
            _tagsToWrite.Clear();

            for (int i = 0; i < SoCLReal; i++)
            {
                if (CLCapPhois[i].Name != null)
                {
                    var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.CotLieu && x.NL_SiloIndex == i);
                    if (tp != null)
                    {
                        _tagsToWrite.Add(CLCapPhois[i].Name, (float)tp.KLCongThuc);
                    }
                    else
                    {
                        _tagsToWrite.Add(CLCapPhois[i].Name, 0f);
                    }
                }
            }
            for (int i = 0; i < SoXMReal; i++)
            {
                if (!string.IsNullOrEmpty(XMCapPhois[i].Name))
                {
                    var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.XiMang && x.NL_SiloIndex == i);
                    if (tp != null)
                        _tagsToWrite.Add(XMCapPhois[i].Name, (float)tp.KLCongThuc);
                    else
                        _tagsToWrite.Add(XMCapPhois[i].Name, 0f);
                }
            }
            if (datPG)
            {
                for (int i = 0; i < SoPGReal; i++)
                {
                    if (PGCapPhois[i].Name != null)
                    {
                        var tp = ct.DsThanhPhan.FirstOrDefault(x => x.NL_PhanLoai == LoaiThanhPhan.PhuGia && x.NL_SiloIndex == i);
                        if (tp != null)
                            _tagsToWrite.Add(PGCapPhois[i].Name, (float)tp.KLCongThuc);
                        else
                            _tagsToWrite.Add(PGCapPhois[i].Name, 0f);
                    }
                }
            }
            else
            {
                for (int i = 0; i < SoPGReal; i++)
                {
                    _tagsToWrite.Add(PGCapPhois[i].Name, 0f);
                }
            }
            _tagsToWrite.Add(NuocCapPhoi.Name, (float)ct.KLNuoc);
            _ksrv.WriteTags(_kSrvPath, _tagsToWrite);
        }
        private void KEPServer_WriteMixerTG(int tgtron, int tgxa, int tgxanua)
        {
            _tagsToWrite.Clear();
            _tagsToWrite.Add(MixerSetTGTron.Name, MixerSetTGTron.ConvertValue(tgtron));
            _tagsToWrite.Add(MixerSetTGXa.Name, MixerSetTGXa.ConvertValue(tgxa));
            _tagsToWrite.Add(MixerSetTGXaNua.Name, MixerSetTGXaNua.ConvertValue(tgxanua));
            _ksrv.WriteTags(_kSrvPath, _tagsToWrite);
        }
#pragma warning restore CS8604 // Possible null reference argument.

        public async void KEPServer_WriteReset()
        {
            if (SysReset.Name != null)
            {
                await _ksrv.WriteTag(_kSrvPath, SysReset.Name, 1);

                await _ksrv.WriteTag(_kSrvPath, SysReset.Name, 0, 1);
            }
        }

        #endregion
        #endregion
    }
}
