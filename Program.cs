using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
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
        public class Options
        {
            [Option('A', "ADDRESS")]
            public string ADDRESS { get; set; }

            [Option("Port")]
            public string Port { get; set; }

            [Option("Schema")]
            public string Schema { get; set; }
        }

        public static Logger Logger;
        public static string Address;
        public static string Port;
        public static string Schema;
        private static Options _options;
        static void Main(string[] args)
        {
            try
            {
                Logger = CreateLogger();
                Logger.Error("Hello World!");

               
                
                Thread.Sleep(5010);


               
#if DEBUG
                Address = "127.0.0.1";
                Port = "5000";
                Schema = "http";
#endif

#if RELEASE
                Port = "8000";
                Schema = "http";
#endif

                Logger.Error($"{nameof(Address)}:{Address}");
                Logger.Error($"{nameof(Port)}:{Port}");
                Logger.Error($"{nameof(Schema)}:{Schema}");

                int count = 5;
                while (string.IsNullOrEmpty(Address))
                {
                    Address = Environment.GetEnvironmentVariable("ADDRESS");
                    Thread.Sleep(1000);

                    count++;

                    if (count >= 5)
                    {
                        Address = "192.168.34.2";
                        Port = "8000";
                        Schema = "http";
                        break;
                    }
                }

                RunMainWorker();
                //using IHost host = CreateHostBuilder(args).Build();

                //var infrastructure = new Infrastructure();
                //if (string.IsNullOrEmpty(Address))
                //{
                //    var cl = infrastructure.TryCreateClient(null,
                //        host.Services.GetRequiredService<IHttpClientFactory>().CreateClient());
                //}


                //host.Services.GetService<IMainWorker>()?.Run(host).Wait();
                //host.RunAsync().Wait();
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

        private static object HandleParseError(IEnumerable<CommandLine.Error> errs)
        {
            return null;
        }

        private static int RunOptionsAndReturnExitCode<TResult>(Options opts)
        {
            _options = opts;
            return 0;
        }

        private static void RunOptions(Options obj)
        {
            _options = obj;
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
                //.WriteTo.Console()
                .CreateLogger();
            return log;
        }
    }
}
