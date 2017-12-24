using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextStone
{
    public class Hand : Zone
    {
        public Hand(Player controller) : base(controller)
        {
            type = Game.ZoneType.Hand;
        }
    }
}
