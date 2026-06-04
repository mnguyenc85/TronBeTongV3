using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TronBeTongV3.Core;

namespace TronBeTongV3.CSDL.Server
{
    public class ServerComm
    {
        private DbServerSync _srv = new();

        public const string SoftwareName = "TramTronBeTong";

        private readonly string[] _sync_tables = ["pm_strings", "kd_khachhang", "kd_laixe", "kd_xe", "ht_thanhphan", "ht_congthuc", "ht_congthuc_thanhphan", "ht_donhang", "ht_phieu", "ht_me"];
        private Dictionary<string, TableInfo> _tblInfos = [];

        private string? _addr;
        public long FactoryId { get; private set; } = -1;
        public bool IsServerOk { get; private set; }
        public bool IsLocalOk { get; private set; }

        /// <summary>
        /// Khởi tạo kết nối đến local
        /// </summary>
        public async Task InitLocal()
        {
            try
            {
                var lst = await _srv.Init(_sync_tables);
                _tblInfos.Clear();
                foreach (var tbl in lst)
                    _tblInfos[tbl.TableName] = tbl;

                IsLocalOk = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                IsLocalOk = false;
            }
        }

        /// <summary>
        /// Kết nối qua user, password để lấy connstr
        /// </summary>
        public async Task<string?> Connect(string addr, string username, string password)
        {
            _addr = addr;
            string url = addr.StartsWith("http") ? $"{addr}:10001/bttt/login" : $"http://{addr}:10001/bttt/login";
            HttpClient client = new();

            client.DefaultRequestHeaders.UserAgent.ParseAdd($"{SoftwareName}/1.0");
            var jsonStr = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
            var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                // Read response JSON
                var responseJson = await response.Content.ReadAsStringAsync();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var csdlparams = doc.RootElement
                              .GetProperty("account")
                              .GetProperty("csdl")
                              .GetString();

                System.Diagnostics.Debug.WriteLine($"{DateTime.Now:HH:mm:ss} Thông tin csdl: {csdlparams}");
                InitServerConnection(csdlparams, addr);

                return csdlparams;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private void InitServerConnection(string? csdlparams, string? host = null)
        {
            if (csdlparams == null) return;
            string[] ss = csdlparams.Split(';');
            if (host == null)
            {
                if (ss.Length > 3)
                {
                    _srv.CreateConnStr(ss[3], ss[0], ss[1], ss[2]);
                    IsServerOk = true;
                }
            }
            else
            {
                if (ss.Length > 2)
                {
                    _srv.CreateConnStr(host, ss[0], ss[1], ss[2]);
                    IsServerOk = true;
                }
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
                        await _srv.Server_Source_Init(tentram, ma, dbcreatedat);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        public async Task GetSourceId(string? tramten, string? tramma)
        {

            if (tramten == null || tramma == null) { FactoryId = 0; return; }
            try
            {
                string ma = GetMa(tramma);
                FactoryId = await _srv.Server_Get_Factory_Id(ma);
            }
            catch { }
        }

        private string GetMa(string matg)
        {
            string ma = $"{MyCopyright.GetPCFootPrint()}-{matg}";
            return MyCopyright.ComputeMD5(ma);
        }


        private bool _isSynchronizing = false;
        private bool _needSynchonize = false;
        public async Task StartSync()
        {
            if (!IsServerOk || FactoryId <= 0) { _needSynchonize = false; return; }

            _needSynchonize = true;
            if (_isSynchronizing) { return; }

            _isSynchronizing = true;
            try
            {
                while (_needSynchonize)
                {
                    _needSynchonize = false;
                    if (IsServerOk)
                    {
                        await _srv.Server_Sync(_tblInfos, FactoryId);
                    }
                }
            }
            catch
            {
                IsServerOk = false;
            }
            _isSynchronizing = false;
        }

    }
}
