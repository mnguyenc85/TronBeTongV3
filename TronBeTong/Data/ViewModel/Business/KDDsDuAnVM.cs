using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using NMWPFControls.Core;
using TronBeTongV3.CSDL;
using TronBeTongV3.Data.DO.Business;
using TronBeTongV3.Reports;

namespace TronBeTongV3.Data.ViewModel
{
    /// <summary>
    /// TODO: ghép vào DbRepository?
    /// </summary>
    public class KDDsDuAnVM
    {
        private DbBridge _db = DbBridge.Instance;

        public DelayUpdate<string> Filter;
        public ObservableCollection<KDDuAnVM> DsDA { get; set; }
        private readonly CollectionViewSource _cvsDsDA = new();
        public ICollectionView CVDsDA { get { return _cvsDsDA.View; } }

        public ObservableCollection<StringDO> DsKDStrings { get; set; }
        private readonly CollectionViewSource _cvsStrDuAn = new();
        public ICollectionView CVStrDuAn { get { return _cvsStrDuAn.View; } }
        private readonly CollectionViewSource _cvsStrCongTrinh = new();
        public ICollectionView CVStrCongTrinh { get { return _cvsStrCongTrinh.View; } }
        private readonly CollectionViewSource _cvsStrHangMuc = new();
        public ICollectionView CVStrHangMuc { get { return _cvsStrHangMuc.View; } }
        private readonly CollectionViewSource _cvsStrDiaChi = new();
        public ICollectionView CVStrDiaChi { get { return _cvsStrDiaChi.View; } }

        public KDDsDuAnVM()
        {
            DsDA = DbRepository.Instance.DsDA;
            DsKDStrings = DbRepository.Instance.DsPmStrings;

            _cvsStrDuAn.Source = DsKDStrings;
            CVStrDuAn.Filter = x =>
            {
                if (x is StringDO s)
                    if (s.PhanLoai == 1) return true;
                return false;
            };
            _cvsStrCongTrinh.Source = DsKDStrings;
            CVStrCongTrinh.Filter = x =>
            {
                if (x is StringDO s)
                    if (s.PhanLoai == 2) return true;
                return false;
            };
            _cvsStrHangMuc.Source = DsKDStrings;
            CVStrHangMuc.Filter = x =>
            {
                if (x is StringDO s)
                    if (s.PhanLoai == 3) return true;
                return false;
            };
            _cvsStrDiaChi.Source = DsKDStrings;
            CVStrDiaChi.Filter = x =>
            {
                if (x is StringDO s)
                    if (s.PhanLoai == 4) return true;
                return false;
            };

            _cvsDsDA.Source = DsDA;
            Filter = new(300)
            {
                ExecUpdate = f =>
                {
                    CVDsDA.Filter = x =>
                    {
                        if (x is KDDuAnVM d)
                        {
                            if (string.IsNullOrEmpty(f))
                            {
                                return true;
                            }
                            if ((!string.IsNullOrEmpty(d.DuAn) && d.DuAn.Contains(f, StringComparison.CurrentCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(d.CongTrinh) && d.CongTrinh.Contains(f, StringComparison.CurrentCultureIgnoreCase)) ||
                                (!string.IsNullOrEmpty(d.HangMuc) && d.HangMuc.Contains(f, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                }
            };

            int stt = 0;
            foreach (var da in DsDA)
            {
                da.STT = ++stt;
            }
        }

        public async Task Save(KDDuAnVM da)
        {
            var conn = await _db.OpenConnAsync();

            var o = da.CreateDO();

            await _db.KD_DuAn_SaveProcAsync(conn, o);
            
            if (da.Id > 0)
            {
                var d1 = DsDA.FirstOrDefault(x => x.Id == da.Id);
                d1?.CopyFrom(da);
            }
            else
            {
                da.Id = o.Id;
                KDDuAnVM da1 = new KDDuAnVM();
                da1.CopyFrom(da);
                da1.STT = DsDA.Count + 1;
                DsDA.Add(da1);
            }
            da.IsChanged = false;

            await _db.CloseConnAsync(conn);
        }

        public void ExportCsv(string filename, IEnumerable<KDKhachHangVM> DsKH)
        {
            foreach (var da in DsDA)
            {
                var kh = DsKH.FirstOrDefault(x => x.Id == da.KHId);
                da.KHMa = kh?.Ma;
            }
            CsvExporter.Export_DuAn(filename, DsDA.ToList());
        }

        public async Task ImportCsv(IEnumerable<KDKhachHangVM> DsKH)
        {
            Ookii.Dialogs.Wpf.VistaOpenFileDialog ofd = new()
            {
                Filter = "CSV files|*.csv"
            };
            if (ofd.ShowDialog() == true)
            {
                var lst = CsvExporter.Import_DuAn(ofd.FileName);
                foreach (var da in lst)
                {
                    var da1 = DsDA.FirstOrDefault(x => (x.DuAn == da.DuAn && x.HangMuc == da.HangMuc && x.CongTrinh == da.CongTrinh));

                    if (da1 == null)
                    {
                        if (da.KHMa != null)
                        {
                            var kh1 = DsKH.FirstOrDefault(x => x.Ma == da.KHMa);
                            if (kh1 != null) da.KHId = kh1.Id;
                            await Save(da);
                        }
                    }
                }
            }
        }

    }
}
