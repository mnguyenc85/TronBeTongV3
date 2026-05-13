using System.Threading.Tasks;
using TronBeTongV3.Core;

namespace TronBeTongV3.CSDL.Server
{
    public class SyncAgent
    {
        private ServerSync _srv = new ServerSync();

        public int SourceId { get; private set; } = -1;
        private string[] sync_tables = ["pm_strings", "kd_khachhang", "kd_laixe", "kd_xe", "ht_thanhphan", "ht_congthuc", "ht_congthuc_thanhphan", "ht_donhang", "ht_phieu", "ht_me"];
        private Dictionary<string, TableInfo> _tblInfos = [];

        public bool IsServerOk { get; private set; }
        public bool IsLocalOk { get; private set; }

        /// <summary>
        /// Khởi tạo kết nối đến local
        /// </summary>
        public async Task Init(string? srv, string? db, string? user, string? pw)
        {
            if (srv == null || db == null || user == null || pw == null)
            {
                IsLocalOk = false;
                return;
            }    

            try
            {
                var lst = await _srv.Init(srv, db, user, pw, sync_tables);
                _tblInfos.Clear();
                foreach (var tbl in lst)
                    _tblInfos[tbl.TableName] = tbl;

                IsLocalOk = true;
            }
            catch (Exception ex) { 
                System.Diagnostics.Debug.WriteLine(ex.Message);
                IsLocalOk = false;
            }
        }

        /// <summary>
        /// Khởi tạo csdl trên server
        /// </summary>
        public async Task InitServer(string? tentram = null, string? dbcreatedat = null)
        {
            if (IsLocalOk)
            {
                try
                {
                    await _srv.CreateServerTables(_tblInfos);
                    IsServerOk = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    IsServerOk = false;
                }

                try
                {
                    if (tentram != null && dbcreatedat != null)
                    {
                        string ma = GetMa(dbcreatedat);
                        await _srv.Server_Source_Init(tentram, ma);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task TestServerConnection()
        {
            IsServerOk = await _srv.TestConnection();
        }

        private bool _isSynchronizing = false;
        private bool _needSynchonize = false;
        public async Task StartSync()
        {
            if (!IsServerOk || SourceId <= 0) { _needSynchonize = false; return; }
            
            _needSynchonize = true;
            if (_isSynchronizing) { return;  }

            _isSynchronizing = true;
            try
            {
                while (_needSynchonize)
                {
                    _needSynchonize = false;
                    if (IsServerOk)
                    {
                        await _srv.Server_Sync(_tblInfos, SourceId);
                    }
                }
            }
            catch {
                IsServerOk = false;
            }
            _isSynchronizing = false;
        }

        public async Task GetSourceId(string? tramten, string? tramma)
        {

            if (tramten == null || tramma == null) { SourceId = 0; return; }
            try
            {
                string ma = GetMa(tramma);
                SourceId = await _srv.Server_Source_Init(tramten, ma);
            }
            catch { }
        }

        public string GetMa(string matg)
        {
            string ma = $"{MyCopyright.GetPCFootPrint()}-{matg}";
            return MyCopyright.ComputeMD5(ma);
        }
    }
}
