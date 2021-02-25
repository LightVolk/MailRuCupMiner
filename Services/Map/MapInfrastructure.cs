using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services.Map
{
    public class MapInfrastructure
    {
        private readonly object _lock = new object();
        public Area GetFreeArea(List<Coord> map, int n,int startIndex=0)
        {
            lock (_lock)
            {
                for (int coordIndex = startIndex; coordIndex < map.Count; coordIndex++)
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

                    if (!success)
                        continue;


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
    }
}
