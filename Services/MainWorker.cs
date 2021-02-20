using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public interface IMainWorker
    {
        void Run();
        void Stop();
        void Dispose();
    }

    public class MainWorker : IMainWorker
    {
        private Client _client;
        private IHttpClientFactory _clientFactory;
        public MainWorker(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
            _client=new Client("default",_clientFactory.CreateClient());
        }
        public void Run()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {

        }

        public void Dispose()
        {

        }
    }
}
