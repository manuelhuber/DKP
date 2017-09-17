﻿using UnityEngine;

namespace Damage {
    public abstract class Attack : MonoBehaviour {
        public virtual bool InRange {
            get { return false; }
        }

        protected Damageable CurrentTarget;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>
        /// true if the target is in range
        /// </returns>
        public virtual bool SetTarget(Damageable target) {
            CurrentTarget = target;
            return InRange;
        }

        public virtual Damageable GetTarget() {
            return CurrentTarget;
        }

        public abstract void AttackNearestTarget();
    }
}