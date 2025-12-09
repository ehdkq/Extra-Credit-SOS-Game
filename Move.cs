using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprint_3
{
    public class Move
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Cell MoveType { get; set; }
        public Player Player { get; set; }

        public Move(int r, int c, Cell type, Player p)
        {
            Row = r;
            Column = c;
            MoveType = type;
            Player = p;
        }
    }
}
