using UnityEngine;

namespace Damage {
    public abstract class Attack : MonoBehaviour {
        protected virtual bool InRange {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// true if the target is in range
        /// </returns>
        public abstract bool SetTarget(Damageable target);

        public abstract void AttackNearestTarget();
    }
}