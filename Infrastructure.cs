using MailRuCupMiner.Clients;
using Mainerspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MailRuCupMiner
{
    public interface IInfrastructure
    {
        IClient TryCreateClient(HttpClient httpClient);
        string CreateAddress(string baseAddr);
        string CreateAddress(string scheme,string address,string port);
        void WriteLog(string message);
    }

    public class Infrastructure : IInfrastructure
    {
        public IClient TryCreateClient(HttpClient httpClient)
        {
            Client client = null;
            while (client == null)
            {
                try
                {
                    string address = Environment.GetEnvironmentVariable("ADDRESS");

                    var baseUrl = CreateAddress(address); //CreateAddress(scheme, address, port);
                    client = new Client(baseUrl, httpClient);
                    WriteLog($"Create client succesfully! Address:{baseUrl}");
                    
                }
                catch(Exception ex)
                {
                    Task.Delay(1000).Wait();
                    continue;
                }
                
            }
            return client;
        }

        public string CreateAddress(string baseAddr)
        {
            return $"http://{baseAddr}:8000/";
        }
        public string CreateAddress(string scheme,string address,string port)
        {
            return $"{scheme}://{address}:{port}";
        }

        public void WriteLog(string message)
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLineAsync(message);
        }
    }
}
