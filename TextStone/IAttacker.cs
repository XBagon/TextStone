using System;
using static TextStone.EventManager;

namespace TextStone
{
    public interface IAttacker
    {
        string name { get; }
        int health { get; set; }
        int attack { get; set; }


        void ExecuteAttack(IAttacker target);
        void ExecuteDealDamage(IAttacker target, int value, DamageType type);
        void ExecuteDeath(IAttacker dealer, int value, DamageType type);
        int GetAttackForAttacking();
        int GetAttackForDefending();
        Func<IAttacker, bool> CanAttack { get; set; }
    }
}