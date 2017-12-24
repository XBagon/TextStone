using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextStone
{
    public abstract class Zone
    {
        public Game.ZoneType type { get; protected set; }
        public Player controller { get; }
        public List<Card> cards { get; set; } = new List<Card>();

        protected Zone(Player controller)
        {
            this.controller = controller;
        }
    }
}
