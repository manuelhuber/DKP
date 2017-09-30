using Control;
using Damage;
using UnityEngine;

namespace Abilities.Scripts {
    public class Vanish : Ability {
        public float Duration;

        public override bool OnActivation(GameObject caster) {
            caster.GetComponent<Damageable>().MakeUntargetableFor(Duration);
            return true;
        }
    }
}