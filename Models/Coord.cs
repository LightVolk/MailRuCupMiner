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
        public Status Status; //0 -  исследованное поле (но не копанное), 1-10 - копали на глубине от 1 до 10  -1 - не исследованное поле
        private bool _busy;
        public Coord(int x, int y, Status status)
        {
            X = x;
            Y = y;
            Status = status;

        }

        public bool IsBusy()
        {
            return _busy;
        }

        public void SetBusy()
        {
            _busy = true;
        }
    }

    public enum Status
    {
       // BusyCoord =-2, //занятая область (ее кто-то в сервисе себе забрал, но еще не использовал)
        ClearCoord =-1,
        ExploredCoord =0,
        DiggedCoord1=1,
        DiggedCoord2 = 2,
        DiggedCoord3 = 3,
        DiggedCoord4 = 4,
        DiggedCoord5 = 5,
        DiggedCoord6 = 6,
        DiggedCoord7 = 7,
        DiggedCoord8 = 8,
        DiggedCoord9 = 9,
        DiggedCoord10 = 10,

    }
}
