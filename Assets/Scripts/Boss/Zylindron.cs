﻿using System.Collections.Generic;
using System.Linq;
using Damage;
using Spells;
using UnityEngine;
using Util;

namespace Boss {
    public class Zylindron : MonoBehaviour {
        public GameObject FirePrefab;
        public float FireInterval;
        public float FireRange = 10;

        public float VulnerabilityRange = 50;

        private float nextFire;

        private void Awake() {
            nextFire = FireInterval;
        }

        private void Update() {
            if (nextFire < Time.time) {
                SpawnFire();
                nextFire += FireInterval;
            }
        }

        private void SpawnFire() {
            PlayersWithinDistance(VulnerabilityRange).ForEach(o => o.AddComponent<Vulnerability>().Duration = 2);
            var availablePlayers = PlayersWithinDistance(FireRange);
            if (!availablePlayers.Any()) return;
            var target = availablePlayers.ElementAt(Random.Range(0, availablePlayers.Count));
            Vector3 targetPos;
            if (PositionUtil.ProjectOnTerrain(target.transform.position, out targetPos)) {
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
            return PositionUtil.DistanceCrowFlies(transform.position, target.transform.position) < distance;
        }
    }
}