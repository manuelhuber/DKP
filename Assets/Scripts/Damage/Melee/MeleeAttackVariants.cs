using System;
using Util;

namespace Damage.Melee {
    /// <summary>
    /// Different kind of melee attacks
    /// At some points it might be good to refactor these to ScriptableObjects
    /// </summary>
    public class MeleeAttackVariants {
        public static void SingleTargetDamage(MeleeAttack attack) {
            attack.GetTarget().Damage(attack.AttackDamage);
        }

        /// <summary>
        /// Creates a backstab action. Backstab does more damage when the target isn't facing the attacker.
        /// </summary>
        /// <param name="factor">
        /// The damage multiplication factor. 2 = double damage
        /// </param>
        /// <returns></returns>
        public static Action<MeleeAttack> Backstab(double factor) {
            return attack => {
                var dmg = attack.AttackDamage;
                if (!PositionUtil.Facing(attack.GetTarget().gameObject.transform, attack.gameObject.transform)) {
                    dmg = (int) (dmg * factor);
                }
                attack.GetTarget().Damage(dmg);
            };
        }
    }
}