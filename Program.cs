﻿using System;
using System.Net.Http;
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
        private static string _address;
        private static string _port;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logger = CreateLogger();

            _address = Environment.GetEnvironmentVariable("ADDRESS");
            _port = Environment.GetEnvironmentVariable("Port");

//#if DEBUG
//            _address = "http://127.0.0.1";
//            _port = "5000";
//#endif

            Logger.Information($"Address: {_address}");

            using IHost host = CreateHostBuilder(args).Build();
            host.RunAsync();
            host.Services.GetService<IMainWorker>()?.Run(host);
            
            Console.ReadLine();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
            {
                services.AddHttpClient();
                services.AddTransient<Client>(x=>new Client($"{_address}:{_port}", x.GetService<IHttpClientFactory>().CreateClient()));
                services.AddSingleton<IMainWorker, MainWorker>(); // главный класс, в котором происходит вся работа
                services.AddTransient<IExploreService, ExploreService>();
                services.AddSingleton<IHelthCheckService,HelthCheckService>();
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
