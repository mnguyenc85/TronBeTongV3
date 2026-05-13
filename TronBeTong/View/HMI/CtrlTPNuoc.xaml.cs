using System;
using System.Windows;
using System.Windows.Controls;
using TronBeTongV3.Data.DO.DonHang;
using TronBeTongV3.Comm;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlTPNuoc.xaml
    /// </summary>
    public partial class CtrlTPNuoc : UserControl
    {
        private const double _EPSILON = 0.01;

        public ModelHeThong? TramTron { get; set; }

        public int MeDat { get; set; }
        public int MeMax { get { return WSWater.MeHT; } }

        private double add_water = 0;

        public CtrlTPNuoc()
        {
            InitializeComponent();
            WSWater.LoaiCan = 2;
            WSWater.DigitFormat = "0";
            WSWater.ZShowDischargeTime = false;
            SiloWater.RoundFormat = "0";
            SiloWater.RoundDigit = 0;
        }

        public void LoadThamSo(DbSettings s)
        {
            WSWater.ChotKLTre = s.GetDoubleValueFromString("chotkl.ts.nuoc.tre", 1);
        }

        /// <summary>
        /// Có thể chốt mẻ không?
        /// </summary>
        public bool CheckChotMe()
        {
            bool ret = false;
            if (TramTron != null)
            {
                if (WSWater.CheckChotMe(TramTron.WIState.ChotNuoc))
                {
                    // Kiểm tra kl chốt
                    if (TramTron.NuocChot.Value > 0)
                    {
                        //WSWater.IsSaveWeightsState = false;
                        WSWater.IsSaveWeightsState = false;
                        ret = true;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Điền dữ liệu vào mẻ m
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true nếu thực hiện điền dữ liệu</returns>
        public bool FillMe(DHMeDO m)
        {
            if (TramTron == null) return false;

            bool dienDL = false;
            if ((m.Flags & 2) == 2 || WSWater.NeedSaveWeights)
            {
                if (m.STT == WSWater.MeHT && !m.ChotNuoc && !WSWater.DaChotKLVaoMe)
                {
                    m.KLNuoc = Math.Round(TramTron.NuocChot.Value, 3);
                    m.ChotNuoc = true;
                    dienDL = true;
                    WSWater.DaChotKLVaoMe = true;
                }
                WSWater.NeedSaveWeights = false;
            }

            return dienDL;
        }

        public void Update(double delta)
        {
            if (TramTron == null) return;

            int some = (int)TramTron.MeSoMe.Value;

            if (TramTron.NuocCapPhoi.IsChanged || TramTron.MeM3Dat.IsChanged || TramTron.MeSoMe.IsChanged)
                SiloWater.UpdateCapPhoi(TramTron.NuocCapPhoi.Value, TramTron.MeM3Dat.Value, some);

            if (TramTron.NuocCPMe.IsChanged)
                SiloWater.UpdateKLMe(TramTron.NuocCPMe.Value, some);

            double themNuoc = TramTron.NuocAdd.Value;
            if (Math.Abs(add_water - themNuoc) > _EPSILON)
            {
                TxtAddWater.BlockInvoke = true;
                TxtAddWater.Value = Math.Round(themNuoc, 1);
                TxtAddWater.BlockInvoke = false;
                add_water = themNuoc;
            }

            SiloWater.UpdateCanThuc(TramTron.NuocChot.Value);

            WSWater.UpdateView(TramTron.WIState, TramTron.CanNuoc, MeDat, delta);
            
            SiloWater.ZState = (int)TramTron.VanNuoc.Value;
            SetWasherState(TramTron.SysWashMixer.GetBool());
        }

        private void WSWater_ButtonClicked(object sender, ButtonArgs e)
        {
        }

        private void SiloWater_ButtonClicked(object sender, ButtonArgs e)
        {
        }

        private void BtStartWash_Click(object sender, RoutedEventArgs e)
        {
            if (TramTron == null || TramTron.SysRunning.GetBool()) return;

            TramTron.S71200_WriteMixerWash(1);
        }

        private void BtStopWash_Click(object sender, RoutedEventArgs e)
        {
            //if (TramTron == null || TramTron.SysRunning.GetBool()) return;
            TramTron?.S71200_WriteMixerWash(0);
        }

        private void TxtAddWater_ValueChanged(object sender, double e)
        {
            TramTron?.S71200_WriteNuocAdd(TxtAddWater.Value);
        }

        public void SetWasherState(bool s)
        {
            LEDWashMixer.IsOn = s;
            BtStartWash.Visibility = s ? Visibility.Hidden : Visibility.Visible;
            BtStopWash.Visibility = s ? Visibility.Visible : Visibility.Hidden;
        }

        public void EnableWasher(bool en)
        {
            BtStartWash.IsEnabled = en;
            //BtStopWash.IsEnabled = en;
        }


        /// <summary>
        /// Kiểm tra xem có cân nào ở trạng thái đầy không?
        /// </summary>
        /// <returns></returns>
        public int CheckCanDay()
        {
            if (TramTron == null) return 0;
            if (WSWater.TTCanHT == TramTron.WIState.DayNuoc) return 1;
            return 0;
        }
    }
}
