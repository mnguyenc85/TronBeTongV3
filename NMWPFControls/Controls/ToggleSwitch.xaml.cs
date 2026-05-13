using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        #region ZOnProperty
        public static readonly DependencyProperty ZOnProperty = DependencyProperty.Register(
            name: "ZOn",
            propertyType: typeof(bool),
            ownerType: typeof(ToggleSwitch),
            typeMetadata: new FrameworkPropertyMetadata(false, new PropertyChangedCallback(ZOnChanged)));

        private static void ZOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleSwitch ctrl)
            {
                ctrl.ZValueChanged?.Invoke(ctrl, ctrl.ZOn);
            }
        }

        public bool ZOn
        {
            get => (bool)GetValue(ZOnProperty);
            set => SetValue(ZOnProperty, value);
        }
        #endregion

        #region ZOnTitleProperty
        public static readonly DependencyProperty ZOnTitleProperty = DependencyProperty.Register(
            name: "ZOnTitle",
            propertyType: typeof(string),
            ownerType: typeof(ToggleSwitch),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

        public string ZOnTitle
        {
            get => (string)GetValue(ZOnTitleProperty);
            set => SetValue(ZOnTitleProperty, value);
        }
        #endregion

        #region ZOffTitleProperty
        public static readonly DependencyProperty ZOffTitleProperty = DependencyProperty.Register(
            name: "ZOffTitle",
            propertyType: typeof(string),
            ownerType: typeof(ToggleSwitch),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

        public string ZOffTitle
        {
            get => (string)GetValue(ZOffTitleProperty);
            set => SetValue(ZOffTitleProperty, value);
        }
        #endregion

        public event EventHandler<bool>? ZRequestChangeValue;
        public event EventHandler<bool>? ZValueChanged;
        public bool ZCanChangeFromUI { get; set; } = true;
        
        public ToggleSwitch()
        {
            InitializeComponent();
            BdrMain.DataContext = this;
        }

        private void BorderLatchOn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Toggle(true);
            //Focus();
        }

        private void BorderLatchOff_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Toggle(false);
            //Focus();
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusRectOn.Visibility = Visibility.Visible;
            FocusRectOff.Visibility = Visibility.Visible;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusRectOn.Visibility = Visibility.Collapsed;
            FocusRectOff.Visibility = Visibility.Collapsed;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Toggle(!ZOn);
                    e.Handled = true;
                    break;
                case Key.Left:
                    if (ZOn)
                    {
                        Toggle(false);
                        e.Handled = true;
                    }
                    break;
                case Key.Right:
                    if (!ZOn)
                    {
                        Toggle(true);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void Toggle(bool v)
        {
            ZRequestChangeValue?.Invoke(this, v);
            if (ZCanChangeFromUI)
            {
                ZOn = v;
                ZValueChanged?.Invoke(this, v);
            }
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                LblTitleOn.Foreground = Brushes.White;
                LblTitleOff.Foreground = Brushes.WhiteSmoke;
            }
            else
            {
                LblTitleOn.Foreground = Brushes.Gray;
                LblTitleOff.Foreground = Brushes.Gray;
            }
        }
    }
}
