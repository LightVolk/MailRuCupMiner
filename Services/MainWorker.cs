using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mainerspace;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MailRuCupMiner.Services
{
    public interface IMainWorker
    {
        Task Run(IHost host);
        void Stop();
        void Dispose();
    }

    public class MainWorker : IMainWorker
    {
        private Client _client;
        private IHttpClientFactory _clientFactory;
        public MainWorker(Client client)
        {
            _client = client;
        }
        public async Task Run(IHost host)
        {
            
            while (true)
            {
                try
                {
                    if(!await host?.Services?.GetService<IHelthCheckService>()?.IsServerReady())
                        continue;

                    await GetReportAsync(host, 0, 0, 1, 1);
                    await GetReportAsync(host, 0, 0, 2, 2);
                    await GetReportAsync(host, 0, 0, 5, 5);
                    await GetReportAsync(host, 10, 10, 1, 1);
                    await GetReportAsync(host, 10, 10, 2, 2);
                    await GetReportAsync(host, 10, 10, 5, 5);
                    Environment.Exit(0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    Thread.Sleep(1000);
                  //  Environment.Exit(0);
                }
            }
        }

        public void Stop()
        {

        }

        public void Dispose()
        {

        }

        private async Task<Report> GetReportAsync(IHost host,int posX, int posY, int sizeX, int sizeY)
        {
            var sw = Stopwatch.StartNew();
            var exploreService = host.Services.GetService<IExploreService>();
            var report = await exploreService?.ExploreAreaAsync(posX,posY,sizeX,sizeY);
            sw.Stop();
           
            Program.Logger.Error($"Report. {nameof(report.Amount)}:{report.Amount}; Coordinates: ({report.Area.PosY},{report.Area.PosY}), ({report.Area.SizeX} {report.Area.SizeY}) Time:{sw.ElapsedMilliseconds}");
            return report;
        }
    }
}
