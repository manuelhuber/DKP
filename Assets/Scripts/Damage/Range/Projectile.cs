using System.Collections.Generic;
using UnityEngine;

namespace Damage.Range {
    public abstract class Projectile : MonoBehaviour {
        protected GameObject Target;
        public List<int> AffectedTeams;

        public void SetTarget(GameObject t) {
            Target = t;
        }
    }
}