using Serilog;
using System;
using System.IO;

namespace TronBeTongV3.Core
{
    public static class AppLogger
    {
        public static void Init()
        {
            // Đường dẫn thư mục log
            string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);

            // Tên file dạng: logs/log-2025-08-06_01.log
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH-mm-ss");
            string logFile = Path.Combine(logDir, $"log-{date}_{time}.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFile,
                    rollingInterval: RollingInterval.Infinite,      // không chia nhỏ trong cùng file
                    retainedFileCountLimit: 10,                     // giữ lại 10 file gần nhất (tuỳ chọn)
                    shared: true)
                .CreateLogger();

            Log.Information("Logger initialized.");
        }

        public static void Shutdown()
        {
            Log.CloseAndFlush();
        }
    }
}
