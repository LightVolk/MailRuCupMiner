using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Clients;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public interface IDigService
    {
        Task<ICollection<string>> Dig(int depth);
    }

    public class DigService : IDigService
    {
        private readonly IClient _client;
        private ILicenseService _licenseService;
        private IMapService _mapService;
        private IExploreService _exploreService;
        private Infrastructure _infrastructure;
        public DigService(ILicenseService licenseService, IMapService mapService, IExploreService exploreService, Infrastructure infrastructure, IHttpClientFactory httpClientFactory)
        {
            _infrastructure = infrastructure;
            _client = _infrastructure.TryCreateClient(null, httpClientFactory.CreateClient());
            _licenseService = licenseService;
            _mapService = mapService;
            _exploreService = exploreService;

        }


        public async Task<ICollection<string>> Dig(int depth)
        {
            _infrastructure.WriteLog("Start dig");
            var freeLicense = await _licenseService.TryGetFreeLicence();

            try
            {
                var freeArea = _mapService.GetFreeArea();

                if (freeArea == null)
                    return new List<string>();
                _infrastructure.WriteLog($"freeArea:{freeArea.PosX};{freeArea.PosY};{freeArea.SizeX};{freeArea.SizeY}");

                var exloreAreaReport = await
                    _exploreService.ExploreAreaAsync(freeArea.PosX, freeArea.PosY, (int)freeArea.SizeX, (int)freeArea.SizeX);

                if (exloreAreaReport == null) return new List<string>();

                if (exloreAreaReport.Amount > 0)
                {
                    _infrastructure.WriteLog($"{nameof(exloreAreaReport)}:{exloreAreaReport.Amount}");
                    var dig = await _client.DigAsync(new Dig()
                    { Depth = depth, LicenseID = freeLicense.Id, PosX = freeArea.PosX, PosY = freeArea.PosY });

                    foreach (var d in dig)
                    {
                        _infrastructure.WriteLog(d);
                    }

                    _mapService.RecalculateMap(exloreAreaReport);
                    return dig;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                _infrastructure.WriteLog($"{ex.Message} {ex.StackTrace}");
            }
            finally
            {
                freeLicense?.RegisterDig();
                _licenseService.ReturnLicenseBack(freeLicense);
                _infrastructure.WriteLog("End dig");
            }

            return new List<string>();
        }

    }
}
