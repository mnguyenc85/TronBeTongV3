using NMComm.S71200;
using NMWPFControls.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TronBeTongV3.Comm;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndDatThamSo.xaml
    /// </summary>
    public partial class WndDatThamSo : Window, INotifyPropertyChanged
    {
        private DispatcherTimer _tmr;
        public ModelHeThong? TramTron { get; set; }

        private List<TagTextBox> _textboxes = [];
        private List<TagCheckbox> _checkboxes = [];

        private double _appZoom = 1;
        public double AppZoom { get { return _appZoom; } set { if (_appZoom != value) { _appZoom = value; NotifyChanged(); } } }

        public WndDatThamSo()
        {
            InitializeComponent();
            DataContext = this;

            _tmr = new DispatcherTimer()
            {
                Interval = new TimeSpan(500 * 10000),                
            };
            _tmr.Tick += _tmr_Tick;

            double appZoom = DbRepository.Instance.Settings.GetDoubleValue("app.zoom", 1);
            AppZoom = appZoom;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TramTron != null)
            {
                // Empty levels
                _textboxes.Add(new TagTextBox(TxtZeroLvlCL1, TramTron, TramTron.EmptyLevelCL1));
                _textboxes.Add(new TagTextBox(TxtZeroLvlCL2, TramTron, TramTron.EmptyLevelCL2));
                _textboxes.Add(new TagTextBox(TxtZeroLvlCL3, TramTron, TramTron.EmptyLevelCL3));
                _textboxes.Add(new TagTextBox(TxtZeroLvlCL4, TramTron, TramTron.EmptyLevelCL4));
                _textboxes.Add(new TagTextBox(TxtZeroLvlXM1, TramTron, TramTron.EmptyLevelXM1));
                _textboxes.Add(new TagTextBox(TxtZeroLvlXM2, TramTron, TramTron.EmptyLevelXM2));
                _textboxes.Add(new TagTextBox(TxtZeroLvlNuoc, TramTron, TramTron.EmptyLevelNuoc));
                _textboxes.Add(new TagTextBox(TxtZeroLvlPG1, TramTron, TramTron.EmptyLevelPG1));

                // Cutoff level
                _textboxes.Add(new TagTextBox(TxtCutOffLvlCL1, TramTron, TramTron.CutOffLevelCL1));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlCL2, TramTron, TramTron.CutOffLevelCL2));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlCL3, TramTron, TramTron.CutOffLevelCL3));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlCL4, TramTron, TramTron.CutOffLevelCL4));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlXM1, TramTron, TramTron.CutOffLevelXM1));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlXM2, TramTron, TramTron.CutOffLevelXM2));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlXM3, TramTron, TramTron.CutOffLevelXM3));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlXM4, TramTron, TramTron.CutOffLevelXM4));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlNuoc, TramTron, TramTron.CutOffLevelNuoc));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlPG1, TramTron, TramTron.CutOffLevelPG1));
                _textboxes.Add(new TagTextBox(TxtCutOffLvlPG2, TramTron, TramTron.CutOffLevelPG2));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelCL1, TramTron, TramTron.EnableCutOffLevelCL1));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelCL2, TramTron, TramTron.EnableCutOffLevelCL2));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelCL3, TramTron, TramTron.EnableCutOffLevelCL3));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelCL4, TramTron, TramTron.EnableCutOffLevelCL4));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelXM1, TramTron, TramTron.EnableCutOffLevelXM1));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelXM2, TramTron, TramTron.EnableCutOffLevelXM2));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelXM3, TramTron, TramTron.EnableCutOffLevelXM3));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelXM4, TramTron, TramTron.EnableCutOffLevelXM4));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelNuoc, TramTron, TramTron.EnableCutOffLevelNuoc));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelPG1, TramTron, TramTron.EnableCutOffLevelPG1));
                _checkboxes.Add(new TagCheckbox(ChkAutoCutOffLevelPG2, TramTron, TramTron.EnableCutOffLevelPG2));

                // Coarsfine
                _textboxes.Add(new TagTextBox(TxtCoarsfineCL1, TramTron, TramTron.CoarsFineCL1));
                _textboxes.Add(new TagTextBox(TxtCoarsfineCL2, TramTron, TramTron.CoarsFineCL2));
                _textboxes.Add(new TagTextBox(TxtCoarsfineCL3, TramTron, TramTron.CoarsFineCL3));
                _textboxes.Add(new TagTextBox(TxtCoarsfineCL4, TramTron, TramTron.CoarsFineCL4));
                _textboxes.Add(new TagTextBox(TxtCoarsfineXM1, TramTron, TramTron.CoarsFineXM1));
                _textboxes.Add(new TagTextBox(TxtCoarsfineXM2, TramTron, TramTron.CoarsFineXM2));
                _textboxes.Add(new TagTextBox(TxtCoarsfineXM3, TramTron, TramTron.CoarsFineXM3));
                _textboxes.Add(new TagTextBox(TxtCoarsfineXM4, TramTron, TramTron.CoarsFineXM4));
                _textboxes.Add(new TagTextBox(TxtCoarsfineNuoc, TramTron, TramTron.CoarsFineNuoc));
                _textboxes.Add(new TagTextBox(TxtCoarsfinePG1, TramTron, TramTron.CoarsFinePG1));

                // Paused time
                _textboxes.Add(new TagTextBox(TxtPausedTimeCL1, TramTron, TramTron.PauseTimeCl1));
                _textboxes.Add(new TagTextBox(TxtPausedTimeCL2, TramTron, TramTron.PauseTimeCl2));
                _textboxes.Add(new TagTextBox(TxtPausedTimeCL3, TramTron, TramTron.PauseTimeCl3));
                _textboxes.Add(new TagTextBox(TxtPausedTimeCL4, TramTron, TramTron.PauseTimeCl4));
                _textboxes.Add(new TagTextBox(TxtPausedTimeXM1, TramTron, TramTron.PauseTimeXM1));
                _textboxes.Add(new TagTextBox(TxtPausedTimeXM2, TramTron, TramTron.PauseTimeXM2));
                _textboxes.Add(new TagTextBox(TxtPausedTimeNuoc, TramTron, TramTron.PauseTimeNuoc));
                _textboxes.Add(new TagTextBox(TxtPausedTimePG1, TramTron, TramTron.PauseTimePG1));

                // Fine factor
                _textboxes.Add(new TagTextBox(TxtFineFactorCL1, TramTron, TramTron.FineFactorCL1));
                _textboxes.Add(new TagTextBox(TxtFineFactorCL2, TramTron, TramTron.FineFactorCL2));
                _textboxes.Add(new TagTextBox(TxtFineFactorCL3, TramTron, TramTron.FineFactorCL3));
                _textboxes.Add(new TagTextBox(TxtFineFactorCL4, TramTron, TramTron.FineFactorCL4));
                _textboxes.Add(new TagTextBox(TxtFineFactorXM1, TramTron, TramTron.FineFactorXM1));
                _textboxes.Add(new TagTextBox(TxtFineFactorXM2, TramTron, TramTron.FineFactorXM2));
                _textboxes.Add(new TagTextBox(TxtFineFactorNuoc, TramTron, TramTron.FineFactorNuoc));
                _textboxes.Add(new TagTextBox(TxtFineFactorPG1, TramTron, TramTron.FineFactorPG1));

                // Mức cân nháy
                _textboxes.Add(new TagTextBox(TxtMucCanNhayCL1, TramTron, TramTron.MucCanNhayCL1));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayCL2, TramTron, TramTron.MucCanNhayCL2));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayCL3, TramTron, TramTron.MucCanNhayCL3));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayCL4, TramTron, TramTron.MucCanNhayCL4));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayXM1, TramTron, TramTron.MucCanNhayXM1));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayXM2, TramTron, TramTron.MucCanNhayXM2));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayNuoc, TramTron, TramTron.MucCanNhayNuoc));
                _textboxes.Add(new TagTextBox(TxtMucCanNhayPG1, TramTron, TramTron.MucCanNhayPG1));

                //_checkboxes.Add(new TagCheckbox(ChkVitTinh, TramTron, TramTron.ChonVitTinh));
                _checkboxes.Add(new TagCheckbox(ChkEnPulseCL1, TramTron, TramTron.EnablePulseCL1));
                _checkboxes.Add(new TagCheckbox(ChkEnPulseCL2, TramTron, TramTron.EnablePulseCL2));
                _checkboxes.Add(new TagCheckbox(ChkEnPulseCL3, TramTron, TramTron.EnablePulseCL3));
                _checkboxes.Add(new TagCheckbox(ChkEnPulseCL4, TramTron, TramTron.EnablePulseCL4));

                _textboxes.Add(new TagTextBox(TxtKL0RungCanCL1, TramTron, TramTron.KL0RungXaCanCL1));
                _textboxes.Add(new TagTextBox(TxtKL0RungCanCL2, TramTron, TramTron.KL0RungXaCanCL2));
                _textboxes.Add(new TagTextBox(TxtKL0RungCanCL3, TramTron, TramTron.KL0RungXaCanCL3));
                _textboxes.Add(new TagTextBox(TxtKL0RungCanXM1, TramTron, TramTron.KL0RungXaCanXM1));
                _textboxes.Add(new TagTextBox(TxtKL0RungCanXM2, TramTron, TramTron.KL0RungXaCanXM2));

                #region Tham số thời gian
                _textboxes.Add(new TagTextBox(TxtTreKDCL1, TramTron, TramTron.TreKhoiDongCL1));
                _textboxes.Add(new TagTextBox(TxtTreKDCL2, TramTron, TramTron.TreKhoiDongCL2));
                _textboxes.Add(new TagTextBox(TxtTreKDCL3, TramTron, TramTron.TreKhoiDongCL3));
                _textboxes.Add(new TagTextBox(TxtTreKDCL4, TramTron, TramTron.TreKhoiDongCL4));
                _textboxes.Add(new TagTextBox(TxtTreKDXM1, TramTron, TramTron.TreKhoiDongXM1));
                _textboxes.Add(new TagTextBox(TxtTreKDXM2, TramTron, TramTron.TreKhoiDongXM2));
                _textboxes.Add(new TagTextBox(TxtTreKDNuoc, TramTron, TramTron.TreKhoiDongNuoc));
                _textboxes.Add(new TagTextBox(TxtTreKDPG1, TramTron, TramTron.TreKhoiDongPG1));

                _textboxes.Add(new TagTextBox(TxtTreXaCanCL1, TramTron, TramTron.TreXaCanCL1));
                _textboxes.Add(new TagTextBox(TxtTreXaCanCL2, TramTron, TramTron.TreXaCanCL2));
                _textboxes.Add(new TagTextBox(TxtTreXaCanCL3, TramTron, TramTron.TreXaCanCL3));
                _textboxes.Add(new TagTextBox(TxtTreXaCanCL4, TramTron, TramTron.TreXaCanCL4));
                _textboxes.Add(new TagTextBox(TxtTreXaCanXM1, TramTron, TramTron.TreXaCanXM1));
                _textboxes.Add(new TagTextBox(TxtTreXaCanXM2, TramTron, TramTron.TreXaCanXM2));
                _textboxes.Add(new TagTextBox(TxtTreXaCanNuoc, TramTron, TramTron.TreXaCanNuoc));
                _textboxes.Add(new TagTextBox(TxtTreXaCanPG1, TramTron, TramTron.TreXaCanPG1));

                //_textboxes.Add(new TagTextBox(TxtVaoBTXCL1, TramTron, TramTron.TGVaoBTXCL1));
                //_textboxes.Add(new TagTextBox(TxtVaoBTXCL2, TramTron, TramTron.TGVaoBTXCL2));
                //_textboxes.Add(new TagTextBox(TxtVaoBTXCL3, TramTron, TramTron.TGVaoBTXCL3));
                //_textboxes.Add(new TagTextBox(TxtVaoBTXCL4, TramTron, TramTron.TGVaoBTXCL4));

                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaCL1, TramTron, TramTron.TreDongCuaXaCL1));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaCL2, TramTron, TramTron.TreDongCuaXaCL2));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaCL3, TramTron, TramTron.TreDongCuaXaCL3));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaCL4, TramTron, TramTron.TreDongCuaXaCL4));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaXM1, TramTron, TramTron.TreDongCuaXaXM1));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaXM2, TramTron, TramTron.TreDongCuaXaXM2));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaNuoc, TramTron, TramTron.TreDongCuaXaNuoc));
                _textboxes.Add(new TagTextBox(TxtTreDongCuaXaPG1, TramTron, TramTron.TreDongCuaXaPG1));

                _textboxes.Add(new TagTextBox(TxtTGCTXaCL1, TramTron, TramTron.TGChuTrinhXaCL1));
                _textboxes.Add(new TagTextBox(TxtTGCTXaCL2, TramTron, TramTron.TGChuTrinhXaCL2));
                _textboxes.Add(new TagTextBox(TxtTGCTXaCL3, TramTron, TramTron.TGChuTrinhXaCL3));
                _textboxes.Add(new TagTextBox(TxtTGCTXaCL4, TramTron, TramTron.TGChuTrinhXaCL4));
                _textboxes.Add(new TagTextBox(TxtTGMoXaCL1, TramTron, TramTron.TGMoXaCL1));
                _textboxes.Add(new TagTextBox(TxtTGMoXaCL2, TramTron, TramTron.TGMoXaCL2));
                _textboxes.Add(new TagTextBox(TxtTGMoXaCL3, TramTron, TramTron.TGMoXaCL3));
                _textboxes.Add(new TagTextBox(TxtTGMoXaCL4, TramTron, TramTron.TGMoXaCL4));

                //_textboxes.Add(new TagTextBox(TxtTgTreDungBTX, TramTron, TramTron.TGTreDungBTXMeCuoi));
                //_textboxes.Add(new TagTextBox(TxtTgCLDiQuaBTX, TramTron, TramTron.SetTGLenPheuCLTG));
                //_textboxes.Add(new TagTextBox(TxtTgMoThungCLTG, TramTron, TramTron.SetTGTreMoXaCLTG));
                
                _textboxes.Add(new TagTextBox(TxtTgTronBeTong, TramTron, TramTron.PrTGTronBeTong));
                _textboxes.Add(new TagTextBox(TxtTgTreMoCTHalf, TramTron, TramTron.PrTGTreMoCoiTronHalf));
                _textboxes.Add(new TagTextBox(TxtTgTreMoCT, TramTron, TramTron.PrTGTreMoCoiTron));

                _textboxes.Add(new TagTextBox(TxtTgCTDamRung, TramTron, TramTron.TGChuTrinhDamRung));
                _textboxes.Add(new TagTextBox(TxtTgBatDamRung, TramTron, TramTron.TGBatDamRung));
                #endregion

                #region Rung & sục khí
                _textboxes.Add(new TagTextBox(TxtRungCL1On, TramTron, TramTron.RungCL1On));
                _textboxes.Add(new TagTextBox(TxtRungCL1Cycle, TramTron, TramTron.RungCL1Cycle));
                _textboxes.Add(new TagTextBox(TxtRungCL2On, TramTron, TramTron.RungCL2On));
                _textboxes.Add(new TagTextBox(TxtRungCL2Cycle, TramTron, TramTron.RungCL2Cycle));
                _textboxes.Add(new TagTextBox(TxtRungCL3On, TramTron, TramTron.RungCL3On));
                _textboxes.Add(new TagTextBox(TxtRungCL3Cycle, TramTron, TramTron.RungCL3Cycle));

                _textboxes.Add(new TagTextBox(TxtRungXM1On, TramTron, TramTron.RungTCXM1On));
                _textboxes.Add(new TagTextBox(TxtRungXM1Cycle, TramTron, TramTron.RungTCXM1Cycle));
                _textboxes.Add(new TagTextBox(TxtRungXM2On, TramTron, TramTron.RungTCXM2On));
                _textboxes.Add(new TagTextBox(TxtRungXM2Cycle, TramTron, TramTron.RungTCXM2Cycle));

                _textboxes.Add(new TagTextBox(TxtSucKhiOn, TramTron, TramTron.SucKhiTimerOn));
                _textboxes.Add(new TagTextBox(TxtSucKhiCycle, TramTron, TramTron.SucKhiTimerCycle));

                _textboxes.Add(new TagTextBox(TxtRungCLTGTre, TramTron, TramTron.RungCLTGTre));

                _textboxes.Add(new TagTextBox(TxtBomMoRungOn, TramTron, TramTron.BomMoTGOn));
                _textboxes.Add(new TagTextBox(TxtBomMoRungOff, TramTron, TramTron.BomMoTGOff));
                #endregion

                #region Hiệu chuẩn cân
                WICalibCL1.SetTags(TramTron, TramTron.CLCanAI, TramTron.CLCanKL, TramTron.CLCanZero, TramTron.CLCanSpan);
                //WICalibCL2.SetTags(TramTron, TramTron.CLCanAI, TramTron.CLCanKL, TramTron.CLCanZero, TramTron.CLCanSpan);
                //WICalibCL3.SetTags(TramTron, TramTron.CLCanAI, TramTron.CLCanKL, TramTron.CLCanZero, TramTron.CLCanSpan);
                WICalibXM1.SetTags(TramTron, TramTron.XiCanAI, TramTron.XiCanKL, TramTron.XiCanZero, TramTron.XiCanSpan);
                //WICalibXM2.SetTags(TramTron, TramTron.XiCanAI, TramTron.XiCanKL, TramTron.XiCanZero, TramTron.XiCanSpan);
                WICalibWater.SetTags(TramTron, TramTron.NuocCanAI, TramTron.NuocCanKL, TramTron.NuocCanZero, TramTron.NuocCanSpan);
                #endregion

                #region Xe skip
                _textboxes.Add(new TagTextBox(TxtXeSkipTGDoCL, TramTron, TramTron.XeSkipTGDungDoCLDT2));
                _textboxes.Add(new TagTextBox(TxtXeSkipTGTrungCap, TramTron, TramTron.XeSkipTGTrungCap));
                _textboxes.Add(new TagTextBox(TxtXeSkipTGDT0DT2, TramTron, TramTron.XeSkipDT0_DT2));
                _textboxes.Add(new TagTextBox(TxtXeSkipTGDT0DT1, TramTron, TramTron.XeSkipDT0_DT1));
                #endregion

                // Cho phép đọc tham số
                TramTron.EnableRead(1);
            }

            foreach (var c in _textboxes)
            {
                if (c.Tag.Link == null) c.Txt.Visibility = Visibility.Collapsed;
            }
            foreach (var c in _checkboxes)
            {
                if (c.Tag.Link == null) c.Chk.Visibility = Visibility.Collapsed;
            }

            _tmr_t0 = DateTime.Now.Ticks;
            _tmr.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _tmr.Stop();
            // Dừng đọc tham số
            TramTron?.EnableRead(0);
        }

        private long _tmr_t0;
        private void _tmr_Tick(object? sender, EventArgs e)
        {
            long t = DateTime.Now.Ticks;
            double delta = (t - _tmr_t0) / 10000000d;
            _tmr_t0 = t;
            UpdateView(delta);
        }

        public void UpdateView(double delta)
        {
            if (TramTron != null && TramTron.IsRunning)
            {
                foreach (var t in _textboxes)
                {
                    t.UpdateView(delta);
                }
                foreach (var c in _checkboxes)
                {
                    c.UpdateView(delta);
                }

                foreach(var t in _textboxes) { t.UpdateView(delta); }

                WICalibCL1.Update();
                //WICalibCL2.Update();
                //WICalibCL3.Update();
                WICalibXM1.Update();
                //WICalibXM2.Update();
                WICalibWater.Update();
            }
        }

        private void BtUnlock_Click(object sender, RoutedEventArgs e)
        {
            WndUnlockPassword wnd = new WndUnlockPassword()
            {
                Owner = this
            };
            if (wnd.ShowDialog() == true)
            {
                PnlCalib.IsEnabled = true;
            }
        }

        private class TagTextBox
        {
            private double _v0 = double.MinValue;
            private ModelHeThong _ttbt;

            public TextBoxDouble Txt;
            public ModelTag Tag;

            public TagTextBox(TextBoxDouble txt, ModelHeThong tt, ModelTag tag)
            {
                Txt = txt;
                txt.Foreground = Brushes.Gray;
                txt.ValueChanged += Txt_ValueChanged;
                _ttbt = tt;
                Tag = tag;
            }

            private void Txt_ValueChanged(object? sender, double e)
            {
                if (Math.Abs(Txt.Value - Tag.Value) > 0.001)
                {
                    Txt.Foreground = Brushes.Red;
                    _ttbt.WriteTag(Tag, Txt.Value);
                }
            }

            public void UpdateView(double delta)
            {
                if (Tag.Value != _v0)
                {
                    Txt.Value = Math.Round(Tag.Value, 1);
                    Txt.Foreground = Brushes.Black;
                    _v0 = Tag.Value;
                }
            }
        }

        private class TagCheckbox
        {
            private double _v0 = double.MinValue;
            private bool _isUpdating = false;
            public CheckBox Chk;
            private ModelHeThong _ttbt;
            public ModelTag Tag;

            public TagCheckbox(CheckBox chk, ModelHeThong tt, ModelTag tag)
            {
                Chk = chk;
                chk.Checked += Chk_Checked;
                chk.Unchecked += Chk_Unchecked;
                _ttbt = tt;
                Tag = tag;
            }

            private void Chk_Unchecked(object sender, RoutedEventArgs e)
            {
                if (_isUpdating) return;
                _ttbt.WriteTag(Tag, 0);
            }

            private void Chk_Checked(object sender, RoutedEventArgs e)
            {
                if (_isUpdating) return;
                _ttbt.WriteTag(Tag, 1);
            }

            public void UpdateView(double delta)
            {
                if (Tag.Value != _v0)
                {
                    _isUpdating = true;
                    Chk.IsChecked = Tag.Value > 0.001;
                    _v0 = Tag.Value;
                    _isUpdating = false;
                }
            }
        }

        #region INotifyPropertyChanged
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = [];
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (_argsCache != null && propertyName != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChanged(_argsCache[propertyName]);
            }
        }

        private void NotifyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        #endregion
    }
}
