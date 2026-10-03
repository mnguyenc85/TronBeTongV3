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
    /// Interaction logic for CtrlWICalibration.xaml
    /// </summary>
    public partial class CtrlWICalibration : UserControl
    {
        private bool firstUpdate = false;
        private bool _sendZero, _sendSpan;
        private double _ai;

        private ModelHeThong? _ttbt;
        public ModelTag? TagAI { get; set; }
        public ModelTag? TagKL { get; set; }
        public ModelTag? TagZero { get; set; }
        public ModelTag? TagSpan { get; set; }

        public string ZTitle
        {
            get { return (string)GetValue(ZTitleProperty); }
            set { SetValue(ZTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZTitleProperty =
            DependencyProperty.Register("ZTitle", typeof(string), typeof(CtrlWICalibration), new PropertyMetadata(""));

        public CtrlWICalibration()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetTags(ModelHeThong ttbt, ModelTag ai, ModelTag kl, ModelTag zero, ModelTag span)
        {
            _ttbt = ttbt;
            TagAI = ai;
            TagKL = kl;
            TagZero = zero;
            TagSpan = span;
            IsEnabled = TagZero.Link != null && TagSpan.Link != null;
        }

        public void Update()
        {
            if (_ttbt == null) return;
            bool update = _ttbt.GetWriteTagCount(262) == 0;

            if (TagAI != null)
            {
                _ai = TagAI.Value;
                txtChanelAI.Text = TagAI.Value.ToString("F0");
            }
            if (TagKL != null)
            {
                txtMass.Text = TagKL.Value.ToString("F1");
            }

            if (TagZero != null && (!txtZero.IsFocused || _sendZero) && update)
            {
                if (_sendZero)
                {
                    txtZero.Foreground = Brushes.Black;
                    _sendZero = false;
                }
                txtZero.Text = TagZero.Value.ToString();
            }
            if (TagSpan != null && (!txtSpan.IsFocused || _sendSpan) && update)
            {
                if (_sendSpan)
                {
                    txtSpan.Foreground = Brushes.Black;
                    _sendSpan = false;
                }
                txtSpan.Text = TagSpan.Value.ToString("F6");
            }
            firstUpdate = true;
        }

        private void BtSetZero_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtChanelAI.Text, out int v))
            {
                txtZero.Text = txtChanelAI.Text;
                _setZero = v;
                SendZero();
            }
        }

        private void BtSetSpan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSetMass.Text))
            {
                MessageBox.Show("Ô `Set Mass` không được để trống!", "Đặt span", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (double.TryParse(txtSetMass.Text, out double klNhap))
            {
                double deltaAI = _ai - _setZero;
                if (Math.Abs(deltaAI) > 0) {
                    _setSpan = Math.Round(klNhap / deltaAI, 6);
                    txtSpan.Text = _setSpan.ToString();
                    SendSpan();
                }
            }
        }

        private double _setZero;
        private void txtZero_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!txtZero.IsFocused) return;
            if (double.TryParse(txtZero.Text, out _setZero))
            {
                BtSetZero.IsEnabled = true;
                LblWriteZero.Text = _setZero.ToString("F0");
            }
            else
            {
                BtSetZero.IsEnabled = false;
            }
        }

        private double _setSpan;
        private void txtSpan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!txtSpan.IsFocused) return;
            if (double.TryParse(txtSpan.Text, out _setSpan))
            {
                BtSetSpan.IsEnabled = true;
                LblWriteSpan.Text = _setSpan.ToString("F5");
            }
            else
            {
                BtSetSpan.IsEnabled = false;
            }
        }

        private void LblSetMass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                LblWriteZero.Visibility = Visibility.Visible;
                LblWriteSpan.Visibility = Visibility.Visible;
            }
        }

        private void txtZero_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendZero();
            }
        }
        private void txtSpan_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendSpan();
            }
        }

        private void SendZero()
        {
            if (_ttbt != null && TagZero != null)
            {
                // if (double.TryParse(txtZero.Text, out double z))
                // _ttbt.WriteTag(TagZero, z);
                _ttbt.WriteTag(TagZero, _setZero);
                txtZero.Foreground = Brushes.Red;
                _sendZero = true;
            }
        }

        private void SendSpan()
        {
            if (_ttbt != null && TagSpan != null)
            {
                // if (double.TryParse(txtSpan.Text, out double s))
                // _ttbt.WriteTag(TagSpan, s);
                _ttbt.WriteTag(TagSpan, _setSpan);
                txtSpan.Foreground = Brushes.Red;
                _sendSpan = true;
            }
        }
    }
}
