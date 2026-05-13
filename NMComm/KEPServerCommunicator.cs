using System;
using System.IO;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace NMComm
{
    public class KEPServerCommunicator
    {
        private string endpointUrl = "opc.tcp://127.0.0.1:49320";

        private ApplicationConfiguration? _config;
        private ConfiguredEndpoint? _endpoint;
        private Session? _session;

        public CommStates State { get; set; }
        public DeviceConnectionInfo ConnInfo { get; set; } = new DeviceConnectionInfo();
        public List<NodeId> NodeIds { get; private set; } = [];
        private Dictionary<string, string> NodeIdNames = [];

        private CancellationTokenSource? _cancellationTokenSource;

        public bool Running { get; private set; }
        /// <summary>
        /// Số lần đọc bị bỏ qua. (Sau khi đọc quá nhiều bad tags thì giảm tần số đọc)
        /// </summary>
        public int SkipRead { get; set; } = 0;

        public KEPServerCommunicator()
        {
        }

        public async Task<bool> Start()
        {
            if (_endpoint == null || _config == null)
            {
                await Initialize();
            }

            if (_endpoint != null && _config != null)
            {
                _session = await Session.Create(
                    _config, _endpoint, false,
                    "MySession", 60000,
                    new UserIdentity(new AnonymousIdentityToken()), null);

                _cancellationTokenSource = new CancellationTokenSource();

                State = CommStates.Opened;
                Running = true;
                return true;
            }
            return false;
        }

        public async Task Stop()
        {
            if (_session != null)
            {
                _cancellationTokenSource?.Cancel();
                while (_isReadingTags) await Task.Delay(1000);

                _session.Close();
                State = CommStates.Closed;
                ConnInfo.State = DeviceConnStates.None;
                Running = false;
            }
        }

        public void CreateNodes(string path, List<string> tagnames)
        {
            NodeIds.Clear();
            NodeIdNames.Clear();
            foreach (var s in tagnames)
            {
                string id = $"ns=2;s={path}.{s}";
                NodeIds.Add(new NodeId(id));
                NodeIdNames.Add(id, s);
            }
        }

        private bool _isReadingTags = false;
        public async Task<Dictionary<string, KEPServerReadValue>> ReadTags()
        {
            Dictionary<string, KEPServerReadValue> tags = [];

            if (_session == null || _cancellationTokenSource == null || _isReadingTags) return tags;
            if (SkipRead > 0) { SkipRead--; return tags; }

            _isReadingTags = true;
            try
            {
                int nobad = 0;
                var (results, errors) = await _session.ReadValuesAsync(NodeIds, _cancellationTokenSource.Token);
                if (results != null && errors != null)
                {
                    for (int i = 0; i < NodeIds.Count; i++)
                    {
                        string id = NodeIds[i].ToString();
                        if (ServiceResult.IsGood(errors[i]))
                        {
                            tags.Add(NodeIdNames[id], new KEPServerReadValue(results[i].Value));
                        }
                        else
                        {
                            //tags.Add(NodeIdNames[id], new KEPServerReadValue());
                            //System.Diagnostics.Debug.WriteLine($"{NodeIds[i]} lỗi: {errors[i]}");
                            nobad++;
                        }
                    }
                }

                ConnInfo.NoBadTags = nobad;
                if (nobad > 0)
                {
                    if (nobad > NodeIds.Count / 2) SkipRead = 20;
                    if (nobad == NodeIds.Count) ConnInfo.State= DeviceConnStates.Disconnect;
                    else ConnInfo.State = DeviceConnStates.Bad;
                }
                else
                {
                    ConnInfo.State = DeviceConnStates.Good;                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            _isReadingTags = false;

            return tags;
        }

        public async void WriteTags(string path, Dictionary<string, object> tags, double delay = 0)
        {
            if (_session == null || _cancellationTokenSource == null) return;

            if (delay > 0) await Task.Delay((int)(delay * 1000), _cancellationTokenSource.Token);

            var nodesToWrite = new WriteValueCollection();
            foreach (var (tag, v) in tags)
            {
                string id = $"ns=2;s={path}.{tag}";
                nodesToWrite.Add(new WriteValue()
                {
                    NodeId = new NodeId(id),
                    AttributeId = Attributes.Value,
                    Value = new DataValue() { Value = v }
                });
            }

            await _session.WriteAsync(null, nodesToWrite, _cancellationTokenSource.Token);
            //var writeResponses = await _session.WriteAsync(null, nodesToWrite, _cancellationTokenSource.Token);
        }

        public async Task WriteTag(string path, string tag, object v, double delay = 0)
        {
            if (_session == null || _cancellationTokenSource == null) return;

            if (delay > 0) await Task.Delay((int)(delay * 1000), _cancellationTokenSource.Token);

            var nodesToWrite = new WriteValueCollection();
            var wtag = new WriteValue()
            {
                NodeId = new NodeId($"ns=2;s={path}.{tag}"),
                AttributeId = Attributes.Value,
                Value = new DataValue() { Value = v }
            };
            nodesToWrite.Add(wtag);

            await _session.WriteAsync(null, nodesToWrite, _cancellationTokenSource.Token);
            //var writeResponses = await _session.WriteAsync(null, nodesToWrite, _cancellationTokenSource.Token);

            System.Diagnostics.Debug.WriteLine($"Write to {wtag.NodeId} = {wtag.Value}");
        }

        private async Task Initialize()
        {
            try
            {
                _config = new ApplicationConfiguration()
                {
                    ApplicationName = "TronBeTong",
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        AutoAcceptUntrustedCertificates = true,
                        ApplicationCertificate = new CertificateIdentifier(),
                        // Cấu hình các store path rỗng để khỏi báo lỗi
                        TrustedIssuerCertificates = new CertificateTrustList { StoreType = "Directory", StorePath = "OPC Foundation/CertificateStores/UA Certificate Authorities" },
                        TrustedPeerCertificates = new CertificateTrustList { StoreType = "Directory", StorePath = "OPC Foundation/CertificateStores/UA Applications" },
                        RejectedCertificateStore = new CertificateTrustList { StoreType = "Directory", StorePath = "OPC Foundation/CertificateStores/RejectedCertificates" }
                    },
                    TransportQuotas = new TransportQuotas(),
                    ClientConfiguration = new ClientConfiguration()
                };
                await _config.Validate(ApplicationType.Client);

                var selectedEndpoint = CoreClientUtils.SelectEndpoint(_config, endpointUrl, false);
                _endpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(_config));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _endpoint = null;
                _config = null;
            }
        }

    }
}
