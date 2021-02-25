using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services.Map
{
    public class RandomMapService:IMapService
    {
        private readonly MapService _mapService;
        private readonly MapInfrastructure _mapInfrastructure;
        public RandomMapService(MapInfrastructure mapInfrastructure)
        {
            _mapInfrastructure = mapInfrastructure;
            _mapService=new MapService(mapInfrastructure);
        }
        public List<Coord> GetMap()
        {
            return _mapService.GetMap();
        }

        public List<Coord> RecalculateMap(List<Coord> map, Report report)
        {
            return _mapService.RecalculateMap(map, report);
        }

        public void RecalculateMap(Report report)
        {
            _mapService.RecalculateMap(GetMap(), report);
        }

        public Area GetFreeArea()
        {
            var random = new Random(DateTime.UtcNow.Millisecond);
            var startIndex= random.Next();
            return _mapInfrastructure.GetFreeArea(_mapService.GetMap(), Constants.N, startIndex);
        }
    }
}
