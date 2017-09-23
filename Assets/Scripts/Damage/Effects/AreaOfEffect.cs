using System.Collections.Generic;
using Damage.Common;
using UnityEngine;

namespace Damage.Effects {
    public class AreaOfEffect : Effect {
        public int Amount;
        public float Intervall;
        public int Lifetime;

        private readonly List<Damageable> affected = new List<Damageable>();
        private float nextTime;

        private void Awake() {
            if (Lifetime > 0) {
                Destroy(gameObject, Lifetime);
            }
            nextTime = Time.time + Intervall;
        }

        private void Update() {
            if (!(nextTime <= Time.time)) return;
            nextTime = Time.time + Intervall;
            affected.ForEach(target => target.ModifyHitpoints(Amount));
        }

        private void OnTriggerEnter(Collider other) {
            var target = other.gameObject.GetComponent<Damageable>();
            if (target == null) return;
            if (AffectedTeams.Contains(other.gameObject.GetComponent<Team>().TeamId)) {
                affected.Add(target);
            }
        }

        private void OnTriggerExit(Collider other) {
            affected.Remove(other.gameObject.GetComponent<Damageable>());
        }
    }
}