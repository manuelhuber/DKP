using UnityEngine;

namespace Damage.Range {
    public abstract class Projectile : MonoBehaviour {
        protected GameObject Target;

        public void SetTarget(GameObject t) {
            Target = t;
        }
    }
}