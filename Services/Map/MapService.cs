using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Models;
using MailRuCupMiner.Services.Map;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public class MapService : IMapService
    {
        private List<Coord> _map;
        private readonly object _lock = new object();
        private readonly MapInfrastructure _mapInfrastructure;

        public MapService(MapInfrastructure mapInfrastructure)
        {
            _mapInfrastructure = mapInfrastructure;
            InitMap();
        }

        public List<Coord> GetMap()
        {
            return _map;
        }


        private void InitMap()
        {
            _map = InitMap(_map, Constants.X, Constants.Y);
        }

        public List<Coord> RecalculateMap(List<Coord> map, Report report)
        {
            lock (_lock)
            {
                if (map == null)
                {
                    map = InitMap(null, Constants.X, Constants.Y);
                }

                var maxX = report.Area.PosX + report.Area.SizeX;
                var maxY = report.Area.PosY + report.Area.SizeY;

                Parallel.For(0, map.Count, i =>
                {
                    var coord = map[i];

                    if (coord.X > maxX)
                        return;

                    if (coord.X >= report.Area.PosX && coord.Y >= report.Area.PosY
                                                    && coord.X <= report.Area.PosX + report.Area.SizeX &&
                                                    coord.Y <= report.Area.PosY + report.Area.SizeY)
                        lock (_lock)
                        {
                            coord.Status = Status.ExploredCoord;
                        }
                });

                return map;
            }
        }

        public void RecalculateMap(Report report)
        {
             RecalculateMap(_map, report);
        }

        /// <summary>
        /// Получить незанятую область для копания
        /// </summary>
        /// <returns></returns>
        public Area GetFreeArea()
        {
            return _mapInfrastructure.GetFreeArea(_map, Constants.N);
        }

        
        private List<Coord> InitMap(List<Coord> array, int x, int y)
        {
            if (array != null)
                return array;

            array = new List<Coord>();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    array.Add(new Coord(i, j, Status.ClearCoord));
                }
            }

            return array;
        }
    }
}
