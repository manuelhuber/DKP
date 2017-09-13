using UnityEngine;

namespace Damage {
    public abstract class Projectile : MonoBehaviour {
        protected GameObject target;

        public void SetTarget(GameObject t) {
            target = t;
        }
    }
}