using Control;
using Damage.Melee;
using UnityEngine;

namespace Abilities.Scripts {
    public class AoEMelee : Ability {
        public float Duration = 10;
        public GameObject Effect;

        public override RangeIndicatorType IndicatorType {
            get { return RangeIndicatorType.Self; }
        }

        public override bool OnActivation(GameObject c) {
            var melee = c.GetComponent<MeleeAttack>();
            if (melee == null) return true;
            melee.ChangeAttackForDuration(AttackEverything, Duration);
            return true;
        }

        public void AttackEverything(MeleeAttack attack) {
            Destroy(Instantiate(Effect, attack.gameObject.transform), attack.AttackInterval / 2);
            attack.WithinRange.ForEach(damageable => damageable.ModifyHitpoints(attack.AttackDamage));
        }
    }
}