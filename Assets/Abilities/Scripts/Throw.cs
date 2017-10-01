using Control;
using UnityEngine;

namespace Abilities.Scripts {
    public class Throw : Ability {
        public GameObject RangeIndicator;

        public override SpellTargetingType IndicatorType {
            get { return SpellTargetingType.Self; }
        }

        public override bool OnActivation(GameObject caster) {
            return false;
        }
    }
}