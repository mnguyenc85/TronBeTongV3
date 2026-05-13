using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManualPrintDocket.Printing;
using System.Windows;

namespace ManualPrintDocket.ViewModel
{
    public partial class MainWndVM: ObservableObject
    {
        private XuLyInPhieu _inphieu = XuLyInPhieu.Instance;

        [ObservableProperty]
        private string? _SelectedTemplate;

        public PhieuTronBeTongVM Phieu { get; set; } = new PhieuTronBeTongVM();

        [RelayCommand]
        public void InPhieu()
        {
            if (!ExecInPhieu())
            {
                MessageBox.Show("Lỗi khi in phiếu");
            }
        }

        private bool ExecInPhieu()
        {
            if (string.IsNullOrEmpty(_inphieu.ReportTemplate))
            {
                if (string.IsNullOrEmpty(SelectedTemplate)) return false;
                if (!_inphieu.LoadTemplate(SelectedTemplate))
                {
                    return false;
                }
            }



            return false;
        }
    }
}
