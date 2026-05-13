using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NMWPFControls.Core.MVVM
{
    /// <summary>
    /// Class that implement INotifyPeropertyChanged.
    /// Using cache to reduce create new PropertyChangedEventArgs.
    /// </summary>
    public abstract class VMBase : INotifyPropertyChanged
    {
        private readonly Dictionary<string, PropertyChangedEventArgs> _argsCache = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyChanged([CallerMemberName] string? propertyName = null)
        {
            if (_argsCache != null && propertyName != null)
            {
                if (!_argsCache.ContainsKey(propertyName))
                    _argsCache[propertyName] = new PropertyChangedEventArgs(propertyName);

                NotifyChanged(_argsCache[propertyName]);
            }
        }

        private void NotifyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
