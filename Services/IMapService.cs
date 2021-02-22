using System.Collections.Generic;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public interface IMapService
    {
        IEnumerable<Coord> GetMap();
        IEnumerable<Coord> RecalculateMap(List<Coord> map, Report report);
        Area GetFreeArea();
    }
}