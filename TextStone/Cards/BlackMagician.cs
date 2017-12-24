using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone.Cards
{
    class BlackMagician : Card
    {
        public BlackMagician() : base("Black Magician", "Draw 3 cards, the first and third cards cost (0) Mana. The middle card cost is increased by the reducement.", 6, 4, 5)
        {

        }

        public override Card Instantiate()
        {
            var card = new BlackMagician();
            card.Initialize();
            PlayEvent.Subscribe(card, 10, OnPlay);
            return card;
        }

        public Action<Object, PlayEventArgs> OnPlay { get; set; } = (me, args) =>
        {
            if (me == args.card)
            {
                var drawArgs = args.card.zone.controller.ExecuteDraw(3);
                int manaReducement = drawArgs.cards[0].mana + drawArgs.cards[2].mana;
                drawArgs.cards[0].mana = 0;
                drawArgs.cards[1].mana += manaReducement;
                if (drawArgs.cards[1].mana > 10) drawArgs.cards[1].mana = 10;
                drawArgs.cards[2].mana = 0;
            }
        };
    }
}
