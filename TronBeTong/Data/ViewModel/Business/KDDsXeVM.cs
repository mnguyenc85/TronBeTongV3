using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using NMWPFControls.Core;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.Data.ViewModel
{
    /// <summary>
    /// TODO: ghép vào DbRepository?
    /// </summary>
    public class KDDsXeVM
    {
        private DbBridge _db = DbBridge.Instance;
        private DbRepository _r = DbRepository.Instance;

        public DelayUpdate<string> Filter;
        private int _stt = 0;
        public ObservableCollection<KDXeLaiXeVM> DsXLx { get; private set; }
        private readonly CollectionViewSource _cvsDsXLx = new();
        public ICollectionView CVDsXLx { get { return _cvsDsXLx.View; } }

        public KDDsXeVM()
        {
            DsXLx = _r.DsXLx;
            _cvsDsXLx.Source = DsXLx;
            Filter = new(300)
            {
                ExecUpdate = f =>
                {
                    _stt = 0;
                    CVDsXLx.Filter = x =>
                    {
                        if (x is KDXeLaiXeVM xlx)
                        {
                            if (string.IsNullOrEmpty(f))
                            {
                                xlx.STT = ++_stt;
                                return true;
                            }
                        }
                        return false;
                    };
                }
            };
        }

        public async Task Save(KDXeVM xe, KDLaiXeVM lx, bool checkXe = true, bool checkLaiXe = false)
        {
            if (xe.IsChanged && !string.IsNullOrEmpty(xe.BSX))
            {
                // Đảm bảo biển số xe luôn chữ hoa
                xe.BSX = xe.BSX.ToUpper();

                // Đảm bảo chỉ có 1 biển số xe duy nhất
                KDXeVM? xe1 = null;
                if (checkXe) xe1 = _r.DsXe.FirstOrDefault(x => x.BSX == xe.BSX);
                
                if (xe1 != null)
                {
                    xe = xe1;
                }
                else
                {
                    var xedo = xe.CreateDO();
                    await _db.Xe_SaveAsync(xedo);
                    xe.Id = xedo.Id;
                    xe.IsChanged = false;
                }
            }

            if (lx.IsChanged && !string.IsNullOrEmpty(lx.Ten))
            {
                KDLaiXeVM? lx1 = null;
                if (checkLaiXe) lx1 = _r.DsLaiXe.FirstOrDefault(x => x.Ten == lx.Ten);                    

                if (lx1 == null)
                {
                    var lxdo = lx.CreateDO();
                    await _db.LaiXeSaveAsync(lxdo);
                    lx.Id = lxdo.Id;
                    lx.IsChanged = false;
                }
                else
                {
                    lx = lx1;
                }
            }

            await _r.AddXeLaiXe(xe, lx);
        }
    }
}
