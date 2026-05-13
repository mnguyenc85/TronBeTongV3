using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TronBeTongV3.Comm;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlMidBin.xaml
    /// </summary>
    public partial class CtrlMidBin : UserControl
    {
        #region ZFill
        public Brush ZFill
        {
            get { return (Brush)GetValue(ZFillProperty); }
            set { SetValue(ZFillProperty, value); }
        }
        public static readonly DependencyProperty ZFillProperty =
            DependencyProperty.Register("ZFill", typeof(Brush), typeof(CtrlMidBin), new PropertyMetadata(null, OnZFillPropertyChanged));
        private static void OnZFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlMidBin s)
            {
                s.SetBrush(s.ZFill);
            }
        }
        #endregion

        public ModelHeThong? TramTron { get; set; }

        public int SetTGLenPheuCLTG { get; set; }
        public int SetTGTreMoXaCLTG { get; set; }

        public CtrlMidBin()
        {
            InitializeComponent();

            UpdateOutputValve(0, 0);
        }
        public void SetBrush(Brush b)
        {
            Img1.ZBlend = b;
        }

        public void UpdateOutputValve(int s, double delta)
        {
            Arrow1.SetState(s);
            BtOn1.ZState = s;
        }

        public void UpdateView(double delta)
        {
            if (TramTron == null) return;
            if (TramTron.SetTGTreMoXaCLTG.IsChanged)
            {
                SetTGLenPheuCLTG = (int)TramTron.SetTGLenPheuCLTG.Value;
                LblLenCLTG.Content = $"{TramTron.TGLenPheuCLTG.Value}/{SetTGLenPheuCLTG}";
            }
            if (TramTron.TGLenPheuCLTG.IsChanged)
            {
                LblLenCLTG.Content = $"{TramTron.TGLenPheuCLTG.Value}/{SetTGLenPheuCLTG}";
            }

            if (TramTron.SetTGTreMoXaCLTG.IsChanged)
            {
                SetTGTreMoXaCLTG = (int)TramTron.SetTGTreMoXaCLTG.Value;
                LblTreMoXaCLTG.Content = $"{TramTron.TGTreMoXaCLTG.Value}/{SetTGTreMoXaCLTG}";
            }
            if (TramTron.TGTreMoXaCLTG.IsChanged)
            {
                LblTreMoXaCLTG.Content = $"{TramTron.TGTreMoXaCLTG.Value}/{SetTGTreMoXaCLTG}";
            }

            if (TramTron.SensorMoCLTG.IsChanged)
            {
                Led1.IsOn = TramTron.SensorMoCLTG.GetBool();
            }

            if (TramTron.SensorDongCLTG.IsChanged)
            {
                LedClose.IsOn = TramTron.SensorDongCLTG.GetBool();
            }

            if (TramTron.VanCLTG.IsChanged)
            {
                UpdateOutputValve((int)TramTron.VanCLTG.Value, delta);
            }
        }
    }
}
