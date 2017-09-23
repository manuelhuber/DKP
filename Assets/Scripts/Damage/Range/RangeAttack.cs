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

        public override void AttackNearestTarget() {
            if (CurrentTarget != null || !(nextAttackPossible < Time.time)) return;
            var findNewTarget = nearestTarget == null
                                || !IsInRange(nearestTarget)
                                || !IsInLineOfSight(nearestTarget.gameObject);
            if (findNewTarget) {
                var target = TargetManager.GetEnemies(GetComponent<Team>().TeamId)
                    .Where(IsInLineOfSight)
                    .Aggregate(null, PositionUtil.FindNearest(gameObject.transform.position));
                if (target != null) nearestTarget = target.GetComponent<Damageable>();
            }
            CurrentTarget = nearestTarget;
        }

        private void Shoot() {
            animator.SetTrigger("Attack");
            var projectile = Instantiate(ProjectilePrefab, gameObject.transform.position,
                gameObject.transform.rotation);
            projectile.GetComponent<Projectile>().SetTarget(CurrentTarget.gameObject);
        }

        private bool IsInLineOfSight(GameObject target) {
            RaycastHit hit;
            if (!PositionUtil.RayFromTo(gameObject, target, out hit)) return false;
            return hit.transform.gameObject == target;
        }

        private bool IsInRange(Component target) {
            return target != null &&
                   PositionUtil.BeelineDistance(gameObject.transform.position, target.transform.position) <= Range;
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
            RaycastHit hit;
            if (!PositionUtil.RayFromTo(gameObject, CurrentTarget.gameObject, out hit)) return;
            if (hit.transform.gameObject != CurrentTarget.gameObject) {
                var status = GetComponent<Damageable>();
                if (status != null) {
                    status.DisplayText("No Line of Sight");
                }
                return;
            }
            Shoot();
            nextAttackPossible = Time.time + AttackInterval;
        }
    }
}