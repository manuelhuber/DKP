using Control;
using Damage;
using UnityEngine;

namespace Abilities.Scripts {
    public class SmokeBomb : Ability {
        public float Duration;
        public int HealingAmount;

        public GameObject EffectPrefab;
        public float EffectDuration;

        public override SpellTargetingType IndicatorType {
            get { return SpellTargetingType.None; }
        }

        public override bool OnActivation(GameObject caster) {
            var damageable = caster.GetComponent<Damageable>();
            damageable.MakeUntargetableFor(Duration);
            damageable.ModifyHitpoints(HealingAmount);
            Destroy(Instantiate(EffectPrefab, caster.transform.position, Quaternion.identity), EffectDuration);
            return true;
        }
    }
}