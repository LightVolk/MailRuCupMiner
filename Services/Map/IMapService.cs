using System.Collections.Generic;
using MailRuCupMiner.Models;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public interface IMapService
    {
        List<Coord> GetMap();
        List<Coord> RecalculateMap(List<Coord> map, Report report);
        void RecalculateMap(Report report);
        Area GetFreeArea();
    }
}