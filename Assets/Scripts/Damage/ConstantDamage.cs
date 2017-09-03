using System.Collections.Generic;
using UnityEngine;

namespace Damage {
    public class ConstantDamage : MonoBehaviour {
        public int DamageAmount;
        public float DamageIntervall;

        private readonly List<Damageable> Damageables = new List<Damageable>();
        private float nextDmg;

        private void Awake() {
            nextDmg = Time.time + DamageIntervall;
        }

        private void Update() {
            if (!(nextDmg <= Time.time)) return;
            nextDmg += DamageIntervall;
            Damageables.ForEach(target => target.CauseDamage(DamageAmount));
        }

        private void OnTriggerEnter(Collider other) {
            var target = other.gameObject.GetComponent<Damageable>();
            if (target == null) return;
            Damageables.Add(target);
        }

        private void OnTriggerExit(Collider other) {
            var target = other.gameObject.GetComponent<Damageable>();
            if (target == null) return;
            Damageables.Remove(target);
        }
    }
}