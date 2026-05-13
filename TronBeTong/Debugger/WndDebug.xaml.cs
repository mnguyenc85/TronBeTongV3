using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using NMWPFControls.Core;
using TronBeTongV3.Comm;

namespace TronBeTongV3.Debugger
{
    /// <summary>
    /// Interaction logic for WndDebug.xaml
    /// </summary>
    public partial class WndDebug : Window
    {
        private double _t0;
        private DebuggerUtils _utils = new();
        private DelayUpdate<string> _filter;
        private int _stt = 0;

        public ObservableCollection<PlcVar> LstVars { get; private set; } = [];
        private readonly CollectionViewSource _cvsLstVars = new();
        public ICollectionView CVLstVars { get { return _cvsLstVars.View; } }


        public ModelHeThong? TramTron { get; set; }
        public TagsObserver? Observer { get; set; }

        public WndDebug()
        {
            InitializeComponent();
            DataContext = this;

            _cvsLstVars.Source = LstVars;
            _filter = new(300)
            {
                ExecUpdate = f =>
                {
                    _stt = 0;
                    CVLstVars.Filter = x =>
                    {
                        if (x is PlcVar v)
                        {
                            if (string.IsNullOrEmpty(f))
                            {
                                v.STT = ++_stt;
                                return true;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(v.DiaChi) && v.DiaChi.Contains(f, StringComparison.OrdinalIgnoreCase))
                                {
                                    v.STT = ++_stt;
                                    return true;
                                }
                                if (!string.IsNullOrEmpty(v.Ten) && v.Ten.Contains(f, StringComparison.OrdinalIgnoreCase))
                                {
                                    v.STT = ++_stt;
                                    return true;
                                }
                            }
                        }
                        return false;
                    };
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime t = DateTime.Now;
            _t0 = t.Ticks / 10000000d;
            LblTime.Text = t.ToString("HH:mm:ss.fff");

            if (TramTron != null) ListVars(TramTron);
        }

        private void ListVars(ModelHeThong tt)
        {
            int i = 0;
            if (Observer != null)
            {
                foreach (var (k, l) in tt.AllLinks)
                {
                    LstVars.Add(new PlcVar(l.Tag)
                    {
                        STT = ++i,
                        Ten = k,
                        DiaChi = _utils.GetS71200Addr(l.S71200Db, l.S71200Tag),
                        TheoDoi = Observer.Lines.ContainsKey(k),
                    });
                }
                LblObserved.Text = Observer.Lines.Count.ToString();
            }
            else
            {
                foreach (var (k, l) in tt.AllLinks)
                {
                    LstVars.Add(new PlcVar(l.Tag)
                    {
                        STT = ++i,
                        Ten = k,
                        DiaChi = _utils.GetS71200Addr(l.S71200Db, l.S71200Tag),
                    });
                }
            }
        }

        private void BtUpdate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in LstVars)
            {
                v.UpdateFromTag(_t0);
            }
        }
        private void BtSetObserver_Click(object sender, RoutedEventArgs e)
        {
            if (Observer != null)
            {
                foreach (var v in LstVars)
                {
                    if (!string.IsNullOrEmpty(v.Ten))
                    {
                        if (v.TheoDoi)
                        {
                            Observer.AddLine(v.Ten);
                        }
                        else
                        {
                            Observer.RemoveLine(v.Ten);
                        }
                    }
                }
                LblObserved.Text = Observer.Lines.Count.ToString();
            }
        }

        private void TxtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _filter.Run(TxtFilter.Text);
        }

        private void BtClearFilter_Click(object sender, RoutedEventArgs e)
        {
            TxtFilter.Text = "";
        }

        private void BtCopy_Click(object sender, RoutedEventArgs e)
        {
            CopySelectedVars();
        }

        private void LvTags_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CopySelectedVars();
                e.Handled = true;
            }
        }

        private void CopySelectedVars()
        {
            if (LvTags.SelectedItems.Count == 0)
                return;

            var sb = new StringBuilder();

            foreach (PlcVar v in LvTags.SelectedItems)
            {
                sb.AppendLine($"{v.STT}\t{v.Ten}\t{v.DiaChi}");
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}
