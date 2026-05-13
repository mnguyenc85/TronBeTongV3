using S7.Net;

namespace NMComm.S71200
{
    public abstract class S71200Communicator
    {
        protected Plc? _plc;
        private Thread? _communicationThread;
        private long _t0;
        protected CancellationTokenSource? _cancelSource;

        public event EventHandler? StateChanged;
        private bool _newIPAddr = false;                    // Khi địa chỉ IP hoặc Port thay đổi
        private string? _ipAddr;
        /// <summary>
        /// Địa chỉ IP của PLC
        /// </summary>
        public string? IPAddress { get { return _ipAddr; } set { if (_ipAddr != value) { _ipAddr = value; _newIPAddr = true; } } }
        private int _ipPort;
        public int IPPort { get { return _ipPort; } set { if (_ipPort != value) { _ipPort = value; _newIPAddr = true; } } }
        /// <summary>
        /// Trạng thái kết nối
        /// </summary>
        public CommStates State { get; private set; } = CommStates.Closed;
        /// <summary>
        /// Thời gian đợi đến khi bắt đầu kết nối (ms)<br/>
        /// Sử dụng: khi mất kết nối, ko muốn liên tục kết nối lại
        /// </summary>
        public double DelayConnect { get; set; }

        /// <summary>
        /// Lưu các thông báo lỗi
        /// </summary>
        public List<string> Messages { get; private set; } = new();

        /// <summary>
        /// Thời gian kết nối (ticks)
        /// </summary>
        public long CommTime { get; private set; }
        /// <summary>
        /// Chu kỳ mỗi vòng lặp (ticks)
        /// </summary>
        public long CycleTime { get; set; } = 100000;
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Chu kỳ thực mỗi vòng lặp (s)
        /// </summary>
        public double RealCycleTime { get; protected set; }

        public void Start()
        {
            if (IsRunning) return; // Prevent multiple starts
            IsRunning = true;
            _communicationThread = new Thread(CommunicationLoop);
            //_communicationThread.IsBackground = true;
            _t0 = DateTime.Now.Ticks;
            _communicationThread.Start();
        }

        public void Stop() {
            _cancelSource?.Cancel();
            IsRunning = false;
            if (_communicationThread != null && _communicationThread.IsAlive)
            {
                _communicationThread.Join(); // Wait for the thread to finish
            }
            Messages.Clear();
        }

        protected async void CommunicationLoop()
        {
            _cancelSource?.Dispose();
            _cancelSource = new CancellationTokenSource();

            while (IsRunning)
            {
                long t = DateTime.Now.Ticks;
                RealCycleTime = (t - _t0) / 10000000d;
                _t0 = t;

                try
                {
                    if (_newIPAddr) Disconnect();
                    DelayConnect -= RealCycleTime;
                    if (DelayConnect > 0) continue;

                    await Connect(_cancelSource.Token);

                    if (IsRunning && _plc != null && _plc.IsConnected)
                    {
                        await Comm(_plc, RealCycleTime, _cancelSource.Token);
                    }

                    CommTime = DateTime.Now.Ticks - _t0;
                    if (CommTime < CycleTime)
                    {
                        int delay = (int)((CycleTime - CommTime) / 10000);
                        //Thread.Sleep(delay);
                        await Task.Delay(delay, _cancelSource.Token);
                    }
                }
                catch (Exception ex)
                {
                    //Messages.Add(ex.ToString());
                    System.Diagnostics.Debug.WriteLine(ex.Message);

                    // Nghỉ ít nhất 10s mới có thể kết nối lại
                    DelayConnect = 10 + (DateTime.Now.Ticks - _t0) / 10000000d;
                }
            }

            Disconnect();
        }

        private async Task Connect(CancellationToken token)
        {
            _newIPAddr = false;
            if (_plc != null && _plc.IsConnected) return;

            if (string.IsNullOrEmpty(IPAddress))
            {
                State = CommStates.Closed;
                StateChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            _plc = new(CpuType.S71200, IPAddress, IPPort, 0, 1);

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Ket noi den {_plc.IP}:{_plc.Port}");

            State = CommStates.Openning;
            StateChanged?.Invoke(this, EventArgs.Empty);

            await _plc.OpenAsync(token);

            if (_plc.IsConnected)
            {
                State = CommStates.Opened;
            }
            else
            {
                State = CommStates.Closed;
            }
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Disconnect()
        {
            _newIPAddr = false;
            if (_plc != null && _plc.IsConnected)
            {
                _plc.Close();
                System.Diagnostics.Debug.WriteLine("_plc.Close()");
            }

            if (State != CommStates.Closed)
            {
                State = CommStates.Closed;
                StateChanged?.Invoke(this, EventArgs.Empty);
                DelayConnect = 0;
            }

            DelayConnect = 0;
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} S71200Comm.Disconnect");
        }

        protected abstract Task Comm(Plc plc, double delta, CancellationToken token);
    }
}
