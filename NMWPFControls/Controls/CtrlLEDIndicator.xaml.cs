using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for CtrlLEDIndicator.xaml
    /// </summary>
    public partial class CtrlLEDIndicator : UserControl
    {
        #region TitleProperty
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            name: "Title",
            propertyType: typeof(string),
            ownerType: typeof(CtrlLEDIndicator),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        #region OnColorProperty
        public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register(
            name: "OnColor",
            propertyType: typeof(Brush),
            ownerType: typeof(CtrlLEDIndicator),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: Brushes.Red));

        public Brush OnColor
        {
            get => (Brush)GetValue(OnColorProperty);
            set => SetValue(OnColorProperty, value);
        }
        #endregion

        #region IsOnProperty
        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
            name: "IsOn",
            propertyType: typeof(bool),
            ownerType: typeof(CtrlLEDIndicator),
            typeMetadata: new FrameworkPropertyMetadata(defaultValue: false));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }
        #endregion

        public CtrlLEDIndicator()
        {
            InitializeComponent();

            DataContext = this;
        }

    }
}
