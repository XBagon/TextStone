using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone
{
    public class Player : IAttacker
    {
        public string name { get; }

        public int maxMana { get; set; }
        public int mana { get; set; }

        public int maxHealth { get; set; } = 30;
        public int health { get; set; } = 30;

        public int attack { get; set; }

        public Deck deck { get; set; }
        public Hand hand { get; set; }
        public Field field { get; set; }
        public Grave grave { get; set; }

        public Card HeroPower;

        public Player(string name, Card[] deck)
        {
            this.name = name;
            this.deck = new Deck(deck,this);
            hand = new Hand(this);
            field = new Field(this);
            grave = new Grave(this);

            DrawEvent.Subscribe(this, 0, Draw);
            PlayCardEvent.Subscribe(this, 0, Play);
            AttackOrderEvent.Subscribe(this, 0, AttackOrder);
            AttackEvent.Subscribe(this, 0, Attack);
            DamageEvent.Subscribe(this, 0, Damage);
            DeathEvent.Subscribe(this, 0, Death);
        }

        public string ToString(string format) //TODO: add vorzeichen (#) und namen
        {
            return format.
            Replace("mm", maxMana + "").
            Replace("mh", maxHealth + "").
            Replace("m", mana + "").
            Replace("h", health + "").
            Replace("a", attack + "");
            //Replace("hp", HeroPower.name);
        }

        public DrawEventArgs ExecuteDraw(int amount)
        {
            var args = new DrawEventArgs(this, amount);
            DrawEvent.Call(args);
            return args;
        }

        public Action<Object, DrawEventArgs> Draw { get; set; } = (me, args) =>
        {
            if (me == args.player)
            {
                for (int i = 0; i < args.amount; i++)
                {
                    args.cards[i] = args.player.deck.cards[i];
                    args.player.deck.cards[i].Move(args.player.hand);
                }
            }
        };

        public bool ExecutePlay(Card card)
        {
            var args = new PlayCardEventArgs(this, card);
            PlayCardEvent.Call(args);
            return args.succesful;
        }

        public Action<Object, PlayCardEventArgs> Play { get; set; } = (me,args) =>
        {
            if (me == args.player)
            {
                if (args.card.CanPlay(args.card))
                {
                    args.player.mana -= args.card.mana;
                    args.succesful = true;
                    PlayEvent.Call(new PlayEventArgs(args.player, args.card));
                }
            }
        };

        public bool ExecuteAttackOrder(IAttacker attacker, IAttacker target)
        {
            var args = new AttackOrderEventArgs(this, attacker, target);
            AttackOrderEvent.Call(args);
            return args.succesful;
        }

        public Action<Object, AttackOrderEventArgs> AttackOrder { get; set; } = (me, args) =>
        {
            if (me == args.player)
            {
                if (args.attacker.CanAttack(args.attacker))
                {
                    args.succesful = true;
                    AttackEvent.Call(new AttackEventArgs(args.attacker, args.defender));
                }
            }
        };

        public void ExecuteAttack(IAttacker target)
        {
            AttackEvent.Call(new AttackEventArgs(this, target));
        }

        public Action<Object, AttackEventArgs> Attack { get; set; } = (me, args) =>
        {
            if (args.attacker == me)
            {
                args.attacker.ExecuteDealDamage(args.defender, args.attacker.GetAttackForAttacking(), DamageType.Attack);
                args.defender.ExecuteDealDamage(args.attacker, args.defender.GetAttackForDefending(), DamageType.Defense);
            }
        };

        public Func<IAttacker, bool> CanAttack { get; set; } = (attacker) =>
        {
            return true;//TODO
        };

        public int GetAttackForAttacking()
        {
            return attack;
        }

        public int GetAttackForDefending()
        {
            return 0;
        }

        public void ExecuteDealDamage(IAttacker target, int value, DamageType type)
        {
            DamageEvent.Call(new DamageEventArgs(this, target, value, type));
        }

        public Action<Object, DamageEventArgs> Damage { get; set; } = (me, args) =>
        {
            if (me == args.target)
            {
                args.target.health -= args.value;
                if (args.target.health <= 0)
                {
                    args.target.ExecuteDeath(args.dealer, args.value, args.type);
                }
            }
        };

        public void ExecuteDeath(IAttacker dealer, int value, DamageType type)
        {
            DeathEvent.Call(new DeathEventArgs(this, dealer, value, type));
        }

        public Action<Object, DeathEventArgs> Death { get; set; } = (me, args) =>
        {
            if (me == args.target)
            {
                Player player = args.target as Player;
                if (player != null)
                {
                    Console.WriteLine($"{player.name} died."); //DEBUG
                }
            }
        };

        //TODO: DISCOVER STUFF
        public Card[] ExecuteDiscoverChoices(List<Card> pool, int choices)
        {
            var args = new DiscoverChoicesEventArgs(this, pool, choices);
            DiscoverChoicesEvent.Call(args);
            return args.choices;
        }

        public Action<Object, DiscoverChoicesEventArgs> DiscoverChoices { get; set; } = (me, args) =>
        {
            if (me == args.player)
            {
                Player player = args.player as Player;
                if (player != null)
                {
                    for (int i = 0; i < args.choices.Length; i++)
                    {
                        int randInd = Game.RNG.Next(args.pool.Count);
                        args.choices[i] = args.pool[randInd];
                        args.pool.RemoveAt(randInd);
                    }
                }
            }
        };

        public Card ExecuteDiscoverChoose(DiscoverChoicesEventArgs choices, int choice)
        {
            var args = new DiscoverChooseEventArgs(this, choices, choice);
            DiscoverChooseEvent.Call(args);
            return args.choice;
        }

        public Action<Object, DiscoverChooseEventArgs> DiscoverChoose { get; set; } = (me, args) =>
        {
            if (me == args.player)
            {
                args.choice = args.choices.choices[args.choiceID];
            }
        };

    }
}
