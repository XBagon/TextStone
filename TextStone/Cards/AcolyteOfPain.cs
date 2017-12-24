using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone.Cards
{
    class AcolyteOfPain : Card
    {
        public AcolyteOfPain() : base("Acolyte Of Pain", "Whenever this minion takes damage, draw a card.", 3, 1, 3)
        {
            
        }

        public override Card Instantiate()
        {
            var card = new AcolyteOfPain();
            card.Initialize();
            DamageEvent.Subscribe(card, 10, OnDamage);
            return card;
        }

        public Action<Object, DamageEventArgs> OnDamage { get; set; } = (me, args) =>
        {
            if (me == args.target)
            {
                ((Card)me).zone.controller.ExecuteDraw(1);
            }
        };
    }
}
