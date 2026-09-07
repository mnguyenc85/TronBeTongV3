using Serilog;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TronBeTongV3.Debugger
{
    public static class CrashLogger
    {
        public static void Initialize()
        {
            string logDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "logs");

            Directory.CreateDirectory(logDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(logDir, "app-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();

            Application.Current.DispatcherUnhandledException +=
                Current_DispatcherUnhandledException;

            AppDomain.CurrentDomain.UnhandledException +=
                CurrentDomain_UnhandledException;

            TaskScheduler.UnobservedTaskException +=
                TaskScheduler_UnobservedTaskException;

            Log.Information("Application started");
        }

        public static void Shutdown()
        {
            Log.Information("Application stopped");
            Log.CloseAndFlush();
        }

        private static void Current_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Fatal(
                e.Exception,
                "Unhandled UI exception");

            Log.CloseAndFlush();
        }

        private static void CurrentDomain_UnhandledException(
            object? sender,
            UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(
                    ex,
                    "Unhandled AppDomain exception");
            }
            else
            {
                Log.Fatal(
                    "Unhandled AppDomain exception: {ExceptionObject}",
                    e.ExceptionObject);
            }

            Log.CloseAndFlush();
        }

        private static void TaskScheduler_UnobservedTaskException(
            object? sender,
            UnobservedTaskExceptionEventArgs e)
        {
            Log.Fatal(
                e.Exception,
                "Unobserved task exception");

            Log.CloseAndFlush();

            e.SetObserved();
        }
    }
}
