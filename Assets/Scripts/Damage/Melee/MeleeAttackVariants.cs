namespace Damage.Melee {
    public class MeleeAttackVariants {
        public static void SingleTargetDamage(MeleeAttack attack) {
            attack.GetTarget().ModifyHitpoints(attack.AttackDamage);
        }
    }
}