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
            if (TagAI != null && (TagAI.IsChanged || !firstUpdate))
            {
                txtChanelAI.Text = TagAI.Value.ToString("F0");
            }
            if (TagKL != null && (TagKL.IsChanged || !firstUpdate))
            {
                txtMass.Text = TagKL.Value.ToString("F1");
            }

            if (TagZero != null && (TagZero.IsChanged || !firstUpdate))
            {
                txtZero.Text = TagZero.Value.ToString();
            }
            if (TagSpan != null && (TagSpan.IsChanged || !firstUpdate))
            {
                txtSpan.Text = TagSpan.Value.ToString("F5");
            }
            firstUpdate = true;
        }

        private void BtSetZero_Click(object sender, RoutedEventArgs e)
        {
            if (_ttbt != null && TagZero != null)
            {
                if (double.TryParse(txtZero.Text, out double z))
                    _ttbt.WriteTag(TagZero, z);
            }
        }

        private void BtSetSpan_Click(object sender, RoutedEventArgs e)
        {
            if (_ttbt != null && TagSpan != null)
            {
                if (double.TryParse(txtSpan.Text, out double s))
                    _ttbt.WriteTag(TagSpan, s);
            }
        }
    }
}
