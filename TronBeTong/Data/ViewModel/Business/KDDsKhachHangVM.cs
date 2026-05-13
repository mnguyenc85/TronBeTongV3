using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using NMWPFControls.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Reports;

namespace TronBeTongV3.Data.ViewModel
{
    /// <summary>
    /// TODO: ghép vào DbRepository?
    /// </summary>
    public class KDDsKhachHangVM
    {
        private DbBridge _db = DbBridge.Instance;
        private DbRepository _r = DbRepository.Instance;

        public DelayUpdate<string> Filter;
        private int _stt = 0;
        private ObservableCollection<KDKhachHangVM> _dsKH;
        private CollectionViewSource _cvDsKH = new();
        public ICollectionView CVDsKH { get { return _cvDsKH.View; } }

        public KDDsKhachHangVM()
        {
            _dsKH = DbRepository.Instance.DsKH;
            _cvDsKH.Source = _dsKH;
            Filter = new(300)
            {
                ExecUpdate = f =>
                {
                    _stt = 0;
                    CVDsKH.Filter = x =>
                    {
                        if (x is KDKhachHangVM k)
                        {
                            if (string.IsNullOrEmpty(f))
                            {
                                k.STT = ++_stt;
                                return true;
                            }
                            if ((!string.IsNullOrEmpty(k.Ten) && k.Ten.Contains(f, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(k.Ma) && k.Ma.Contains(f, StringComparison.OrdinalIgnoreCase)))
                            {
                                k.STT = ++_stt;
                                return true;
                            }
                        }
                        return false;
                    };
                }
            };
        }

        public async Task Save(KDKhachHangVM k)
        {
            var o = k.CreateDO();
            await _db.KhachHangSaveAsync(o);
            if (k.Id > 0)
            {
                k.Id = o.Id;
                if (_r.DictionaryKH.TryGetValue(k.Id, out var k1)) {
                    k1.CopyFrom(k);
                }
            }
            else
            {
                k.Id = o.Id;
                var k1 = new KDKhachHangVM(o)
                {
                    STT = _r.DsKH.Count + 1
                };
                _r.AddKhachHang(k1);
            }
            k.IsChanged = false;
        }

        public void ExportCsv()
        {
            Ookii.Dialogs.Wpf.VistaSaveFileDialog svd = new()
            {
                Filter = "CSV files|*.csv"
            };
            if (svd.ShowDialog() == true)
            {
                CsvExporter.Export_KhachHang(svd.FileName, _dsKH.ToList());
            }
        }

        public async Task ImportCsv()
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog ofd = new()
            {
                Filter = "CSV files|*.csv"
            };
            if (ofd.ShowDialog() == true)
            {
                var lst = CsvExporter.Import_KhachHang(ofd.FileName);
                foreach (var kh in lst)
                {
                    var kh1 = _dsKH.FirstOrDefault(x => x.Ma == kh.Ma);
                    if (kh1 == null)
                    {
                        await Save(kh);
                    }
                }
            }
        }

        public KDKhachHangVM? FindKHById(int id)
        {
            return _dsKH.FirstOrDefault(x =>x.Id == id);
        }

        public KDKhachHangVM? FindKHByMa(string ma)
        {
            return _dsKH.FirstOrDefault(x => x.Ma == ma);
        }
        public KDKhachHangVM? FindKHByTen(string ten)
        {
            return _dsKH.FirstOrDefault(x => x.Ten == ten);
        }
    }
}
