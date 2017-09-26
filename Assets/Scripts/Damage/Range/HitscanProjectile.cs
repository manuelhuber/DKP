using System.Collections.Generic;
using Damage.Common;
using UnityEngine;
using Util;

namespace Damage.Range {
    public class HitscanProjectile : Projectile {
        [Tooltip("Positive numbers heal, negative numbers deal damage")] public int Damage;
        public float Range = 100;

        private void Shoot() {
            var hits = PositionUtil.RayAllFromTo(gameObject, Target, Range);
            foreach (var hit in hits) {
                var dmg = hit.transform.gameObject.GetComponent<Damageable>();
                var team = hit.transform.gameObject.GetComponent<Team>();
                if (dmg == null || !AffectedTeams.Contains(team.TeamId)) continue;
                dmg.ModifyHitpoints(Damage);
                break;
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