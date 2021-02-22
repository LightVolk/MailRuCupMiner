using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
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
        public MainWorker()
        {
     
        }
        public async Task Run(IHost host)
        {
            var exploreService = host?.Services.GetService<IExploreService>();
            while (true)
            {
                try
                {
                    //if (!await host?.Services?.GetService<IHelthCheckService>()?.IsServerReady())
                    //    continue;



                    var report1 = await GetReportAsync(exploreService, 0, 0, 1, 1);
                    var report2 = await GetReportAsync(exploreService, 0, 0, 2, 2);
                    var report3 = await GetReportAsync(exploreService, 0, 0, 5, 5);
                    var report4 = await GetReportAsync(exploreService, 10, 10, 1, 1);
                    var report5 = await GetReportAsync(exploreService, 10, 10, 2, 2);
                    var report6 = await GetReportAsync(exploreService, 10, 10, 5, 5);

                    Program.Logger.Error($"report1 done!:{report1.Amount}");
                    Program.Logger.Error($"report2 done!:{report2.Amount}");
                    Program.Logger.Error($"report3 done!:{report3.Amount}");
                    Program.Logger.Error($"report4 done!:{report4.Amount}");
                    Program.Logger.Error($"report5 done!:{report5.Amount}");
                    Program.Logger.Error($"report6 done!:{report6.Amount}");
                }
                catch (Exception e)
                {
                   Program.Logger.Error(e,"error");
                }
                finally
                {
                    await Task.Delay(1000);
                }
            }
        }

        public void Run()
        {
            var httpClient = new HttpClient();
            var infr= new Infrastructure();
            var client = infr.TryCreateClient(null, httpClient);
            var exploreService = new ExploreService(client);

            var report1 =  GetReportAsync(exploreService, 0, 0, 1, 1).Result;
            var report2 =  GetReportAsync(exploreService, 0, 0, 2, 2).Result;
            var report3 =  GetReportAsync(exploreService, 0, 0, 5, 5).Result;
            var report4 =  GetReportAsync(exploreService, 10, 10, 1, 1).Result;
            var report5 =  GetReportAsync(exploreService, 10, 10, 2, 2).Result;
            var report6 =  GetReportAsync(exploreService, 10, 10, 5, 5).Result;

            Program.Logger.Error($"report1 done!:{report1.Amount}");
            Program.Logger.Error($"report2 done!:{report2.Amount}");
            Program.Logger.Error($"report3 done!:{report3.Amount}");
            Program.Logger.Error($"report4 done!:{report4.Amount}");
            Program.Logger.Error($"report5 done!:{report5.Amount}");
            Program.Logger.Error($"report6 done!:{report6.Amount}");
        }

       
        public void Stop()
        {

        }

        public void Dispose()
        {

        }

        private async Task<Report> GetReportAsync(IExploreService exploreService, int posX, int posY, int sizeX, int sizeY)
        {
            var sw = Stopwatch.StartNew();
            var report = await exploreService?.ExploreAreaAsync(posX, posY, sizeX, sizeY);
            sw.Stop();

            Program.Logger.Error($"Report. {nameof(report.Amount)}:{report.Amount}; Coordinates: ({report.Area.PosY},{report.Area.PosY}), ({report.Area.SizeX} {report.Area.SizeY}) Time:{sw.ElapsedMilliseconds}");
            return report;
        }
    }
}
