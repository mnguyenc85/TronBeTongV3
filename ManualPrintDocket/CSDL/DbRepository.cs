using System.IO;

namespace ManualPrintDocket.CSDL
{
    public class DbRepository
    {
        public string AppPath;
        public string DataPath;
        public string ReportsPath;

        #region Singleton
        private static DbRepository? _instance;
        public static DbRepository Instance { get { return _instance ??= new DbRepository(); } }
        private DbRepository()
        {
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            DataPath = AppPath + "data\\";
            ReportsPath = AppPath + "reports\\";
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(ReportsPath)) Directory.CreateDirectory(ReportsPath);
        }
        #endregion
    }
}
