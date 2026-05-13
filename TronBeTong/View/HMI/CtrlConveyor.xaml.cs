using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlConveyor.xaml
    /// </summary>
    public partial class CtrlConveyor : UserControl
    {
        #region RAngle
        public double RAngle
        {
            get { return (double)GetValue(RAngleProperty); }
            set { SetValue(RAngleProperty, value); }
        }
        public static readonly DependencyProperty RAngleProperty =
            DependencyProperty.Register("RAngle", typeof(double), typeof(CtrlConveyor), new PropertyMetadata(0d));

        public Point RCenter
        {
            get { return (Point)GetValue(RCenterProperty); }
            set { SetValue(RCenterProperty, value); }
        }

        public static readonly DependencyProperty RCenterProperty =
            DependencyProperty.Register("RCenter", typeof(Point), typeof(CtrlConveyor), new PropertyMetadata(new Point()));
        #endregion

        #region ZLength
        public int ZLength
        {
            get { return (int)GetValue(ZLengthProperty); }
            set { SetValue(ZLengthProperty, value); }
        }

        public static readonly DependencyProperty ZLengthProperty =
            DependencyProperty.Register("ZLength", typeof(int), typeof(CtrlConveyor), new PropertyMetadata(0, OnZLengthPropertyChanged));
        private static void OnZLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlConveyor c) c.UpdateLength();
        }
        #endregion

        #region ZState
        public int ZState
        {
            get { return (int)GetValue(ZStateProperty); }
            set { SetValue(ZStateProperty, value); }
        }
        public static readonly DependencyProperty ZStateProperty =
            DependencyProperty.Register("ZState", typeof(int), typeof(CtrlConveyor), new PropertyMetadata(0, OnZStatePropertyChanged));

        private static void OnZStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlConveyor c)
            {
                c.SetState();
            }
        }
        #endregion

        #region ZMaxState
        /// <summary>
        /// Số trạng thái tối đa của nút động cơ.
        /// Ví dụ: dừng, tốc độ 1, tốc độ 2 = 3
        /// </summary>
        public int ZMaxState
        {
            get { return (int)GetValue(ZMaxStateProperty); }
            set { SetValue(ZMaxStateProperty, value); }
        }
        public static readonly DependencyProperty ZMaxStateProperty =
            DependencyProperty.Register("ZMaxState", typeof(int), typeof(CtrlConveyor), new PropertyMetadata(3));
        #endregion

        public int Id { get; set; }
        public event EventHandler<ButtonArgs>? ButtonClick;

        public CtrlConveyor()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
        }

        private bool _isupdate = false;
        public void UpdateLength()
        {
            if (ZLength < 0 || _isupdate) return;

            _isupdate = true;
            PnlMiddle.Children.Clear();

            for (int i = 0; i < ZLength; i++)
            {
                BlendImageCtrl img = new()
                {
                    ZSource = ImgMiddle.Source
                };
                PnlMiddle.Children.Add(img);
            }
            _isupdate = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SetState()
        {
            if (ZState > 0)
            {
                ImgLeft.ZBlend = BtMotor.ZActiveFill;
                ImgRight.ZBlend = BtMotor.ZActiveFill;
            }
            else
            {
                ImgLeft.ZBlend = null;
                ImgRight.ZBlend = null;
            }
            BtMotor.ZState = ZState;
        }

        private void BtMotor_ZStateChanged(object sender, int e)
        {
            ButtonClick?.Invoke(this, new ButtonArgs()
            {
                Button = Core.ButtonTypes.DongCo,
                BtState = e,
                ObjectId = Id
            });
        }
    }
}
