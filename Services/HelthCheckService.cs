using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mainerspace;
using Serilog;

namespace MailRuCupMiner.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHelthCheckService
    {
        Task<bool> IsServerReady();
    }

    public class HelthCheckService : IHelthCheckService
    {
        private Client _client;
        private readonly Infrastructure _infr;
        private readonly IHttpClientFactory _http;
        public HelthCheckService(Infrastructure infrastructure,IHttpClientFactory httpClientFactory)
        {
            _infr = infrastructure;
            _http = httpClientFactory;
            _client = infrastructure.TryCreateClient( httpClientFactory.CreateClient());            
        }

        public async Task<bool> IsServerReady()
        {
            try
            {
                if (_client == null)
                {
                    Program.Logger.Error($"client is null!");
                    _client = _infr.TryCreateClient(_http.CreateClient());
                    return false;
                }

                var helthResultObject = await _client.HealthCheckAsync();

                var codePropInfo = helthResultObject.GetType().GetProperty("code");
                var codeValue = (string)(codePropInfo?.GetValue(helthResultObject, null));

                if (string.IsNullOrEmpty(codeValue))
                {
                    Program.Logger.Error($"{codeValue} is null! Return false");
                    return false;
                }

                if (codeValue.Equals("200"))
                {
                    Program.Logger.Error($"{codeValue} 200! Return true");
                    return true;
                }
                Program.Logger.Error($"{codeValue} not equals 200! Return false");
                return false;
            }
            catch (Exception e)
            {
                Program.Logger.Error(e,"Error in helth check service");
                return false;
            }
            
        }
    }
}
