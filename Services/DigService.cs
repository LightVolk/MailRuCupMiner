using System;
using System.Collections.Generic;
using System.Linq;
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
        public DigService(IClient client,ILicenseService licenseService, IMapService mapService,IExploreService exploreService)
        {
            _client = client;
            _licenseService = licenseService;
            _mapService = mapService;
            _exploreService = exploreService;
        }


        public async Task<ICollection<string>> Dig(int depth)
        {
            var freeLicense = _licenseService.GetFreeLicence();
            var freeArea = _mapService.GetFreeArea();

            if(freeArea==null)
                return new List<string>();
            
            var dig = await _client.DigAsync(new Dig(){Depth = depth,LicenseID = freeLicense.Id,PosX = freeArea.PosX,PosY = freeArea.PosY});

            var infr = new Infrastructure();
            foreach (var d in dig)
            {
                infr.WriteInStdErr(d);
            }
            return dig;
        }

    }
}
