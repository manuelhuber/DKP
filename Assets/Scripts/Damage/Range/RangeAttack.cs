using System.Collections.Generic;
using System.Linq;
using Control;
using Damage.Common;
using Raid;
using UnityEngine;
using Util;

namespace Damage.Range {
    public class RangeAttack : Attack {
        public GameObject ProjectilePrefab;

        public override bool InRange {
            get { return IsInRange(CurrentTarget); }
        }

        private Damageable nearestTarget;
        private float nextAttackPossible;
        private Animator animator;
        private Damageable status;
        private Team team;

        public override bool AttackNearestTarget() {
            var alreadGotAValidTarget = CurrentTarget != null && CurrentTarget.Targetable;
            if (alreadGotAValidTarget || !(nextAttackPossible < Time.time)) return InRange;
            var findNewTarget = nearestTarget == null
                                || !IsInRange(nearestTarget)
                                || !IsInLineOfSight(nearestTarget.gameObject)
                                || !nearestTarget.Targetable;
            if (findNewTarget) {
                var target = TargetManager.GetEnemies(team)
                    .Where(o => {
                        var dmg = o.GetComponent<Damageable>();
                        return dmg != null && dmg.Targetable;
                    })
                    .Where(IsInLineOfSight)
                    .Aggregate(null, PositionUtil.FindNearest(gameObject.transform.position));
                nearestTarget = target != null ? target.GetComponent<Damageable>() : null;
            }
            CurrentTarget = nearestTarget;
            return InRange;
        }

        private void Shoot() {
            animator.SetTrigger("Attack");
            var projectile = Instantiate(ProjectilePrefab, gameObject.transform.position,
                gameObject.transform.rotation);
            var script = projectile.GetComponent<Projectile>();
            script.SetTarget(CurrentTarget.gameObject);
            script.AffectedTeams = TargetManager.GetEnemyIds(team);
        }

        private bool IsInLineOfSight(GameObject target) {
            RaycastHit hit;
            return PositionUtil.RayFromToHitOnlyTerrain(gameObject.transform.position, target.transform.position,
                out hit);
        }

        private bool IsInRange(Component target) {
            return target != null &&
                   PositionUtil.BeelineDistance(gameObject.transform.position, target.transform.position) <= Range;
        }

        private void Awake() {
            status = GetComponent<Damageable>();
            team = GetComponent<Team>();
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag);
        }

        private void Update() {
            if (CurrentTarget == null || !InRange || !(nextAttackPossible < Time.time)) return;
            if (CurrentTarget.IsDead()) {
                CurrentTarget = null;
                return;
            }
            if (!IsInLineOfSight(CurrentTarget.gameObject)) {
                status.DisplayText("No Line of Sight");
                return;
            }
            Shoot();
            nextAttackPossible = Time.time + AttackInterval;
        }
    }
}