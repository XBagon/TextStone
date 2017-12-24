using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static TextStone.EventManager;

namespace TextStone
{
    public class Card : IAttacker
    {
        public Zone zone { get; set; }

        public string name { get; }

        public string description { get; }

        public int MANA { get; }
        public int mana { get; set; }

        public int HEALTH { get; }
        public int maxHealth { get; set; }
        public int health { get; set; }

        public int ATTACK { get; }
        public int attack { get; set; }

        public enum AttackState
        {
            Zero, Sleeping, Frozen, Exhausted, CanAttack, CanAttackTwice
        }

        public AttackState state { get; set; } = AttackState.Zero;

        public Card(string name, string description, int MANA, int ATTACK, int HEALTH)
        {
            this.name = name;
            this.description = description;
            this.MANA = MANA;
            this.mana = MANA;
            this.HEALTH = HEALTH;
            this.maxHealth = HEALTH;
            this.health = HEALTH;
            this.ATTACK = ATTACK;
            this.attack = ATTACK;
        }

        public virtual Card Instantiate()
        {
            var card = new Card(name,description,MANA,ATTACK,HEALTH);
            AttackEvent.Subscribe(card, 0, Attack);
            DamageEvent.Subscribe(card, 0, Damage);
            DeathEvent.Subscribe(card, 0, Death);
            PlayEvent.Subscribe(card, 0, Play);
            TurnEvent.Subscribe(card, 0, NewTurn);
            return card;
        }

        public void Initialize()
        {
            AttackEvent.Subscribe(this, 0, Attack);
            DamageEvent.Subscribe(this, 0, Damage);
            DeathEvent.Subscribe(this, 0, Death);
            PlayEvent.Subscribe(this, 0, Play);
            TurnEvent.Subscribe(this, 0, NewTurn);
        }

        public string ToString(string format)
        {
            return format.
            Replace("#mh", maxHealth + "").
            Replace("#m", mana + "").
            Replace("#M", MANA + "").
            Replace("#h", health + "").
            Replace("#H", HEALTH + "").
            Replace("#a", attack + "").
            Replace("#A", ATTACK + "").
            Replace("#s",state.ToString("G")).
            Replace("#n", name).
            Replace("#z", zone.type.ToString("G"));
        }

        public Card GetThis()
        {
            return this;
        }

        public void ExecuteAttack(IAttacker target)
        {
            AttackEvent.Call(new AttackEventArgs(this, target));
        }

        public Action<Object,AttackEventArgs> Attack { get; set; } = (me,args) =>
        {
            if (args.attacker == me)
            {
                ((Card) me).state--;
                args.attacker.ExecuteDealDamage(args.defender, args.attacker.GetAttackForAttacking(),DamageType.Attack);
                args.defender.ExecuteDealDamage(args.attacker, args.defender.GetAttackForDefending(),DamageType.Defense);
            }
        };

        public Func<IAttacker, bool> CanAttack { get; set; } = (attacker) =>
        {
            var card = (Card) attacker;
            return (int)card.state >= (int)AttackState.CanAttack && card.zone.type == Game.ZoneType.Field;
        };

        public int GetAttackForAttacking()
        {
            return attack;
        }

        public int GetAttackForDefending()
        {
            return attack;
        }

        public void ExecuteDealDamage(IAttacker target, int value, DamageType type)
        {
            DamageEvent.Call(new DamageEventArgs(this,target,value,type));
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

        public void ExecuteDeath(IAttacker killer, int value, DamageType type)
        {
            DeathEvent.Call(new DeathEventArgs(this, killer, value,type));
        }

        public Action<Object,DeathEventArgs> Death { get; set; } = (me,args) =>
        {
            if (me == args.target)
            {
                Card card = args.target as Card;
                if (card != null)
                {
                    card.Move(card.zone.controller.grave);
                }
            }
        };

        public Func<Card, bool> CanPlay { get; set; } = (sender) =>
        {
            return sender.zone.controller.mana >= sender.mana;
        };


        public Action<Object, PlayEventArgs> Play { get; set; } = (me, args) =>
        {
            if (me == args.card)
            {
                args.card.Move(args.card.zone.controller.field);
                args.card.state = AttackState.Sleeping;
            }
        };

        public Action<Object, TurnEventArgs> NewTurn { get; set; } = (me, args) =>
        {
            var card = (Card) me;
            if (card.zone == args.player.field)
            {
                card.state = AttackState.CanAttack;
            }
        };

        public void Move(Zone zone)
        {
            this.zone.cards.Remove(this);
            zone.cards.Add(this);
            this.zone = zone;
        }
    }
}
