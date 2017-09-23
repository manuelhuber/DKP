namespace Damage.Melee {
    public class MeleeAttackVariants {
        public static void SingleTargetDamage(MeleeAttack attack) {
            attack.GetTarget().ModifyHitpoints(attack.AttackDamage);
        }

        public static void AttackAllTargets(MeleeAttack attack) {
            attack.WithinRange.ForEach(damageable => damageable.ModifyHitpoints(attack.AttackDamage));
        }
    }
}