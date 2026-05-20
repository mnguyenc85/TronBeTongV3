using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TronBeTongV3.CSDL.Server
{
    public class ServerComm
    {
        public const string SoftwareName = "TramTronBeTong";

        private string? _addr;
        public long FactoryId { get; private set; } = -1;

        /// <summary>
        /// Kết nối qua user, password để lấy connstr
        /// </summary>
        public async Task<string?> Connect(string addr, string username, string password)
        {
            _addr = addr;
            string url = addr.StartsWith("http")? $"{addr}:10001/bttt/login": $"http://{addr}:10001/bttt/login";
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

                return csdlparams;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
