using System.ComponentModel;

namespace TronBeTongV3.Core.MVVM
{
    public class DoubleItem : INotifyPropertyChanged
    {
        private double _value;
        public double Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
