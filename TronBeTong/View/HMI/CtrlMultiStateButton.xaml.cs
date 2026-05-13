using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for CtrlMultiStateButton.xaml
    /// </summary>
    public partial class CtrlMultiStateButton : UserControl
    {
        #region ZText
        public string ZText
        {
            get { return (string)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }

        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(CtrlMultiStateButton), new PropertyMetadata(null));
        #endregion

        #region ZMaxState
        public int ZMaxState
        {
            get { return (int)GetValue(ZMaxStateProperty); }
            set { if (value > 0) SetValue(ZMaxStateProperty, value); }
        }
        public static readonly DependencyProperty ZMaxStateProperty =
            DependencyProperty.Register("ZMaxState", typeof(int), typeof(CtrlMultiStateButton), new PropertyMetadata(3, OnZMaxStateChanged));
        private static void OnZMaxStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CtrlMultiStateButton bt)
            {
                bt.PnlState.Visibility = bt.ZMaxState > 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        public Brush ZActiveFill
        {
            get { return (Brush)GetValue(ZActiveFillProperty); }
            set { SetValue(ZActiveFillProperty, value); }
        }
        public static readonly DependencyProperty ZActiveFillProperty =
            DependencyProperty.Register("ZActiveFill", typeof(Brush), typeof(CtrlMultiStateButton), new PropertyMetadata(null));

        /// <summary>
        /// Tự động cập nhật hiển thị
        /// </summary>
        public bool ZSelfUpdateView { get; set; } = false;
        private int _state = 0;
        public int ZState
        {
            get { return _state; }
            set { if (_state != value) { _state = value; UpdateView(); } }
        }
        public event EventHandler<int>? ZStateChanged;

        public CtrlMultiStateButton()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rectGlow.Visibility = Visibility.Visible;
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rectGlow.Visibility = Visibility.Hidden;
            if (Keyboard.Modifiers == ModifierKeys.Control) 
                HandleClicked(-1);
            else 
                HandleClicked();
        }

        public void UpdateView()
        {
            LED1.Fill = _state > 0 ? ZActiveFill : Brushes.Gray;
            LED2.Fill = _state > 1 ? ZActiveFill : Brushes.Gray;
            BdrBtn.Background = _state > 0 ? ZActiveFill : Brushes.WhiteSmoke;
            LblTitle.Foreground = IsEnabled? (_state > 0 ? Brushes.White : Brushes.Black): Brushes.LightGray;
        }

        private bool _hasFocus = false;
        private bool _isHover = false;
        private void BdrBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            _isHover = true;
            ShowFocus();
        }

        private void BdrBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            _isHover = false;
            ShowFocus();
        }

        private void BdrBtn_GotFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = true;
            ShowFocus();
        }

        private void BdrBtn_LostFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = false;
            ShowFocus();
        }

        private void ShowFocus()
        {
            rectFocus.Visibility = _hasFocus || _isHover? Visibility.Visible: Visibility.Hidden;
        }

        private void BdrBtn_KeyDown(object sender, KeyEventArgs e)
        {
            if (_hasFocus && e.Key == Key.Enter)
            {
                HandleClicked(1);
            }
        }

        private void HandleClicked(int i = 1)
        {
            int s = _state + i;
            if (s < 0) s += ZMaxState;
            s = s % ZMaxState;
            if (ZSelfUpdateView)
            {
                _state = s;
                UpdateView();
            }
            ZStateChanged?.Invoke(this, s);
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LblTitle.Foreground = IsEnabled ? (_state > 0 ? Brushes.White : Brushes.Black) : Brushes.LightGray;
        }
    }
}
