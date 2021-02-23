using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using MailRuCupMiner.Clients;
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
        public static string Schema;
        
        static void Main(string[] args)
        {
            try
            {
                Logger = CreateLogger();
                Logger.Error("Hello World!");

               
                
                Thread.Sleep(5010);

                Logger.Error($"{nameof(Address)}:{Address}");
                Logger.Error($"{nameof(Port)}:{Port}");
                Logger.Error($"{nameof(Schema)}:{Schema}");


                //RunMainWorker();
                using IHost host = CreateHostBuilder(args).Build();

                host.Services.GetService<IMainWorker>()?.Run(host).Wait();
                host.RunAsync().Wait();
            }
            catch (Exception e)
            {
                Logger.Error(e, "error");

            }
            finally
            {
                Logger.Error("Wait...");
                

                while (true)
                {
                    Thread.Sleep(1);
                }
            }

        }

        public static void RunMainWorker()
        {
            var mainWorker = new MainWorker();
            mainWorker.Run();
        }

        


        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var infrastructure = new Infrastructure();
            string address = Environment.GetEnvironmentVariable("ADDRESS");
            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddHttpClient();

                #region singleton
                services.AddSingleton<IClient, Client>(x => new Client(infrastructure.CreateAddress(address), x.GetService<IHttpClientFactory>().CreateClient()));
                services.AddSingleton<IMainWorker, MainWorker>(); // главный класс, в котором происходит вся работа                
                services.AddSingleton<IExploreService, ExploreService>();
                services.AddSingleton<ILicenseService, LicenseService>();
                services.AddSingleton<IMapService, MapService>();
                #endregion

                #region transient
                services.AddTransient<Infrastructure>();
                services.AddTransient<IHelthCheckService, HelthCheckService>();
                services.AddTransient<IDigService, DigService>();
                #endregion


            });
        }

        static Logger CreateLogger()
        {
            using var log = new LoggerConfiguration()
                //.WriteTo.Console()
                .CreateLogger();
            return log;
        }
    }
}
