using UnityEngine;

namespace Damage.Range {
    public class TargetSeekingProjectile : Projectile {
        public float Speed = 5;

        private void Update() {
            if (Target == null) return;
            transform.LookAt(Target.transform.position);
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }
    }
}