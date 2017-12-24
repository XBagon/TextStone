using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextStone
{
    //TODO: Optimize events, standard events shouldn't require a subscription!
    public class EventManager
    {
        public class TurnEventArgs : EventArgs
        {
            public readonly Player player;

            public TurnEventArgs(Player player)
            {
                this.player = player;
            }
        }

        public static PriorityEventHandler<TurnEventArgs> TurnEvent = new PriorityEventHandler<TurnEventArgs>();

        public class DrawEventArgs : EventArgs
        {
            public readonly Player player;
            public readonly int amount;
            public readonly Card[] cards;

            public DrawEventArgs(Player player, int amount)
            {
                this.player = player;
                this.amount = amount;
                this.cards = new Card[amount];
            }
        }

        public static PriorityEventHandler<DrawEventArgs> DrawEvent = new PriorityEventHandler<DrawEventArgs>();

        public class PlayEventArgs : EventArgs
        {
            public readonly Player player;
            public readonly Card card;

            public PlayEventArgs(Player player, Card card)
            {
                this.player = player;
                this.card = card;
            }
        }

        public static PriorityEventHandler<PlayEventArgs> PlayEvent = new PriorityEventHandler<PlayEventArgs>();

        public class PlayCardEventArgs : EventArgs
        {
            public readonly Player player;
            public readonly Card card;
            public bool succesful { get; set; } = false;

            public PlayCardEventArgs(Player player, Card card)
            {
                this.player = player;
                this.card = card;
            }
        }

        public static PriorityEventHandler<PlayCardEventArgs> PlayCardEvent = new PriorityEventHandler<PlayCardEventArgs>();

        public class AttackEventArgs : EventArgs
        {
            public readonly IAttacker attacker;
            public readonly IAttacker defender;

            public AttackEventArgs(IAttacker attacker, IAttacker defender)
            {
                this.attacker = attacker;
                this.defender = defender;
            }
        }

        public enum DamageType
        {
            Attack, Defense, Spell, HeroPower
        }

        public static PriorityEventHandler<AttackEventArgs> AttackEvent = new PriorityEventHandler<AttackEventArgs>();

        public class AttackOrderEventArgs : EventArgs
        {
            public bool succesful { get; set; } = false;
            public readonly Player player;
            public readonly IAttacker attacker;
            public readonly IAttacker defender;

            public AttackOrderEventArgs(Player player, IAttacker attacker, IAttacker defender)
            {
                this.player = player;
                this.attacker = attacker;
                this.defender = defender;
            }
        }

        public static PriorityEventHandler<AttackOrderEventArgs> AttackOrderEvent = new PriorityEventHandler<AttackOrderEventArgs>();

        public class DamageEventArgs : EventArgs
        {
            public readonly IAttacker dealer;
            public readonly IAttacker target;
            public readonly int value;
            public readonly DamageType type;

            public DamageEventArgs(IAttacker dealer, IAttacker target, int value, DamageType type)
            {
                this.dealer = dealer;
                this.target = target;
                this.value = value;
                this.type = type;
            }
        }

        public static PriorityEventHandler<DamageEventArgs> DamageEvent { get; } = new PriorityEventHandler<DamageEventArgs>();

        public class DeathEventArgs : EventArgs
        {
            public readonly IAttacker killer;
            public readonly IAttacker target;
            public readonly int value;
            public readonly DamageType type;

            public DeathEventArgs(IAttacker target, IAttacker killer, int value, DamageType type)
            {
                this.target = target;
                this.killer = killer;
                this.value = value;
                this.type = type;
            }
        }

        public static PriorityEventHandler<DeathEventArgs> DeathEvent = new PriorityEventHandler<DeathEventArgs>();

        public class DiscoverChoicesEventArgs : EventArgs
        {
            public readonly Player player;
            public readonly List<Card> pool;
            public readonly Card[] choices;

            public DiscoverChoicesEventArgs(Player player, List<Card> pool, int choices)
            {
                this.player = player;
                this.pool = pool;
                this.choices = new Card[choices];
            }
        }

        public static PriorityEventHandler<DiscoverChoicesEventArgs> DiscoverChoicesEvent = new PriorityEventHandler<DiscoverChoicesEventArgs>();

        public class DiscoverChooseEventArgs : EventArgs
        {
            public readonly Player player;
            public readonly DiscoverChoicesEventArgs choices;
            public readonly int choiceID;
            public Card choice { get; set; }

            public DiscoverChooseEventArgs(Player player, DiscoverChoicesEventArgs choices, int choiceID)
            {
                this.player = player;
                this.choices = choices;
                this.choiceID = choiceID;
            }
        }

        public static PriorityEventHandler<DiscoverChooseEventArgs> DiscoverChooseEvent = new PriorityEventHandler<DiscoverChooseEventArgs>();
    }
}
