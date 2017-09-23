using System.Runtime.InteropServices;
using Control;
using Damage.Melee;
using UnityEngine;

namespace Abilities.Scripts {
    public class SelfBuffMeleeAoE : Ability {
        public float Duration = 10;

        public override bool OnActivation(GameObject c) {
            var melee = c.GetComponent<MeleeAttack>();
            if (melee == null) return true;
            melee.ChangeAttackForDuration(MeleeAttackVariants.AttackAllTargets, Duration);
            return true;
        }
    }
}