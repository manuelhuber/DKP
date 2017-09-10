using Control;
using UnityEngine;
using Util;

namespace Damage {
    public class PlayerHealth : Damageable {
        protected override void Die() {
            UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PLAYER_ANIMATION_TAG)
                .SetTrigger("Die");
        }
    }
}