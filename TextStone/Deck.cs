using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextStone
{
    public class Deck : Zone
    {
        public Deck(Card[] cards, Player controller) : base(controller)
        {
            type = Game.ZoneType.Deck;
            foreach (var card in cards)
            {
                var instance = card.Instantiate();
                instance.zone = this;
                this.cards.Add(instance);
            }
        }

    }
}
