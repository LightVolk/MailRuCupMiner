using System;
using System.Net.Http;
using System.Threading.Tasks;
using MailRuCupMiner.Services;
using Mainerspace;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace MailRuCupMiner
{
    class Program
    {
        public static Logger Logger;
        public static string Address;
        public static string Port;
        public static string Scheme;
        static async Task Main(string[] args)
        {
            try
            {
                Logger = CreateLogger();
                Logger.Error("Hello World!");

                await Task.Delay(5000);

                Address = Environment.GetEnvironmentVariable("ADDRESS");
                Port = Environment.GetEnvironmentVariable("Port");
                Scheme = Environment.GetEnvironmentVariable("Schema");
           
#if DEBUG
                Address = "127.0.0.1";
                Port = "5000";
                Scheme = "http";
#endif

                while (string.IsNullOrEmpty(Address))
                {
                    await Task.Delay(1000);
                }

                var infrastructure = new Infrastructure();

                Address = infrastructure.CreateAddress(Scheme, Address, Port);
                Logger.Error($"Address: {Address}");

                using IHost host = CreateHostBuilder(args).Build();
                host.Services.GetService<IMainWorker>()?.Run(host);
                host.RunAsync().Wait();
            }
            catch (Exception e)
            {
                Logger.Error(e, "error");

            }
            finally
            {
                Logger.Error("Wait...");
                Task.Delay(-1).Wait();
            }

        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {         
            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddHttpClient();
                //services.AddTransient<Client>(x => new Client($"{_address}:{_port}", x.GetService<IHttpClientFactory>().CreateClient()));
                services.AddTransient<Infrastructure>();
                services.AddSingleton<IMainWorker, MainWorker>(); // главный класс, в котором происходит вся работа                
                services.AddTransient<IExploreService, ExploreService>();
                services.AddTransient<IHelthCheckService, HelthCheckService>();
            });
        }

        static Logger CreateLogger()
        {
            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            return log;
        }
    }
}
