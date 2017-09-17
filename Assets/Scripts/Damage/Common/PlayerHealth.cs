using Control;
using UnityEngine;
using Util;

namespace Damage.Common {
    public class PlayerHealth : Damageable {
        private Animator animator;

        protected override void Die() {
            base.Die();
            animator.SetTrigger("Die");
        }

        protected override void OnHit(int amount) {
            if (amount < 0) animator.SetTrigger("Hit");
        }

        protected override void Start() {
            base.Start();
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag);
        }
    }
}