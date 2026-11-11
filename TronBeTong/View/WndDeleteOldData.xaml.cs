using Accessibility;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TronBeTongV3.Comm.S71200;
using TronBeTongV3.CSDL;

namespace TronBeTongV3.View
{
    /// <summary>
    /// Interaction logic for WndDeleteOldData.xaml
    /// </summary>
    public partial class WndDeleteOldData : Window
    {
        public class CTKDuLieu
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public double M3 { get; set; }
            public long TongDon { get; set; }
            public long TongPhieu { get; set; }
            public long TongMe { get; set; }

            public override string ToString()
            {
                return $"{Year}/{Month}";
            }
        }

        private DbBridge _db = DbBridge.Instance;

        public ObservableCollection<CTKDuLieu> TKDuLieu { get; set; } = [];

        public WndDeleteOldData()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BtThongKe_Click(object sender, RoutedEventArgs e)
        {
            ThongKe();
        }

        private async void ThongKe()
        {
            TKDuLieu.Clear();

            int y = 1, m = 1;
            // Đọc đơn đâu tiên
            var dh0 = await _db.HT_DonHang_GetFirstAsync();
            if (dh0 != null)
            {
                y = dh0.CreatedAt.Year;
                m = dh0.CreatedAt.Month;
                int y1 = DateTime.Now.Year;
                int m1 = DateTime.Now.Month;
                while (y < y1 || (y == y1 && m <= m1))
                {
                    var tongDh = await _db.HT_DonHang_CountByMonthAsync(y, m);
                    var tongPhieu = await _db.HT_Phieu_CountByMonthAsync(y, m);
                    var tongMe = await _db.HT_Me_CountByMonthAsync(y, m);

                    TKDuLieu.Add(new CTKDuLieu()
                    {
                        Year = y,
                        Month = m,
                        TongDon = tongDh,
                        TongPhieu = tongPhieu,
                        TongMe = tongMe
                    });

                    m++; if (m > 12) { m = 1; y++; }
                }

                CboDeletable.Items.Clear();
                for (int i = 0; i < TKDuLieu.Count - 1; i++)
                {
                    CboDeletable.Items.Add(TKDuLieu[i]);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Bắt đầu thống kê từ: {y}-{m}: Không có dữ liệu");
            }

            BtDelete.IsEnabled = true;
        }

        private async void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CboDeletable.SelectedItem is CTKDuLieu k)
            {
                DateTime tg1 = new(k.Year, k.Month, 1);
                int month = k.Month++;
                int year = k.Year;
                if (month > 12) { month = 1; year++; }
                DateTime tg2 = new(k.Year, k.Month, 1);

                await _db.HT_Me_DeleteAsync(tg1, tg2);
                await _db.HT_Phieu_DeleteAsync(tg1, tg2);
                if (ChkDeleteDonHang.IsChecked == true) await _db.HT_DonHang_DeleteAsync(tg1, tg2);

                BtDelete.IsEnabled = false;
            }
        }
    }
}
