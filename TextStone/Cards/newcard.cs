using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone.Cards
{
    class newcard : Card
    {
        public newcard() : base("newcard", "", 0, 0, 0)
        {

        }

        public override Card Instantiate()
        {
            var card = new newcard();
            card.Initialize();
            DamageEvent.Subscribe(card, 10, OnDamage);
            return card;
        }

        public Action<Object, DamageEventArgs> OnDamage { get; set; } = (me, args) =>
        {
            if (me == args.target)
            {
                
            }
        };
    }
}
