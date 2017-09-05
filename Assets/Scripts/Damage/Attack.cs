using UnityEngine;

namespace Damage {
    public abstract class Attack : MonoBehaviour {
        public abstract void SetTarget(Damageable target);
        public abstract void AttackNearestTarget();
        public abstract bool CurrentTargetIsInRange();
    }
}