using NMWPFControls.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMWPFControls.Controls
{
    /// <summary>
    /// Interaction logic for NMDateTimeVn.xaml
    /// </summary>
    public partial class NMDateTimeVn : UserControl
    {
        private int maxDay = 31;
        private bool _showTime = true;
        private bool _showSec = true;

        /// <summary>
        /// (Custom) Show second
        /// </summary>
        public bool ShowSecond
        {
            get { return _showSec; }
            set
            {
                if (_showSec != value)
                {
                    _showSec = value;
                    if (_showSec)
                    {
                        pnlSecond.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        pnlSecond.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// (Custom) Show time
        /// </summary>
        public bool ShowTime
        {
            get { return _showTime; }
            set
            {
                if (_showTime != value)
                {
                    _showTime = value;
                    if (_showTime)
                    {
                        pnlTime.Visibility = Visibility.Visible;
                        btNow.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        pnlTime.Visibility = Visibility.Collapsed;
                        btNow.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        /// <summary>
        /// (Custom)
        /// </summary>
        public DateTime Value
        {
            get { return new DateTime(txtYear.Value, txtMon.Value, txtDay.Value, txtHour.Value, txtMin.Value, txtSec.Value); }
            set
            {
                txtYear.Value = value.Year;
                txtMon.Value = value.Month;
                txtDay.Value = value.Day;
                txtHour.Value = value.Hour;
                txtMin.Value = value.Minute;
                txtSec.Value = value.Second;
            }
        }

        public event EventHandler<DateTime>? ValueChanged;

        public NMDateTimeVn()
        {
            InitializeComponent();

            DateTime now = DateTime.Now;
            txtDay.Value = now.Day;
            txtMon.Value = now.Month;
            txtYear.Value = now.Year;

            txtDay.NextFocus += ToNextField;
            txtMon.NextFocus += ToNextField;
            txtYear.NextFocus += ToNextField;
            txtHour.NextFocus += ToNextField;
            txtMin.NextFocus += ToNextField;

            txtSec.PrevFocus += ToPrevField;
            txtMin.PrevFocus += ToPrevField;
            txtHour.PrevFocus += ToPrevField;
            txtYear.PrevFocus += ToPrevField;
            txtMon.PrevFocus += ToPrevField;
        }

        private void ToPrevField(object? sender, EventArgs e)
        {
            if (sender == txtSec)
            {
                txtMin.Focus();
                txtMin.CaretIndex = txtMin.Text.Length;
            }
            else if (sender == txtMin)
            {
                txtHour.Focus();
                txtHour.CaretIndex = txtHour.Text.Length;
            }
            else if (sender == txtHour)
            {
                txtYear.Focus();
                txtYear.CaretIndex = txtYear.Text.Length;
            }
            else if (sender == txtYear)
            {
                txtMon.Focus();
                txtMon.CaretIndex = txtMon.Text.Length;
            }
            else if (sender == txtMon)
            {
                txtDay.Focus();
                txtDay.CaretIndex = txtDay.Text.Length;
            }
        }

        private void ToNextField(object? sender, EventArgs e)
        {
            if (sender == txtDay)
            {
                txtMon.Focus();
                txtMon.CaretIndex = 0;
            }
            else if (sender == txtMon)
            {
                txtYear.Focus();
                txtYear.CaretIndex = 0;
            }
            else if (sender == txtYear && _showTime)
            {
                txtHour.Focus();
                txtHour.CaretIndex = 0;
            }
            else if (sender == txtHour)
            {
                txtMin.Focus();
                txtMin.CaretIndex = 0;
            }
            else if (sender == txtMin)
            {
                txtSec.Focus();
                txtSec.CaretIndex = 0;
            }    
        }

        public void setDateTime(int y, int m, int d, int h, int min)
        {
            txtYear.Value = y;
            txtMon.Value = m;
            txtDay.Value = d;
            txtHour.Value = h;
            txtMin.Value = min;
        }

        private void txtSec_ValueChanged(object sender, int e)
        {
            if (e > 59)
            {
                txtSec.Value = 0;
                txtMin.Value += 1;
            }
            else if (e < 0)
            {
                txtSec.Value = 59;
                txtMin.Value -= 1;
            }
            ValueChanged?.Invoke(this, Value);
        }

        private void txtMin_ValueChanged(object sender, int e)
        {
            if (e > 59)
            {
                txtMin.Value = 0;
                txtHour.Value += 1;
            }
            else if (e < 0)
            {
                txtMin.Value = 59;
                txtHour.Value -= 1;
            }
            ValueChanged?.Invoke(this, Value);
        }

        private void txtHour_ValueChanged(object sender, int e)
        {
            if (e > 23)
            {
                txtHour.Value = 0;
                txtDay.Value += 1;
            }
            else if (e < 0)
            {
                txtHour.Value = 23;
                txtDay.Value -= 1;
            }
            ValueChanged?.Invoke(this, Value);
        }

        private void txtDay_ValueChanged(object sender, int e)
        {
            if (e < 1)
            {
                txtMon.Value -= 1;
                txtDay.Value = maxDay;
            }
            else if (e > maxDay)
            {
                txtMon.Value += 1;
                txtDay.Value = 1;
            }
            ValueChanged?.Invoke(this, Value);
        }

        private void txtMon_ValueChanged(object sender, int e)
        {
            maxDay = CalendarUtils.GetDaysInMonth(txtMon.Value, txtYear.Value);
            if (e > 12)
            {
                txtMon.Value = 1;
                txtYear.Value += 1;
            }
            else if (e < 1)
            {
                txtMon.Value = 12;
                txtYear.Value -= 1;
            }
            ValueChanged?.Invoke(this, Value);
        }

        private void txtYear_ValueChanged(object sender, int e)
        {
            if (e > 9999) txtYear.Value = 9999;
            else if (e < 2000) txtYear.Value = 2000;
            maxDay = CalendarUtils.GetDaysInMonth(txtMon.Value, txtYear.Value);
            ValueChanged?.Invoke(this, Value);
        }

        private void lich1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lich1.SelectedDate != null)
            {
                txtYear.Value = lich1.SelectedDate.Value.Year;
                txtMon.Value = lich1.SelectedDate.Value.Month;
                txtDay.Value = lich1.SelectedDate.Value.Day;
            }
            Popup1.IsOpen = false;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            //btShowCalendar.IsChecked = false;
        }

        private void Popup1_Opened(object sender, EventArgs e)
        {
        }

        private void BtToday_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            lich1.DisplayDate = dt;
            lich1.SelectedDate = dt;
            txtHour.Value = 0;
            txtMin.Value = 0;
        }

        private void btShowCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (Popup1.IsOpen != true)
            {
                DateTime dt = new DateTime(txtYear.Value, txtMon.Value, txtDay.Value, txtHour.Value, txtMin.Value, 0);
                lich1.DisplayDate = dt;
                lich1.SelectedDate = dt;
                Popup1.IsOpen = true;
            }
            else
            {
                Popup1.IsOpen = false;
            }
        }

        private void BtNow_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            lich1.DisplayDate = dt;
            lich1.SelectedDate = dt;
            txtHour.Value = dt.Hour;
            txtMin.Value = dt.Minute;
            Popup1 .IsOpen = false;
        }

        private void pnl1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (pnl1.IsEnabled == true) pnl1.Background = Brushes.White;
            else
            {
                pnl1.Background = Brushes.WhiteSmoke;
            }
        }
    }
}
