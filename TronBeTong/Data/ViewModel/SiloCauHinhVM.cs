using NMWPFControls.Core.MVVM;
using TronBeTongV3.Core;
using TronBeTongV3.Data.DO;

namespace TronBeTongV3.Data.ViewModel
{

    public class SiloCauHinhVM : VMBase
    {
        public int Id { get; set; }
        public int STT { get; set; }
        public string? Ten { get; set; }
        public string? Ma { get; set; }

        public LoaiThanhPhan PhanLoai { get; set; }
        /// <summary>
        /// Index theo phân loại
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Id trong tblsilonguyenlieu
        /// </summary>
        public int NLId { get; set; }
        /// <summary>
        /// Id trong tblsilothamso
        /// </summary>
        public int TSId { get; set; }

        private SiloNguyenLieuVM? _nl;
        public SiloNguyenLieuVM? NguyenLieu { get { return _nl; } set { if (_nl != value) { _nl = value; NotifyChanged(); } } }
        public SiloThamSoDO? ThamSo { get; set; }

        public int Flags { get; set; }

        public void ToDO(SiloCauHinhDO o)
        {
            o.Id = Id;
            o.PhanLoai = (int)PhanLoai;
            o.Index = Index;
            o.NLId = NguyenLieu != null? NguyenLieu.Id: -1;
            o.TSId = ThamSo != null? ThamSo.Id: -1;
        }

        public SiloCauHinhDO CreateDO()
        {
            var o = new SiloCauHinhDO();
            ToDO(o);
            return o;
        }
    }
}
