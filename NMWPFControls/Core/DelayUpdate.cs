namespace NMWPFControls.Core
{
    /// <summary>
    /// Ngăn thực hiện ExecUpdate khi giá trị thay đổi liên tục.
    /// Mặc định chỉ thực hiện thay đổi khi giá trị ổn định 500ms
    /// </summary>
    public class DelayUpdate<T>
    {
        private int _delayTime; 
        private bool _isRunning = false;
        private bool _updateChanged = false;
        private T? _value;

        public delegate void DExecUpdate(T? value);
        // Con trỏ tới hàm thực hiện thay đổi
        public DExecUpdate? ExecUpdate { get; set; }

        public DelayUpdate(int delayTime = 500)
        {
            _delayTime = delayTime;
        }
        public DelayUpdate(DExecUpdate updater, int delayTime = 500)
        {
            _delayTime = delayTime;
            ExecUpdate = updater;
        }

        /// <summary>
        /// Chạy hàm ExecUpdate
        /// </summary>
        /// <param name="val">Giá trị đi kèm</param>
        /// <param name="noDelay">Thực hiện update ngay lập tức</param>
        public async void Run(T? val, bool noDelay = false)
        {
            if (noDelay) ExecUpdate?.Invoke(val);
            else
            {
                _value = val; _updateChanged = true;

                if (_isRunning) return;

                _isRunning = true;
                while (_updateChanged)
                {
                    _updateChanged = false;
                    await Task.Delay(_delayTime);
                }

                ExecUpdate?.Invoke(_value);
                _isRunning = false;
            }
        }
    }
}
