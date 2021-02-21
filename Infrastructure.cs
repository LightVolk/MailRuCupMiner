using Mainerspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MailRuCupMiner
{
    public class Infrastructure
    {
        public Client TryCreateClient(Client client,HttpClient httpClient)
        {
            
            while (client == null)
            {
                try
                {
                    var address = Environment.GetEnvironmentVariable("ADDRESS");
                    var port = Environment.GetEnvironmentVariable("Port");
                    var scheme = Environment.GetEnvironmentVariable("Schema");
#if DEBUG
                    address = "127.0.0.1";
                    port = "5000";
                    scheme = "http";
#endif
                    var baseUrl = CreateAddress(scheme, address, port);
                    client = new Client(baseUrl, httpClient);
                }
                catch(Exception ex)
                {
                    Task.Delay(1000).Wait();
                    continue;
                }
                
            }
            return client;
        }

        public string CreateAddress(string scheme,string address,string port)
        {
            return $"{scheme}://{address}:{port}";
        }
    }
}
