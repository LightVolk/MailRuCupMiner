using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRuCupMiner.Models
{
    public class Coord
    {
        public int X;
        public int Y;
        public int Status; //0 -  исследованное поле (но не копанное), 1-10 - копали на глубине от 1 до 10  -1 - не исследованное поле

        public Coord(int x, int y, int status)
        {
            X = x;
            Y = y;
            Status = status;
        }
    }
}
