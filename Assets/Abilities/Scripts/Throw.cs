using Control;
using UnityEngine;

namespace Abilities.Scripts {
    public class Throw : Ability {
        public GameObject RangeIndicator;

        public override RangeIndicatorType IndicatorType {
            get { return RangeIndicatorType.Self; }
        }

        public override bool OnActivation(GameObject caster) {
            return false;
        }
    }
}