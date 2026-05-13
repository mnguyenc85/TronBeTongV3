using System;
using System.Net.Sockets;
using System.Text;

namespace TronBeTongV3.CSDL.Server
{
    public class OnlineMonitorClient
    {
        private DbRepository _r = DbRepository.Instance;

        private bool _running = false;
        private string? _serverAddr;
        private string? _dbName;
        private int _id;
        private string? _name;

        public int SrvStt { get; set; } = -1;
        public bool TramKetNoiPLC { get; set; }
        public bool TramCoiTron { get; set; }
        public bool TramDangTron { get; set; }

        public OnlineMonitorClient()
        {
        }

        public void Init()
        {
            _serverAddr = _r.Settings.GetValue("srv.ip");
            _dbName = _r.Settings.GetValue("srv.db");
            _name = _r.Settings.GetValue("srv.tram.id");
        }

        public void Start(int id)
        {
            if (_running) return;

            _id = id;

            _running = true;
            Task.Run(async () =>
            {
                try
                {
                    SrvRegister();
                    while (_running)
                    {
                        SrvStatus();

                        await Task.Delay(5000);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi trạng thái lên server: {ex.Message}");
                }
                _running = false;
            });
        }

        public void Stop()
        {
            _running = false;
        }

        private void SrvRegister()
        {
            if (!string.IsNullOrEmpty(_serverAddr))
            {
                using (TcpClient client = new TcpClient(_serverAddr, 20258))
                using (NetworkStream stream = client.GetStream())
                {
                    string message = $"reg,{_dbName},{_id},{_name}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    // Nhận phản hồi
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] ss = response.Split(',');
                    if (ss.Length > 1)
                    {
                        if (ss[0] == "reg")
                        {
                            if (int.TryParse(ss[1], out int stt)) { SrvStt = stt; }
                            System.Diagnostics.Debug.WriteLine($"Đăng ký {SrvStt}");
                        }
                    }
                }
            }
        }
        private void SrvStatus()
        {
            if (!string.IsNullOrEmpty(_serverAddr))
            {
                using (TcpClient client = new TcpClient(_serverAddr, 20258))
                using (NetworkStream stream = client.GetStream())
                {
                    string message = $"status,{_dbName},{_id},{(TramKetNoiPLC? "1": "0")},{(TramCoiTron? "1": "0")},{(TramDangTron? "1": "0")}";
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);

                    //System.Diagnostics.Debug.WriteLine($"Gửi bản tin {message}");
                }
            }
        }
    }
}
