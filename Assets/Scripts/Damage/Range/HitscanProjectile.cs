using UnityEngine;
using Util;

namespace Damage.Range {
    public class HitscanProjectile : Projectile {
        [Tooltip("Positive numbers heal, negative numbers deal damage")] public int Damage;

        private void Shoot() {
            RaycastHit hit;
            if (PositionUtil.RayFromTo(gameObject, Target, out hit)) {
                var kill = hit.transform.gameObject.GetComponent<Damageable>();
                if (kill != null) {
                    kill.ModifyHitpoints(Damage);
                }
            }
            Destroy(gameObject);
        }

        private void Update() {
            if (Target == null) return;
            Shoot();
        }

        private void Start() {
            if (Target == null) return;
            Shoot();
        }
    }
}