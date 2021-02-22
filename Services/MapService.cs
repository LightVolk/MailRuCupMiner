using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public class MapService
    {
        private List<Coord> _map;
        private readonly object _lock = new object();

        public MapService()
        {
            _map = new List<Coord>();
            InitMap();
        }

        public IEnumerable<Coord> GetMap()
        {
            return _map;
        }


        private void InitMap()
        {
            _map = InitMap(_map, 3500, 3500);
        }

        public List<Coord> RecalculateMap(List<Coord> map, Report report)
        {
            if (map == null)
            {
                map = InitMap(map, Constants.X, Constants.Y);
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
                        coord.Status = 0;
                    }
            });

            return map;
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
