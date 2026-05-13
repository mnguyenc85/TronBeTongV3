using Serilog;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using TronBeTongV3.Core;

namespace TronBeTongV3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private static Mutex? _mutex;

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        protected override void OnStartup(StartupEventArgs e)
        {
            //AppLogger.Init(); // Khởi tạo logger
                              
            // Đăng ký handler ở đây
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.CloseAndFlush();            
            
            bool createdNew;
            _mutex = new Mutex(true, "NCM.TamPhatAn.TronBeTong", out createdNew);

            if (!createdNew)
            {
                // tìm cửa sổ theo title
                IntPtr hWnd = FindWindow(null, "Trạm trộn bê tông");
                if (hWnd != IntPtr.Zero)
                {
                    // Nếu đang minimize thì restore lại
                    if (IsIconic(hWnd))
                    {
                        ShowWindow(hWnd, 9); // SW_RESTORE = 9
                    }
                    SetForegroundWindow(hWnd);
                }
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppLogger.Shutdown(); // Flush log khi thoát
            base.OnExit(e);
        }

        public static IntPtr FindWindowByPartialTitle(string partialTitle)
        {
            IntPtr found = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd)) return true;

                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString();

                if (title.Contains(partialTitle, StringComparison.OrdinalIgnoreCase))
                {
                    found = hWnd;
                    return false; // dừng tìm
                }
                return true; // tiếp tục
            }, IntPtr.Zero);

            return found;
        }
    }
}
