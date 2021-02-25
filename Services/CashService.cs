using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Clients;

namespace MailRuCupMiner.Services
{
    public class CashService
    {
        private Infrastructure _infrastructure;
        private IHttpClientFactory _httpClientFactory;
        private IClient _client;
        public CashService(Infrastructure infrastructure,IHttpClientFactory httpClientFactory)
        {
            _infrastructure = infrastructure;
            _httpClientFactory = httpClientFactory;
            _client = _infrastructure.TryCreateClient(httpClientFactory.CreateClient());
        }

        //public async Task Cash(IEnumerable<int> cash)
        //{
        //    await _client.CashAsync(cash);
        //}
    }
}
