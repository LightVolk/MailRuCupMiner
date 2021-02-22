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
    public class Infrastructure
    {
        public Client TryCreateClient(Client client,HttpClient httpClient)
        {            
            while (client == null)
            {
                try
                {
                    string address = Environment.GetEnvironmentVariable("ADDRESS");
                    //return $"http://{address}:8000/";
                    //var address =  "192.168.34.2"; //Environment.GetEnvironmentVariable("ADDRESS");
                    //var port = "8000";
                    //var scheme = "http";
//#if DEBUG
//                    address = "127.0.0.1";
//                    port = "5000";
//                    scheme = "http";
//#endif
                    var baseUrl = CreateAddress(address); //CreateAddress(scheme, address, port);
                    client = new Client(baseUrl, httpClient);
                    Program.Logger.Error($"Create client succesfully! Address:{baseUrl}");
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

        public void WriteInStdErr(string message)
        {
            TextWriter errorWriter = Console.Error;
            errorWriter.WriteLineAsync(message);
        }
    }
}
