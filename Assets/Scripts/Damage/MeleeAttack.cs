using System;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Damage {
    public class MeleeAttack : Attack {
        public float Range;
        public float AttackInterval;
        public int AttackDamage;

        private float nextAttackPossible;
        private Damageable currentTarget;

        private bool inRange;
        private readonly List<GameObject> withinRange = new List<GameObject>();
        private Animator animator;

        #region AttackInterface

        public override void AttackNearestTarget() {
            throw new NotImplementedException();
        }

        public override bool CurrentTargetIsInRange() {
            return inRange;
        }

        public override void SetTarget(Damageable target) {
            currentTarget = target;
            inRange = target != null && withinRange.Contains(target.gameObject);
        }

        #endregion

        protected virtual void DealDamage() {
            currentTarget.ModifyHitpoints(-AttackDamage);
            if (animator == null) return;
            animator.SetTrigger("Attack");
        }

        /// <summary>
        /// Creates a shpere collider where the cylindrical part (without the rounded ends) is as high as the Y scale
        /// of the gameObject
        /// </summary>
        private void CreateColliderForCylinder() {
            // radius=0.5 means the collider has the same diameter as the object. The range is from the edge of the unit.
            double radius = Range + 0.5;
            // this is the fo
            var height = radius * 2 + 2;
            // radius = capsuleRadius * Max(x,z)
            // totalHeight - 2*radius == transform.scale.height*2

            // actHeight = colHeight * scale.height
            // ballHeight = 2*colRadius * scale.Max(x,z)
            // actHeight - ballHeight = scale.height*2

            var scale = gameObject.transform.localScale;
            var ballHeight = 2 * Math.Max(scale.x, scale.z) * radius;
            var actHeight = scale.y * 2 + ballHeight;
            var colHeight = actHeight / scale.y;

            var child = new GameObject {name = "Melee Range"};
            child.transform.SetParent(gameObject.transform, false);
            child.layer = 2;

            var col = child.AddComponent<CapsuleCollider>();
            col.radius = (float) radius;
            col.height = (float) colHeight;
            col.isTrigger = true;
        }

        #region UnityLifecycle

        private void Awake() {
            CreateColliderForCylinder();
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, "PlayerAnimation");
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