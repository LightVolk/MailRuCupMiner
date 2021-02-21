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
        static async Task Main(string[] args)
        {
            try
            {
                Logger = CreateLogger();
                Logger.Error("Hello World!");

                //CommandLine.Parser.Default.ParseArguments<Options>(args).MapResult((opts) => RunOptionsAndReturnExitCode<Options>(opts), //in case parser sucess
                //    errs => HandleParseError(errs)); //in  case parser fail

                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        _options = o;
                    });
                await Task.Delay(5000);
                Thread.Sleep(5000);


                Address = _options?.ADDRESS;
                Port = _options?.Port;
                Schema = _options?.Schema;
           
#if DEBUG
                Address = "127.0.0.1";
                Port = "5000";
                Schema = "http";
#endif
                Logger.Error($"{nameof(Address)}:{Address}");
                Logger.Error($"{nameof(Port)}:{Port}");
                Logger.Error($"{nameof(Schema)}:{Schema}");

                while (string.IsNullOrEmpty(Address))
                {
                    Address = Environment.GetEnvironmentVariable("ADDRESS");
                    Port = Environment.GetEnvironmentVariable("Port");
                    Schema = Environment.GetEnvironmentVariable("Schema");

                    await Task.Delay(1000);
                    Thread.Sleep(1000);
                }
                                               

                using IHost host = CreateHostBuilder(args).Build();

                var infrastructure = new Infrastructure();
                var cl = infrastructure.TryCreateClient(null, host.Services.GetRequiredService<IHttpClientFactory>().CreateClient());

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
