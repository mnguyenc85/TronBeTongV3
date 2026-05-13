using System;
using System.Windows;
using System.Windows.Controls;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlXeSkip.xaml
    /// </summary>
    public partial class GrCtrlXeSkip : UserControl
    {
        private Point[] _loc = [new Point(), new Point(420, -212), new Point(840, -424)];

        private bool _vArrUp1, _vArrUp2, _vArrDown1, _vArrDown2;
        public bool VisibleArrowUp1
        {
            get { return _vArrUp1; }
            set { if (_vArrUp1 != value) { _vArrUp1 = value; GrArrowUp1.Visibility = _vArrUp1 ? Visibility.Visible : Visibility.Collapsed; } }
        }
        public bool VisibleArrowUp2
        {
            get { return _vArrUp2; }
            set { if (_vArrUp2 != value) { _vArrUp2 = value; GrArrowUp2.Visibility = _vArrUp2 ? Visibility.Visible : Visibility.Collapsed; } }
        }
        public bool VisibleArrowDown1
        {
            get { return _vArrDown1; }
            set { if (_vArrDown1 != value) { _vArrDown1 = value; GrArrowDown1.Visibility = _vArrDown1 ? Visibility.Visible : Visibility.Collapsed; } }
        }
        public bool VisibleArrowDown2
        {
            get { return _vArrDown2; }
            set { if (_vArrDown2 != value) { _vArrDown2 = value; GrArrowDown2.Visibility = _vArrDown2 ? Visibility.Visible : Visibility.Collapsed; } }
        }

        public bool DT0 { get { return LEDDT0.IsOn; } set { LEDDT0.IsOn = value; } }
        public bool DT1 { get { return LEDDT1.IsOn; } set { LEDDT1.IsOn = value; } }
        public bool DT2 { get { return LEDDT2.IsOn; } set { LEDDT2.IsOn = value; } }
        public GrCtrlXeSkip()
        {
            InitializeComponent();
        }

        public void SetSkipPos(int i, double r)
        {
            int n = _loc.Length - 1;
            double x = 0, y = 0;
            if (r >= 1)
            {
                if (i < n)
                {
                    x = _loc[i + 1].X; y = _loc[i + 1].Y;
                }
                else
                {
                    x = _loc[i].X; y = _loc[i].Y;
                }
            }
            else if (r >= 0 && r < 1)
            {
                if (i < n)
                {
                    x = _loc[i].X * (1 - r) + _loc[i + 1].X * r;
                    y = _loc[i].Y * (1 - r) + _loc[i + 1].Y * r;
                }
                else
                {
                    x = _loc[i].X; y = _loc[i].Y;
                }
            }
            else if (r >= -1 && r < 0)
            {
                if (i > 0)
                {
                    x = _loc[i - 1].X * (-r) + _loc[i].X * (r + 1);
                    y = _loc[i - 1].Y * (-r) + _loc[i].Y * (r + 1);
                }
                else
                {
                    x = _loc[0].X; y = _loc[0].Y;
                }
            }
            else
            {
                if (i > 0)
                {
                    x = _loc[i - 1].X; y = _loc[i - 1].Y;
                }
                else
                {
                    x  = _loc[0].X; y = _loc[0].Y;
                }
            }
            Canvas.SetLeft(GrSkip, x);
            Canvas.SetTop(GrSkip, y);

            LblDebug.Text = $"{i} {r:F3}";
        }
    }
}
