using System;
using System.Collections.Generic;
using Control;
using UnityEngine;
using Util;

namespace Damage {
    public class MeleeAttack : Attack {
        public float Range {
            get { return range; }
            set {
                range = value;
                UpdateCollider();
            }
        }

        public float AttackInterval;
        public int AttackDamage;

        [SerializeField] private float range;

        private float nextAttackPossible;
        private Damageable currentTarget;

        private bool inRange;
        private readonly List<GameObject> withinRange = new List<GameObject>();
        private Animator animator;
        private CapsuleCollider rangeCollider;

        #region AttackInterface

        public override void AttackNearestTarget() {
            throw new NotImplementedException();
        }

        public override bool CurrentTargetIsInRange() {
            return inRange;
        }

        public override bool SetTarget(Damageable target) {
            currentTarget = target;
            inRange = target != null && withinRange.Contains(target.gameObject);
            return inRange;
        }

        #endregion

        protected virtual void DealDamage() {
            currentTarget.ModifyHitpoints(-AttackDamage);
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
            var sphereRadius = 2 * Math.Max(scale.x, scale.z) * radius;
            var actHeight = scale.y * 2 + sphereRadius;
            var colHeight = actHeight / scale.y;

            rangeCollider.radius = (float) radius;
            rangeCollider.height = (float) colHeight;
        }

        #region UnityLifecycle

        private void Awake() {
            CreateColliderForCylinder();
            UpdateCollider();
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag);
        }

        private void Update() {
            if (currentTarget == null || !inRange || !(nextAttackPossible < Time.time)) return;
            DealDamage();
            nextAttackPossible = Time.time + AttackInterval;
        }

        private void OnTriggerEnter(Collider other) {
            withinRange.Add(other.gameObject);
            var newTarget = other.gameObject.GetComponent<Damageable>();
            if (newTarget != null && newTarget == currentTarget) inRange = true;
        }

        private void OnTriggerExit(Collider other) {
            withinRange.Remove(other.gameObject);
            if (currentTarget != other.gameObject.GetComponent<Damageable>()) return;
            inRange = false;
        }

        #endregion
    }
}