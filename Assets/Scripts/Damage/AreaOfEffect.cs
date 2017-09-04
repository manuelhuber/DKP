using System.Collections.Generic;
using UnityEngine;

namespace Damage {
    public class AreaOfEffect : MonoBehaviour {
        public int Amount;
        public float Intervall;
        public List<int> AffectedTeams;
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
            nextTime += Intervall;
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
            var target = other.gameObject.GetComponent<Damageable>();
            if (target == null) return;
            if (affected.Contains(target)) affected.Remove(target);
        }
    }
}