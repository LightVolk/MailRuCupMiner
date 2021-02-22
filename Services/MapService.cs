using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public class MapService : IMapService
    {
        private List<Coord> _map;
        private readonly object _lock = new object();


        public MapService()
        {
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

        /// <summary>
        /// Получить незанятую область для копания
        /// </summary>
        /// <returns></returns>
        public Area GetFreeArea()
        {
            return GetFreeAreaInternal(_map, Constants.N);
        }

        private Area GetFreeAreaInternal(List<Coord> map, int n)
        {
            lock (_lock)
            {
                for (int coordIndex = 0; coordIndex < map.Count; coordIndex++)
                {
                    if (map[coordIndex].IsBusy()) continue;

                    var coordAreaTmp = new List<Coord>();
                    var coord = map[coordIndex];
                    bool success = true;
                    for (int i = coord.X; i < coord.X + n; i++)
                    {
                        var findCoord = map.FirstOrDefault(x => x.X == i && coordAreaTmp.Contains(x) == false);
                        if (findCoord == null) continue;

                        if (findCoord.IsBusy())
                        {
                            success = false;
                            break;
                        }

                        coordAreaTmp.Add(findCoord);
                    }

                    if(!success)
                        continue;
                    //for (int i = coord.X; i < coord.X + n; i++)
                    //{
                    //    if (map[i].IsBusy()
                    //    ) //если клетка в пределах N занята - то пропускаем эту клетку и эту Арию
                    //    {
                    //        success = false;
                    //        break;
                    //    }

                    //    coordAreaTmp.Add(map[i]);
                    //}

                    for (int i = coord.Y; i < coord.Y + n; i++)
                    {
                        var findCoord = map.FirstOrDefault(x => x.Y == i && coordAreaTmp.Contains(x) == false);
                        if (findCoord == null) continue;

                        if (findCoord.IsBusy())
                        {
                            success = false;
                            break;
                        }

                        coordAreaTmp.Add(findCoord);
                    }

                    //for (int i = coord.Y; i < coord.Y + n; i++)
                    //{
                    //    if (map[i].IsBusy()
                    //    ) //если клетка в пределах N занята - то пропускаем эту клетку и эту Арию
                    //    {
                    //        success = false;
                    //        break;
                    //    }

                    //    coordAreaTmp.Add(map[i]);
                    //}

                    if (success && coordAreaTmp.Any())
                    {
                        var firstCoord = coordAreaTmp.First();
                        SetBusy(coordAreaTmp);
                        return new Area() { PosX = firstCoord.X, PosY = firstCoord.Y, SizeX = n, SizeY = n };
                    }
                }

                return null;
            }
        }

        private void SetBusy(List<Coord> coords)
        {
            if (coords != null)
                foreach (var coord in coords)
                {
                    coord.SetBusy();
                }
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
