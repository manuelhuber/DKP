using UnityEngine;

namespace Damage {
    public abstract class Attack : MonoBehaviour {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// true if the target is in range
        /// </returns>
        public abstract bool SetTarget(Damageable target);

        public abstract void AttackNearestTarget();
        public abstract bool CurrentTargetIsInRange();
    }
}