using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for NMComboBox.xaml
    /// </summary>
    public partial class NMComboBox : UserControl
    {
        #region ZOpened
        public bool ZOpened
        {
            get { return (bool)GetValue(ZOpenedProperty); }
            set { SetValue(ZOpenedProperty, value); }
        }

        public static readonly DependencyProperty ZOpenedProperty =
            DependencyProperty.Register("ZOpened", typeof(bool), typeof(NMComboBox), new PropertyMetadata(false));
        #endregion

        #region ZListItemTemplate
        public static readonly DependencyProperty ZListItemTemplateProperty =
            DependencyProperty.Register(
            nameof(ZListItemTemplate),
            typeof(DataTemplate),
            typeof(NMComboBox),
            new PropertyMetadata(null));
        public DataTemplate ZListItemTemplate
        {
            get => (DataTemplate)GetValue(ZListItemTemplateProperty);
            set => SetValue(ZListItemTemplateProperty, value);
        }
        #endregion

        #region ZItemsSource
        public static readonly DependencyProperty ZItemsSourceProperty =
            DependencyProperty.Register(nameof(ZItemsSource), typeof(IEnumerable), typeof(NMComboBox), new PropertyMetadata(null));

        public IEnumerable ZItemsSource
        {
            get => (IEnumerable)GetValue(ZItemsSourceProperty);
            set => SetValue(ZItemsSourceProperty, value);
        }
        #endregion

        #region ZText
        public string? ZText
        {
            get { return (string?)GetValue(ZTextProperty); }
            set { SetValue(ZTextProperty, value); }
        }
        public static readonly DependencyProperty ZTextProperty =
            DependencyProperty.Register("ZText", typeof(string), typeof(NMComboBox), new PropertyMetadata(null, ZTextPropertyChanged));
        private static void ZTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NMComboBox comboBox)
            {
            }
        }
        #endregion

        #region ZSelectedItem
        public object? ZSelectedItem
        {
            get { return (object)GetValue(ZSelectedItemProperty); }
            set { SetValue(ZSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty ZSelectedItemProperty =
            DependencyProperty.Register("ZSelectedItem", typeof(object), typeof(NMComboBox), new PropertyMetadata(null, ZSelectedItemPropertyChanged));

        private static void ZSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NMComboBox c)
            {
            }
        }
        #endregion

        #region ZTypedText
        private string? _ttxt;
        /// <summary>
        /// Chỉ thay đổi thi gõ phím
        /// </summary>
        public string? ZTypedText { get { return _ttxt; } private set { EmitZTTxt(value); } }
        private void EmitZTTxt(string? s)
        {
            if (_ttxt != s) { 
                _ttxt = s;
                ZTypedTextChanged?.Invoke(this, s);
            }
        }
        public event EventHandler<string?>? ZTypedTextChanged;
        #endregion

        public FrameworkElement? ZNextFocus { get; set; }
        public event EventHandler<NMCboSelectedChangedArgs>? ZSelectedChanged;

        public TextBox InnerTextBox { get { return TxtZText; } }

        public NMComboBox()
        {
            InitializeComponent();
            PnlMain.DataContext = this;
        }

        private bool isTyped = false;
        private void TxtZText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                ZOpened = true;
                FocusOnListViewItem();
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                ZOpened = false;
            }
            else if (e.Key == Key.Escape)
            {
                ZOpened = false;
                e.Handled = true;
            }
            else
            {
                isTyped = true;
            }
        }
        private void TxtZText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isTyped) ZTypedText = TxtZText.Text;
            isTyped = false;
        }

        private void LvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ZSelectedItem = LvItems.SelectedItem;
            ZSelectedChanged?.Invoke(this, new NMCboSelectedChangedArgs()
            {
                Selected = ZSelectedItem,
                IsDecided = false
            });
        }

        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            ZOpened = !ZOpened;
            if (ZOpened)
            {
                FocusOnListViewItem();
            }
        }

        private void FocusOnListViewItem()
        {
            if (LvItems.SelectedIndex < 0)
            {
                LvItems.Focus();
            }
            else
            {
                int i = LvItems.SelectedIndex < 0 ? 0 : LvItems.SelectedIndex;
                var item = LvItems.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                item?.Focus();
            }
        }

        private void LvItems_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectAndClose();
        }

        private void LvItems_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectAndClose();
                TxtZText.Focus();
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
            {
                if (ZNextFocus != null)
                {
                    ZNextFocus.Focus();
                    ZOpened = false;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Left || e.Key == Key.Right)
            {
                TxtZText.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                if (LvItems.Items.Count == 0)
                {
                    TxtZText.Focus();
                    ZOpened = false;
                    e.Handled = true;
                }
            }
        }

        private void SelectAndClose()
        {
            //ZSelectedItem = LvItems.SelectedItem;
            ZOpened = false;
            ZSelectedChanged?.Invoke(this, new NMCboSelectedChangedArgs()
            {
                Selected = ZSelectedItem,
                IsDecided = true
            });
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (!this.IsKeyboardFocusWithin && !Popup.IsKeyboardFocusWithin)
            {
                ZOpened = false;
            }
        }

        private void Popup_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsKeyboardFocusWithin && !Popup.IsKeyboardFocusWithin)
            {
                ZOpened = false;
            }
        }
    }
}
