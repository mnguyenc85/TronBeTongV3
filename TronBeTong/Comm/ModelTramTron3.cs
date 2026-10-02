using NMComm.S71200;

namespace TronBeTongV3.Comm
{
    /// <summary>
    /// Trạm trộn 60 m3; 3 cl, 2 xi, 1 pg; cân xe skip trực tiếp
    /// </summary>
    public class ModelTramTron3: ModelHeThong
    {
        public override WeightIndicatorState WIState { get; protected set; } = WeightIndicatorState.GetWIS_QuangBinh();

        public ModelTramTron3() : base()
        {
            SoCLReal = 3;
            SoXMReal = 2;
            SoPGReal = 1;

            InitLinks();
        }

        protected override void InitLinks()
        {            
            #region Cấp phối
            var db43 = _plc.Db43CP;
            for (int i = 0; i < SoCLReal; i++)
            {
                AddLink(CLCapPhois[i], db43, db43.CLs[i]);
                AddLink(ClAdds[i], db43, db43.CLAdds[i]);
                AddLink(ClDoAms[i], db43, db43.CLHus[i]);
                AddLink(ClChots[i], db43, db43.CLChots[i]);
            }

            for (int i = 0; i < SoXMReal; i++)
            {
                AddLink(XMCapPhois[i], db43, db43.XMs[i]);
                AddLink(XMAdds[i], db43, db43.XMAdds[i]);
                AddLink(XMChots[i], db43, db43.XMChots[i]);
            }

            for (int i = 0; i < SoPGReal; i++)
            {
                AddLink(PGCapPhois[i], db43, db43.PGs[i]);
                AddLink(PGAdds[i], db43, db43.PGAdds[i]);
                AddLink(PGChots[i], db43, db43.PGChots[i]);
            }

            var db43KLMe = _plc.Db43KLMe;
            AddLink(CLCPMes[0], db43KLMe, db43KLMe.CL1);
            AddLink(CLCPMes[1], db43KLMe, db43KLMe.CL2);
            AddLink(CLCPMes[2], db43KLMe, db43KLMe.CL3);

            AddLink(XMCPMes[0], db43KLMe, db43KLMe.XM1);
            AddLink(XMCPMes[1], db43KLMe, db43KLMe.XM2);
            AddLink(XMCPMes[2], db43KLMe, db43KLMe.XM3);
            AddLink(XMCPMes[3], db43KLMe, db43KLMe.XM4);

            AddLink(NuocCPMe, db43KLMe, db43KLMe.Nuoc);

            AddLink(PGCPMes[0], db43KLMe, db43KLMe.PG1);
            AddLink(PGCPMes[1], db43KLMe, db43KLMe.PG2);

            AddLink(NuocCapPhoi, db43, db43.Water);
            AddLink(NuocAdd, db43, db43.WaterAdd);
            AddLink(NuocChot, db43, db43.WaterClose);

            AddLink(MeM3Dat, _plc.Db09MeDat, _plc.Db09MeDat.M3Dat);
            AddLink(MeSoMe, _plc.Db09MeDat, _plc.Db09MeDat.SoMeDat);
            AddLink(MeM3Me, _plc.Db43CP, _plc.Db43CP.M3Me);
            #endregion

            var db26wi = _plc.Db26WIs;
            #region Cân
            for (int i = 0; i < SoCLReal; i++)
            {
                AddLink(CanCLs[i].TrangThai, db26wi, db26wi.CL_TT[i]);
                AddLink(CanCLs[i].KL, db26wi, db26wi.CL_KL[i]);
                AddLink(CanCLs[i].MeHT, db26wi, db26wi.CL_Me[i]);
            }
            AddLink(CanCLs[0].OutputValves, _plc.M200, _plc.M200.VanCanCL1);
            AddLink(CanCLs[1].OutputValves, _plc.M200, _plc.M200.VanCanCL2);
            AddLink(CanCLs[2].OutputValves, _plc.M200, _plc.M200.VanCanCL3);

            AddLink(CanXMs[0].TrangThai, db26wi, db26wi.XM_TT[0]);
            AddLink(CanXMs[0].KL, db26wi, db26wi.XM_KL[0]);
            AddLink(CanXMs[0].MeHT, db26wi, db26wi.XM_Me[0]);
            AddLink(CanXMs[0].OutputValves, _plc.M200, _plc.M200.VanCanXM1);

            AddLink(CanNuoc.TrangThai, db26wi, db26wi.Nuoc_TT[0]);
            AddLink(CanNuoc.KL, db26wi, db26wi.Nuoc_KL[0]);
            AddLink(CanNuoc.MeHT, db26wi, db26wi.Nuoc_Me[0]);
            AddLink(CanNuoc.OutputValves, _plc.M200, _plc.M200.VanCanNuoc);

            AddLink(CanPGs[0].TrangThai, db26wi, db26wi.PG_TT[0]);
            AddLink(CanPGs[0].KL, db26wi, db26wi.PG_KL[0]);
            AddLink(CanPGs[0].MeHT, db26wi, db26wi.PG_Me[0]);
            AddLink(CanPGs[0].OutputValves, _plc.M200, _plc.M200.VanCanPG);
            #endregion

            #region Van silo
            var m200 = _plc.M200;
            AddLink(VanCLs[0], m200, m200.VanCL1);
            AddLink(VanCLs[1], m200, m200.VanCL2);
            AddLink(VanCLs[2], m200, m200.VanCL3);
            AddLink(VanCLs[3], m200, m200.VanCL4);
            AddLink(VanTinhCLs[0], m200, m200.VanCL1Tinh);
            AddLink(VanTinhCLs[1], m200, m200.VanCL2Tinh);
            AddLink(VanTinhCLs[2], m200, m200.VanCL3Tinh);
            AddLink(VanTinhCLs[3], m200, m200.VanCL4Tinh);
            AddLink(VanXMs[0], m200, m200.VanXM1);
            AddLink(VanXMs[1], m200, m200.VanXM2);
            AddLink(VanXMs[2], m200, m200.VanXM3);
            AddLink(VanXMs[3], m200, m200.VanXM4);
            AddLink(VanNuoc, m200, m200.VanNuoc);
            AddLink(VanCLTG, m200, m200.VanCLTG);
            AddLink(VanPGs[0], m200, m200.VanPhuGia1);
            AddLink(VanPGs[1], m200, m200.VanPhuGia2);
            AddLink(MixerDischarge, m200, m200.MixerDischarge);

            var m100 = _plc.M100;
            AddLink(SensorMixerClose, m100, m100.SensorMixerClose);
            AddLink(SensorMixerOpen, m100, m100.SensorMixerOpen);
            AddLink(SensorMixerOpenHalf, m100, m100.SensorMixerOpenHalf);
            AddLink(MixerRunning, m100, m100.MixerRunning);
            #endregion

            #region Băng Tải & Xe Skip & CLTG

            //AddLink(XeSkipDT0, m100, m100.XeSkipDT0);
            //AddLink(XeSkipDT1, m100, m100.XeSkipDT1);
            //AddLink(XeSkipDT2, m100, m100.XeSkipDT2);
            //AddLink(XeSkipDown, m100, m100.XeSkipDown);
            //AddLink(XeSkipUp, m100, m100.XeSkipUp);
            //AddLink(XeSkipEMC, m100, m100.XeSkipEMC);

            //AddLink(XeSkipQuaTai, db26, db26.QuaTaiXeSkip);
            AddLink(SensorMoCLTG, m100, m100.SensorBaoMoCLTG);
            AddLink(SensorDongCLTG, m100, m100.SensorBaoDongCLTG);
            AddLink(BangTaiNgang, m200, m200.BangTaiNgang);
            AddLink(BangTaiXien, m200, m200.BangTaiXien);
            #endregion

            #region Hệ thống
            AddLink(SysRunning, _plc.M1000, _plc.M1000.SysRunning);
            
            AddLink(SysStartSimulation, _plc.M11, _plc.M11.SysStartSim);
            AddLink(SysReset, _plc.M11, _plc.M11.SysReset);
            AddLink(SysWashMixer, _plc.M11, _plc.M11.SysWashMixer);
            
            AddLink(CheDoXa, _plc.M100, _plc.M100.ModeDischarge);
            AddLink(CheDoXaBeTong, _plc.M100, _plc.M100.ModeDischargeConcrete);
            AddLink(CheDoCan, _plc.M100, _plc.M100.ModeWeight);
            AddLink(CheDoSkip, _plc.M100, _plc.M100.ModeSkip);
            #endregion


            #region Tham số thời gian
            var db29 = _plc.Db29ThamSo;
            AddLink(TreKhoiDongCL1, db29, db29.TreKhoiDongCL[0]);
            AddLink(TreKhoiDongCL2, db29, db29.TreKhoiDongCL[1]);
            AddLink(TreKhoiDongCL3, db29, db29.TreKhoiDongCL[2]);
            AddLink(TreKhoiDongXM1, db29, db29.TreKhoiDongXM[0]);
            AddLink(TreKhoiDongXM2, db29, db29.TreKhoiDongXM[1]);
            AddLink(TreKhoiDongNuoc, db29, db29.TreKhoiDongNuoc[0]);
            //AddLink(TreKhoiDongNuoc2, db29, db29.TreKhoiDongNuoc[0]);
            AddLink(TreKhoiDongPG1, db29, db29.TreKhoiDongPG[1]);
            //AddLink(TreKhoiDongPG2, db29, db29.TreKhoiDongPG[1]);

            AddLink(TreXaCanCL1, db29, db29.TreXaCanCL[0]);
            AddLink(TreXaCanCL2, db29, db29.TreXaCanCL[1]);
            AddLink(TreXaCanCL3, db29, db29.TreXaCanCL[2]);
            AddLink(TreXaCanXM1, db29, db29.TreXaCanXM[0]);
            AddLink(TreXaCanXM2, db29, db29.TreXaCanXM[1]);
            AddLink(TreXaCanNuoc, db29, db29.TreXaCanNuoc[0]);
            AddLink(TreXaCanPG1, db29, db29.TreXaCanPG[0]);

            AddLink(TreXaCL1XuongBT, db29, db29.TGTreXaCLXuongBangTai[0]);
            AddLink(TreXaCL2XuongBT, db29, db29.TGTreXaCLXuongBangTai[1]);
            AddLink(TreXaCL3XuongBT, db29, db29.TGTreXaCLXuongBangTai[2]);
            //AddLink(TreXaCL4XuongBT, db29, db29.TGTreXaCLXuongBangTai[3]);
            //AddLink(TreXaCL5XuongBT, db29, db29.TGTreXaCLXuongBangTai[5]);

            AddLink(TreDongCuaXaCL1, db29, db29.TreDongCuaXaCL[0]);
            AddLink(TreDongCuaXaCL2, db29, db29.TreDongCuaXaCL[1]);
            AddLink(TreDongCuaXaCL3, db29, db29.TreDongCuaXaCL[2]);
            AddLink(TreDongCuaXaXM1, db29, db29.TreDongCuaXaXM[0]);
            AddLink(TreDongCuaXaXM2, db29, db29.TreDongCuaXaXM[1]);
            AddLink(TreDongCuaXaNuoc, db29, db29.TreDongCuaXaNuoc[0]);
            AddLink(TreDongCuaXaPG1, db29, db29.TreDongCuaXaPG[0]);

            AddLink(TGChuTrinhXaCL1, db29, db29.TGDongNhayCL[0]);
            AddLink(TGChuTrinhXaCL2, db29, db29.TGDongNhayCL[1]);
            AddLink(TGChuTrinhXaCL3, db29, db29.TGDongNhayCL[2]);
            AddLink(TGMoXaCL1, db29, db29.TGMoNhayCL[0]);
            AddLink(TGMoXaCL2, db29, db29.TGMoNhayCL[1]);
            AddLink(TGMoXaCL3, db29, db29.TGMoNhayCL[2]);

            AddLink(TGTreDungBTXMeCuoi, db29, db29.TGTreDungBTXMeCuoi);
            AddLink(SetTGLenPheuCLTG, db29, db29.TGCLDiQuaBTX);
            AddLink(SetTGTreMoXaCLTG, db29, db29.TGMoThungCLTG);
            AddLink(TGChuTrinhDamRung, db29, db29.TGChuTrinhDamRung);
            AddLink(TGBatDamRung, db29, db29.TGBatDamRung);

            AddLink(XeSkipTGDungDoCLDT2, db29, db29.TGDungDoCLDT2);
            AddLink(XeSkipTGTrungCap, db29, db29.TGTrungCap);
            AddLink(XeSkipDT0_DT2, db29, db29.TGXeSkipDT0_DT2);
            AddLink(XeSkipDT0_DT1, db29, db29.TGXeSkipDT0_DT1);
            #endregion

            #region Rung và sục khí
            AddLink(SucKhiTimerOn, db29, db29.SucKhiTimerOn[0]);
            AddLink(SucKhiTimerCycle, db29, db29.SucKhiTimerOff[0]);

            AddLink(RungCL1On, db29, db29.RungCLOn[0]);
            AddLink(RungCL1Cycle, db29, db29.RungCLCycle[0]);
            AddLink(RungCL2On, db29, db29.RungCLOn[1]);
            AddLink(RungCL2Cycle, db29, db29.RungCLCycle[1]);
            AddLink(RungCL3On, db29, db29.RungCLOn[2]);
            AddLink(RungCL3Cycle, db29, db29.RungCLCycle[2]);
            AddLink(RungTCXM1On, db29, db29.RungTCXMOn[0]);
            AddLink(RungTCXM1Cycle, db29, db29.RungTCXMCycle);
            AddLink(RungTCXM2On, db29, db29.RungTCXMOn[0]);
            //AddLink(RungTCXM2Cycle, db29, db29.RungTCXMCycle);            // Dùng chung
            // TODO: -> sửa giao diện
            
            AddLink(RungCLTGTre, db29, db29.RungCLTGTre);
            #endregion

            #region Bơm mỡ
            AddLink(BomMoTGOn, db29, db29.BomMoTGOn);
            AddLink(BomMoTGOff, db29, db29.BomMoTGOff);
            #endregion


            var db29HMI = _plc.Db29HMI;
            AddLink(MixerTGTron, db29HMI, db29HMI.TGTronBeTong_ET);
            AddLink(MixerTGXa, db29HMI, db29HMI.TGTreMoCoiTron_ET);
            AddLink(MixerTGXaNua, db29HMI, db29HMI.TGTreMoCoiTronHalf_ET);
            AddLink(TGLenPheuCLTG, db29HMI, db29HMI.TGCotLieuLenCLTG_ET);
            AddLink(TGTreMoXaCLTG, db29HMI, db29HMI.TGMoXaCLTG_ET);
            AddLink(MixerSetTGTron, db29HMI, db29HMI.TGTronBeTong);
            AddLink(MixerSetTGXa, db29HMI, db29HMI.TGTreMoCoiTron);
            AddLink(MixerSetTGXaNua, db29HMI, db29HMI.TGTreMoCoiTronHalf);

            #region Db26
            var db26 = _plc.Db26ThamSo;
            var db26o = _plc.Db26Khac;
            #region Tham số
            AddLink(EmptyLevelCL1, db26, db26.EmptyLevelCLs[0]);
            AddLink(EmptyLevelCL2, db26, db26.EmptyLevelCLs[1]);
            AddLink(EmptyLevelCL3, db26, db26.EmptyLevelCLs[2]);
            //AddLink(EmptyLevelCL4, db26, db26.EmptyLevelCLs[3]);
            AddLink(EmptyLevelXM1, db26, db26.EmptyLevelXMs[0]);
            AddLink(EmptyLevelXM2, db26, db26.EmptyLevelXMs[1]);
            AddLink(EmptyLevelNuoc, db26, db26.EmptyLevelNuoc[0]);
            AddLink(EmptyLevelPG1, db26, db26.EmptyLevelPGs[0]);
            //AddLink(EmptyLevelPG2, db26, db26.EmptyLevelPGs[1]);

            AddLink(CutOffLevelCL1, db26, db26.CutOffLevelCLs[0]);
            AddLink(CutOffLevelCL2, db26, db26.CutOffLevelCLs[1]);
            AddLink(CutOffLevelCL3, db26, db26.CutOffLevelCLs[2]);
            //AddLink(CutOffLevelCL4, db26, db26.CutOffLevelCLs[3]);
            AddLink(CutOffLevelXM1, db26, db26.CutOffLevelXMs[0]);
            AddLink(CutOffLevelXM2, db26, db26.CutOffLevelXMs[1]);
            AddLink(CutOffLevelXM3, db26, db26.CutOffLevelXMs[2]);
            AddLink(CutOffLevelXM4, db26, db26.CutOffLevelXMs[3]);
            AddLink(CutOffLevelNuoc, db26, db26.CutOffLevelNuoc[0]);
            AddLink(CutOffLevelPG1, db26, db26.CutOffLevelPGs[0]);
            //AddLink(CutOffLevelPG2, db26, db26.CutOffLevelPGs[1]);

            AddLink(EnableCutOffLevelCL1, db26, db26.EnableAutoCutOffCLs[0]);
            AddLink(EnableCutOffLevelCL2, db26, db26.EnableAutoCutOffCLs[1]);
            AddLink(EnableCutOffLevelCL3, db26, db26.EnableAutoCutOffCLs[2]);
            //AddLink(EnableCutOffLevelCL4, db26, db26.EnableAutoCutOffCLs[3]);
            AddLink(EnableCutOffLevelXM1, db26, db26.EnableAutoCutOffXMs[0]);
            AddLink(EnableCutOffLevelXM2, db26, db26.EnableAutoCutOffXMs[1]);
            AddLink(EnableCutOffLevelXM3, db26, db26.EnableAutoCutOffXMs[2]);
            AddLink(EnableCutOffLevelXM4, db26, db26.EnableAutoCutOffXMs[3]);
            AddLink(EnableCutOffLevelNuoc, db26, db26.EnableAutoCutOffNuoc[0]);
            AddLink(EnableCutOffLevelPG1, db26, db26.EnableAutoCutOffPGs[0]);
            //AddLink(EnableCutOffLevelPG2, db26, db26.EnableAutoCutOffPGs[1]);

            AddLink(CoarsFineCL1, db26, db26.CoarsFineCLs[0]);
            AddLink(CoarsFineCL2, db26, db26.CoarsFineCLs[1]);
            AddLink(CoarsFineCL3, db26, db26.CoarsFineCLs[2]);
            //AddLink(CoarsFineCL4, db26, db26.CoarsFineCLs[3]);
            AddLink(CoarsFineXM1, db26, db26.CoarsFineXMs[0]);
            AddLink(CoarsFineXM2, db26, db26.CoarsFineXMs[1]);
            AddLink(CoarsFineXM3, db26, db26.CoarsFineXMs[2]);
            AddLink(CoarsFineXM4, db26, db26.CoarsFineXMs[3]);
            AddLink(CoarsFineNuoc, db26, db26.CoarsFineNuoc[0]);
            AddLink(CoarsFinePG1, db26, db26.CoarsFinePGs[0]);
            AddLink(CoarsFinePG2, db26, db26.CoarsFinePGs[1]);

            AddLink(PauseTimeCl1, db26, db26.PauseTimeCLs[0]);
            AddLink(PauseTimeCl2, db26, db26.PauseTimeCLs[1]);
            AddLink(PauseTimeCl3, db26, db26.PauseTimeCLs[2]);
            //AddLink(PauseTimeCl4, db26, db26.PauseTimeCLs[3]);
            AddLink(PauseTimeXM1, db26, db26.PauseTimeXMs[0]);
            AddLink(PauseTimeXM2, db26, db26.PauseTimeXMs[1]);
            AddLink(PauseTimeNuoc, db26, db26.PauseTimeNuoc[0]);
            AddLink(PauseTimePG1, db26, db26.PauseTimePGs[0]);
            //AddLink(PauseTimePG2, db26, db26.PauseTimePGs[1]);

            AddLink(FineFactorCL1, db26, db26.FineFactorCLs[0]);
            AddLink(FineFactorCL2, db26, db26.FineFactorCLs[1]);
            AddLink(FineFactorCL3, db26, db26.FineFactorCLs[2]);
            AddLink(FineFactorCL4, db26, db26.FineFactorCLs[3]);
            AddLink(FineFactorXM1, db26, db26.FineFactorXMs[0]);
            AddLink(FineFactorXM2, db26, db26.FineFactorXMs[1]);
            AddLink(FineFactorNuoc, db26, db26.FineFactorNuoc[0]);
            AddLink(FineFactorPG1, db26, db26.FineFactorPGs[0]);
            //AddLink(FineFactorPG2, db26, db26.FineFactorPGs[1]);

            AddLink(EnablePulseCL1, db26, db26.EnablePulseCLs[0]);
            AddLink(EnablePulseCL2, db26, db26.EnablePulseCLs[1]);
            AddLink(EnablePulseCL3, db26, db26.EnablePulseCLs[2]);
            //AddLink(EnablePulseCL4, db26, db26.EnablePulseCLs[3]);
            //AddLink(EnablePulseCL5, db26, db26.EnablePulseCLs[4]);

            #region Mức cân nháy
            AddLink(MucCanNhayCL1, db26, db26.MucCanNhayCLs[0]);
            AddLink(MucCanNhayCL2, db26, db26.MucCanNhayCLs[1]);
            AddLink(MucCanNhayCL3, db26, db26.MucCanNhayCLs[2]);
            //AddLink(MucCanNhayCL4, db26, db26.MucCanNhayCLs[3]);
            //AddLink(MucCanNhayCL5, db26, db26.MucCanNhayCLs[4]);
            #endregion

            #region KL0 rung xả cân
            AddLink(KL0RungXaCanCL1, db26, db26.KL0RungXaCanCLs[0]);
            AddLink(KL0RungXaCanCL2, db26, db26.KL0RungXaCanCLs[1]);
            AddLink(KL0RungXaCanCL3, db26, db26.KL0RungXaCanCLs[2]);
            //AddLink(KL0RungXaCanCL4, db26, db26.KL0RungXaCanCLs[3]);
            //AddLink(KL0RungXaCanCL5, db26, db26.KL0RungXaCanCLs[4]);
            AddLink(KL0RungXaCanXM1, db26, db26.KL0RungXaCanXMs[0]);
            AddLink(KL0RungXaCanXM2, db26, db26.KL0RungXaCanXMs[1]);
            #endregion

            #region Rung & sục khí
            AddLink(EnableSucKhiSilo1, db26, db26.EnableSucKhiSiloes[0]);
            AddLink(EnableSucKhiSilo2, db26, db26.EnableSucKhiSiloes[1]);
            AddLink(EnableSucKhiSilo3, db26, db26.EnableSucKhiSiloes[2]);
            AddLink(EnableSucKhiSilo4, db26, db26.EnableSucKhiSiloes[3]);
            AddLink(EnableRungTP1, db26, db26.EnableDamRungCLs[0]);
            AddLink(EnableRungTP2, db26, db26.EnableDamRungCLs[1]);
            AddLink(EnableRungTP3, db26, db26.EnableDamRungCLs[2]);
            #endregion
            #endregion
            #region Hiệu chuẩn
            AddLink(CL1CanAI, db26, db26.CalibCLAIs[0]);
            AddLink(CL1CanZero, db26, db26.CalibCLZeroes[0]);
            AddLink(CL1CanSpan, db26, db26.CalibCLSpans[0]);
            AddLink(CL2CanAI, db26, db26.CalibCLAIs[1]);
            AddLink(CL2CanZero, db26, db26.CalibCLZeroes[1]);
            AddLink(CL2CanSpan, db26, db26.CalibCLSpans[1]);
            AddLink(CL3CanAI, db26, db26.CalibCLAIs[2]);
            AddLink(CL3CanZero, db26, db26.CalibCLZeroes[2]);
            AddLink(CL3CanSpan, db26, db26.CalibCLSpans[2]);

            AddLink(XiCanAI, db26, db26.CalibXMAIs[0]);
            AddLink(XiCanZero, db26, db26.CalibXMZeroes[0]);
            AddLink(XiCanSpan, db26, db26.CalibXMSpans[0]);

            AddLink(PGCanAI, db26, db26.CalibPGAIs[0]);
            AddLink(PGCanZero, db26, db26.CalibPGZeroes[0]);
            AddLink(PGCanSpan, db26, db26.CalibPGSpans[0]);

            AddLink(NuocCanAI, db26, db26.CalibNuocAIs[0]);
            AddLink(NuocCanZero, db26, db26.CalibNuocZeroes[0]);
            AddLink(NuocCanSpan, db26, db26.CalibNuocSpans[0]);
            #endregion
            
            #region Khác
            //AddLink(MixerTGTron, db26o, db26o.TGTron);
            //AddLink(MixerTGXa, db26o, db26o.TGXa);
            //AddLink(MixerTGXaNua, db26o, db26o.TGXaNua);
            AddLink(MixerMeHt, db26o, db26o.CoiTronMeHt);
            AddLink(XeSkipTGXaCLDT2, db26o, db26o.TGXaCLDT2);
            AddLink(XM1VitTinh, db26o, db26o.XM1VitTinh);
            AddLink(WaterKeep, db26o, db26o.WaterKeep);
            #endregion
            
            #endregion

            foreach (var (key, link) in _allLinks)
            {
                link.Tag.Name = key;
                if (link.S71200Tag != null) link.Tag.ConvertType(link.S71200Tag.TagType);
                link.Tag.Link = link;
            }
        }

        /// <summary>
        /// Bật/tắt việc đọc 1 số Db để tăng tốc độ kết nối
        /// </summary>
        /// <param name="mode"></param>
        public override void EnableRead(int mode)
        {
            if (mode == 0) S71200_ReadDbParams(false);
            else if (mode == 1) S71200_ReadDbParams(true);
        }

        /// <summary>
        /// Cho phép đọc Db22Params & Db28Params hoặc Db28
        /// </summary>
        /// <param name="en"></param>
        private void S71200_ReadDbParams(bool en)
        {
            if (en)
            {
                _plc.Db26ThamSo.Cycle = 0.2;

                _plc.Db29HMI.Cycle = -1;
                _plc.Db29ThamSo.Cycle = 0.2;
            }
            else
            {
                _plc.Db26ThamSo.Cycle = -1;
                _plc.Db26ThamSo.ForceRead = true;
                
                _plc.Db29HMI.Cycle = 0.2;
                _plc.Db29ThamSo.Cycle = -1;
                _plc.Db29ThamSo.ForceRead = true;
            }
        }

        private void AddLink(ModelTag mtag, PlcDb db, PlcTag tag)
        {
            _allLinks.Add(mtag.Name, new TagLink(mtag, db, tag));
        }
    }
}
