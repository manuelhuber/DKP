using Control;
using Damage;
using UnityEngine;

namespace Abilities.Scripts {
    public class MakeSelfUntargetable : Ability {
        public float Duration;

        public GameObject EffectPrefab;
        public float EffectDuration;

        public override bool OnActivation(GameObject caster) {
            caster.GetComponent<Damageable>().MakeUntargetableFor(Duration);
            var effect = Instantiate(EffectPrefab, caster.transform.position, Quaternion.identity);
            caster.GetComponent<Renderer>().material.color = Color.blue;
            Destroy(effect, EffectDuration);
            return true;
        }
    }
}