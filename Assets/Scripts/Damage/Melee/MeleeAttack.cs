using System;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Util;

namespace Damage.Melee {
    public class MeleeAttack : Attack {
        public override float Range {
            get { return range; }
            set {
                range = value;
                UpdateCollider();
            }
        }

        public override bool InRange {
            get { return CurrentTarget != null && withinRange.Contains(CurrentTarget); }
        }

        [Tooltip("Positive numbers heal, negative numbers deal damage")] public int AttackDamage;

        private float nextAttackPossible;

        private readonly List<Damageable> withinRange = new List<Damageable>();
        private Animator animator;
        private CapsuleCollider rangeCollider;
        private Team team;
        private float range;

        public override void AttackNearestTarget() {
            if (withinRange.Count < 1) return;
            CurrentTarget = withinRange[0];
        }

        protected virtual void DealDamage() {
            CurrentTarget.ModifyHitpoints(AttackDamage);
            if (animator == null) return;
            animator.SetTrigger("Attack");
        }

        /// <summary>
        /// Creates a capsule collider where the cylindrical part (without the rounded ends) is as high as the Y scale
        /// of the gameObject
        /// </summary>
        private void CreateColliderForCylinder() {
            var child = new GameObject {name = "Melee Range"};
            child.transform.SetParent(gameObject.transform, false);
            child.layer = 2;

            var col = child.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
            rangeCollider = col;
        }

        private void UpdateCollider() {
            // radius=0.5 means the collider has the same diameter as the object. The range is from the edge of the unit.
            var radius = Range + 0.5;

            var scale = gameObject.transform.localScale;
            // The radius of the half-sphere that's on top & bottom of the capsule
            // I had to pull out pen'n'paper and draw some stuff to figure this out!
            var sphereRadius = 2 * Math.Max(scale.x, scale.z) * radius;
            var actualHeight = scale.y * 2 + sphereRadius;
            var colliderHeight = actualHeight / scale.y;

            rangeCollider.radius = (float) radius;
            rangeCollider.height = (float) colliderHeight;
        }

        #region UnityLifecycle

        private void Awake() {
            CreateColliderForCylinder();
            UpdateCollider();
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag);
            team = GetComponent<Team>();
        }

        private void Update() {
            if (CurrentTarget == null || !InRange || !(nextAttackPossible < Time.time)) return;
            if (CurrentTarget.IsDead()) {
                CurrentTarget = null;
                return;
            }
            DealDamage();
            nextAttackPossible = Time.time + AttackInterval;
        }

        private void OnTriggerEnter(Collider other) {
            var dmg = other.gameObject.GetComponent<Damageable>();
            if (dmg == null || team.SameTeam(dmg.gameObject)) return;
            withinRange.Add(dmg);
        }

        private void OnTriggerExit(Collider other) {
            var dmg = other.gameObject.GetComponent<Damageable>();
            if (dmg == null) return;
            withinRange.Remove(dmg);
        }

        #endregion
    }
}