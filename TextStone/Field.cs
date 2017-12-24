using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextStone
{
    public class Field : Zone
    {
        public Field(Player controller) : base(controller)
        {
            type = Game.ZoneType.Field;
        }
    }
}
