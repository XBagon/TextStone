using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone.Cards
{
    class FuriousWolf : Card
    {
        public FuriousWolf() : base("Furious Wolf", "After this minion attacks, gain +2 Attack.", 4, 3, 4){}

        public override Card Instantiate()
        {
            var card = new FuriousWolf();
            card.Initialize();
            AttackEvent.Subscribe(card, 50, OnAttack);
            return card;
        }

        public Action<Object, AttackEventArgs> OnAttack { get; set; } = (me, args) =>
        {
            if (me == args.attacker)
            {
                ((Card) me).attack += 2;
            }
        };

    }
}
