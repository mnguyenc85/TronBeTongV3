using Org.BouncyCastle.Math.Field;
using System;
using System.Windows;
using System.Windows.Controls;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for GrCtrlKhungXeSkip.xaml
    /// </summary>
    public partial class GrCtrlKhungXeSkip : UserControl
    {
        /// <summary>
        /// Hiển thị DT0
        /// </summary>
        public bool ZDT0 {
            get
            {
                return LEDDT0.IsOn;
            }
            set
            {
                if (LEDDT0.IsOn != value) { LEDDT0.IsOn = value; }
            }
        }
        /// <summary>
        /// Hiển thị DT1
        /// </summary>
        public bool ZDT1
        {
            get
            {
                return LEDDT1.IsOn;
            }
            set
            {
                if (LEDDT1.IsOn != value) { LEDDT1.IsOn = value; }
            }
        }
        /// <summary>
        /// Hiển thị DT2
        /// </summary>
        public bool ZDT2
        {
            get
            {
                return LEDDT2.IsOn;
            }
            set
            {
                if (LEDDT2.IsOn != value) { 
                    LEDDT2.IsOn = value; 
                    BdrTGXaCL.Visibility = value? Visibility.Visible: Visibility.Collapsed;
                }
            }
        }

        private bool _isUp, _isDown, _isEMC;
        public bool ZIsUp
        {
            get { return _isUp; }
            set { if (_isUp != value) { SetArrowUp(value); }  }
        }

        public bool ZIsDown
        {
            get { return _isDown; }
            set { if (_isDown != value) { SetArrowDown(value); } }
        }

        /// <summary>
        /// Hiển thị EMC
        /// </summary>
        public bool ZEMC
        {
            get { return _isEMC; }
            set { if (LEDDEMC.IsOn != value) { SetEMC(value); } }
        }

        private int _tgXaCLEt;
        public int TGXaCLEt
        {
            get { return _tgXaCLEt; }
            set
            {
                if (_tgXaCLEt != value)
                {
                    _tgXaCLEt = value;
                    LblTGXaCLEt.Text = _tgXaCLEt.ToString();
                }
            }
        }

        public GrCtrlKhungXeSkip()
        {
            InitializeComponent();

            _isDown = ArrowDown.Visibility == Visibility.Visible;
            _isUp = ArrowUp.Visibility == Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetArrowUp(_isUp);
            SetArrowDown(_isDown);
        }

        public void SetArrowUp(bool isShow)
        {
            _isUp = isShow;
            ArrowUp.Visibility = _isUp? Visibility.Visible : Visibility.Collapsed;
        }
        public void SetArrowDown(bool isShow)
        {
            _isDown = isShow;
            ArrowDown.Visibility = _isDown ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetEMC(bool isEMC)
        {
            _isEMC = isEMC;
            LEDDEMC.IsOn = _isEMC;
            LEDDEMC.Visibility = _isEMC? Visibility.Visible : Visibility.Collapsed;
        }

        private const double _emcBlinkDelay = 500;
        private double _emcBlinkET = 0;
        public void Update(double et)
        {
            if (_isEMC)
            {
                _emcBlinkET += et;
                if (_emcBlinkET > _emcBlinkDelay)
                {
                    LEDDEMC.IsOn = !LEDDEMC.IsOn;
                    _emcBlinkET -= 500;
                }
            }
        }
    }
}
