using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Clients;

namespace MailRuCupMiner.Services
{
    public class DigService
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


        public void Dig()
        {
            //_client.di
        }

    }
}
