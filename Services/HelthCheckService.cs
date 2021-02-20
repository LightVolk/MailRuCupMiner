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
    public interface IHelthCheckService
    {
        Task<bool> IsServerReady();
    }

    public class HelthCheckService : IHelthCheckService
    {
        private readonly Client _client;
        public HelthCheckService(Client client)
        {
            _client = client;
        }

        public async Task<bool> IsServerReady()
        {
            try
            {
                var helthResultObject = await _client.HealthCheckAsync();

                var codePropInfo = helthResultObject.GetType().GetProperty("code");
                var codeValue = (string)(codePropInfo?.GetValue(helthResultObject, null));

                if (string.IsNullOrEmpty(codeValue))
                {
                    return false;
                }

                if (codeValue.Equals("200")) return true;
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e,"Error in helth check service");
                return false;
            }
            
        }
    }
}
