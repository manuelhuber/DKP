using Damage;
using UnityEngine;

namespace Enemies.Skeleton_Swordsman.Scripts {
    public class SkeletonHealth : Damageable {
        private Animator animator;

        protected override void Die() {
            base.Die();
            animator.SetTrigger("Die");
            Destroy(gameObject, 2);
        }

        protected override void OnHit(int amount) {
            if (amount < 0) animator.SetTrigger("Hit");
        }

        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
        }
    }
}