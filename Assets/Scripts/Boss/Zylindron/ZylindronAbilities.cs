using System.Collections.Generic;
using System.Linq;
using Abilities.Scripts;
using Damage;
using Damage.Melee;
using UnityEngine;
using Util;

namespace Boss.Zylindron {
    public class ZylindronAbilities : MonoBehaviour {
        public GameObject FirePrefab;
        public float FireInterval;
        public float FireRange = 10;

        public float VulnerabilityRange = 50;

        private float nextFire;
        private MeleeAttack melee;

        private void Update() {
            melee.AttackNearestTarget();
            if (!(nextFire < Time.time)) return;
            SpawnFire();
            nextFire = Time.time + FireInterval;
        }

        private void SpawnFire() {
            PlayersWithinDistance(VulnerabilityRange).ForEach(o => o.AddComponent<Vulnerability>().Duration = 2);
            var availablePlayers = PlayersWithinDistance(FireRange);
            if (!availablePlayers.Any()) return;
            var target = availablePlayers.ElementAt(Random.Range(0, availablePlayers.Count));
            Vector3 targetPos;
            if (PositionUtil.HighestTerrain(target.transform.position, out targetPos)) {
                Instantiate(FirePrefab, targetPos, FirePrefab.transform.rotation);
            }
        }

        private List<GameObject> PlayersWithinDistance(float distance) {
            return GameObject
                .FindGameObjectsWithTag("Player").ToList()
                .Where(o => WithinDistance(o, distance))
                .Where(o => {
                    var health = o.GetComponent<Damageable>();
                    if (health == null) return false;
                    return !health.IsDead();
                }).ToList();
        }

        private bool WithinDistance(GameObject target, float distance) {
            return PositionUtil.BeelineDistance(transform.position, target.transform.position) < distance;
        }

        private void Awake() {
            nextFire = FireInterval;
        }

        private void Start() {
            melee = GetComponent<MeleeAttack>();
        }
    }
}