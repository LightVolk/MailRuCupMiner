using System;
using MailRuCupMiner.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace MailRuCupMiner
{
    class Program
    {
        public static Logger Logger;
         
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logger = CreateLogger();

            using IHost host = CreateHostBuilder(args).Build();
            host.RunAsync();
            host.Services.GetService<IMainWorker>()?.Run();
            Console.ReadLine();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<IMainWorker, MainWorker>();
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
