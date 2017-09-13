using UnityEngine;
using Util;

namespace Damage {
    public class HitScan : Projectile {
        [Tooltip("Positive numbers heal, negative numbers deal damage")] public int Damage;

        private void Shoot() {
            RaycastHit hit;
            if (PositionUtil.RayFromTo(gameObject, target, out hit)) {
                var kill = hit.transform.gameObject.GetComponent<Damageable>();
                if (kill != null) {
                    kill.ModifyHitpoints(Damage);
                }
            }
            Destroy(gameObject);
        }

        private void Update() {
            if (target == null) return;
            Shoot();
        }

        private void Start() {
            if (target == null) return;
            Shoot();
        }
    }
}