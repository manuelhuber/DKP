using System.Linq;
using Damage;
using Damage.Common;
using Raid;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Enemy {
    public class ChaseNearestEnemy : MonoBehaviour {
        [Tooltip("The time in s between checking for nearer enemies ")] public float ReevaluateInterval = 2;
        private NavMeshAgent agent;
        private Attack attack;
        private Team team;
        private float nextEnemyCheck;

        private void Update() {
            if (NotActive()) return;
            if (nextEnemyCheck <= Time.time) {
                attack.SetTarget(TargetManager.GetEnemies(team)
                    .Aggregate(null, PositionUtil.FindNearest(gameObject.transform.position))
                    .GetComponent<Damageable>());

                nextEnemyCheck = Time.time + ReevaluateInterval;
            }
            agent.isStopped = attack.InRange;
            var t = attack.GetTarget();
            if (t != null) agent.SetDestination(t.gameObject.transform.position);
        }

        private bool NotActive() {
            return agent == null || attack == null || !agent.enabled || !attack.enabled;
        }

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
            attack = GetComponent<Attack>();
            team = GetComponent<Team>();
        }
    }
}