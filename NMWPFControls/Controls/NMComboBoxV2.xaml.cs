using NMWPFControls.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for NMComboBoxV2.xaml
    /// </summary>
    public partial class NMComboBoxV2 : UserControl
    {
        #region ZOpened
        public bool ZOpened
        {
            get { return (bool)GetValue(ZOpenedProperty); }
            set { SetValue(ZOpenedProperty, value); }
        }
        public static readonly DependencyProperty ZOpenedProperty =
            DependencyProperty.Register("ZOpened", typeof(bool), typeof(NMComboBoxV2), new PropertyMetadata(false));
        #endregion

        #region ZListItemTemplate
        public static readonly DependencyProperty ZListItemTemplateProperty =
            DependencyProperty.Register(nameof(ZListItemTemplate), typeof(DataTemplate), typeof(NMComboBoxV2), new PropertyMetadata(null));
        public DataTemplate ZListItemTemplate
        {
            get => (DataTemplate)GetValue(ZListItemTemplateProperty);
            set => SetValue(ZListItemTemplateProperty, value);
        }
        #endregion

        #region ZItemsSource
        public static readonly DependencyProperty ZItemsSourceProperty =
            DependencyProperty.Register(nameof(ZItemsSource), typeof(IEnumerable), typeof(NMComboBoxV2), new PropertyMetadata(null, OnZItemSourcePropertyChanged));

        private static void OnZItemSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NMComboBoxV2 c)
            {
                c._cvs.Source = c.ZItemsSource;
                c.LvItems.ItemsSource = c.CVZItemsSource;
                foreach (var sd in c.SortDescriptions)
                {
                    c.CVZItemsSource.SortDescriptions.Add(sd);
                }
            }
        }
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
            DependencyProperty.Register("ZText", typeof(string), typeof(NMComboBoxV2), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region ZSelectedItem
        public object? ZSelectedItem
        {
            get { return (object)GetValue(ZSelectedItemProperty); }
            set { SetValue(ZSelectedItemProperty, value); }
        }
        public static readonly DependencyProperty ZSelectedItemProperty =
            DependencyProperty.Register("ZSelectedItem", typeof(object), typeof(NMComboBoxV2), new PropertyMetadata(null));
        #endregion

        #region ZTypedText
        private string? _ttxt;
        /// <summary>
        /// Chỉ thay đổi thi gõ phím
        /// </summary>
        public string? ZTypedText { get { return _ttxt; } private set { EmitZTTxt(value); } }
        private void EmitZTTxt(string? s)
        {
            if (_ttxt != s)
            {
                _ttxt = s;
                _fitler.Run(s);
                ZTypedTextChanged?.Invoke(this, s);
            }
        }
        public event EventHandler<string?>? ZTypedTextChanged;
        #endregion

        public FrameworkElement? ZNextFocus { get; set; }
        public event EventHandler<NMCboSelectedChangedArgs>? ZSelectedChanged;

        private readonly DelayUpdate<string> _fitler = new(300);
        private readonly CollectionViewSource _cvs = new();
        public ICollectionView CVZItemsSource { get { return _cvs.View; } }
        public string? DisplayMemberPath { get; set; }
        public StringComparison ZStrComparison { get; set; } = StringComparison.CurrentCultureIgnoreCase;
        public List<SortDescription> SortDescriptions { get; private set; } = new List<SortDescription>();

        public TextBox InnerTextBox { get { return TxtZText; } }

        public NMComboBoxV2()
        {
            InitializeComponent();
            PnlMain.DataContext = this;

            _fitler.ExecUpdate = f => ExecFilter(f);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _fitler.Run(null, true);
        }

        public void ZClearSelected()
        {
            ZSelectedItem = null;
            ZText = null;
        }

        public void SetListViewSelectedItem(object? item)
        {
            LvItems.SelectedItem = item;
        }

        public void ZClearFilter()
        {
            _fitler.Run(null, true);
        }

        private bool ExecFilter(string? s)
        {
            if (CVZItemsSource != null)
            {
                if (DisplayMemberPath != null)
                {
                    CVZItemsSource.Filter = x =>
                    {
                        if (string.IsNullOrEmpty(s)) 
                            return true;
                        var p = x.GetType().GetProperty(DisplayMemberPath)?.GetValue(x);
                        if (p != null)
                        {
                            string? ps = p.ToString();
                            return ps != null && ps.Contains(s, ZStrComparison);
                        }
                        return false;
                    };
                }
                else {
                    CVZItemsSource.Filter = x =>
                    {
                        if (string.IsNullOrEmpty(s)) return true;
                        string? ps = x.ToString();
                        return ps != null && ps.Contains(s, ZStrComparison);
                    };
                }
                return true;
            }
            return false;
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
            else if (e.Key == Key.Enter)
            {
                if (ZItemsSource != null)
                {
                    object? obj = null;
                    if (!string.IsNullOrEmpty(DisplayMemberPath))
                    {
                        foreach (var item in ZItemsSource)
                        {
                            var p = item.GetType().GetProperty(DisplayMemberPath)?.GetValue(item);
                            if (p != null)
                            {
                                string? ps = p.ToString();
                                if (ps == ZText)
                                {
                                    obj = item;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ZItemsSource)
                        {
                            string? ps = item.ToString();
                            if (ps == ZText)
                            {
                                obj = item;
                                break;
                            }
                        }
                    }

                    if (obj != null)
                    {
                        ZSelectedItem = obj;
                        ZSelectedChanged?.Invoke(this, new NMCboSelectedChangedArgs()
                        {
                            Selected = ZSelectedItem,
                            IsDecided = true
                        });
                    }
                }
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

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled)
            {
                ZOpened = false;
            }
            TxtZText.IsEnabled = IsEnabled;
        }
    }
}
