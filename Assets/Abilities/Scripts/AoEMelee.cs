using Control;
using Damage.Melee;
using UnityEngine;
using Util;

namespace Abilities.Scripts {
    public class AoEMelee : Ability {
        public float Duration = 10;
        public GameObject Effect;

        public override SpellTargetingType IndicatorType {
            get { return SpellTargetingType.Self; }
        }

        public override bool OnActivation(GameObject c) {
            var melee = c.GetComponent<MeleeAttack>();
            if (melee == null) return true;
            // Overwriting the asset is bad practices, but whatever
            SpellTargeting.Range = melee.ColliderRadius * 2;
            ActivateRangeIndicator(c);
            melee.ChangeAttackForDuration(AttackEverything, Duration);
            melee.DoAfterDelay(() => CancelTargeting(c), Duration);
            return true;
        }

        public void AttackEverything(MeleeAttack attack) {
            Destroy(Instantiate(Effect, attack.gameObject.transform), attack.AttackInterval / 2);
            attack.WithinRange.ForEach(damageable => damageable.Damage(attack.AttackDamage));
        }
    }
}