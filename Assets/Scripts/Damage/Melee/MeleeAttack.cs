using System;
using System.Collections.Generic;
using Control;
using Damage.Common;
using UnityEngine;
using Util;

namespace Damage.Melee {
    public class MeleeAttack : Attack {
        [Tooltip("Positive numbers heal, negative numbers deal damage")] public int AttackDamage;

        public override float Range {
            get { return range; }
            set {
                range = value;
                UpdateCollider();
            }
        }

        public override bool InRange {
            get { return CurrentTarget != null && WithinRange.Contains(CurrentTarget); }
        }

        public Action<MeleeAttack> DefaultAttack {
            get { return defaultAttack; }
            set {
                var old = defaultAttack;
                defaultAttack = value;
                // If the current attack is the old default update to new default
                if (meleeAttack == old) meleeAttack = value;
            }
        }

        public readonly List<Damageable> WithinRange = new List<Damageable>();
        private Action<MeleeAttack> defaultAttack = MeleeAttackVariants.SingleTargetDamage;

        private float nextAttackPossible;
        private Action<MeleeAttack> meleeAttack;
        private Animator animator;
        private CapsuleCollider rangeCollider;
        private Team team;
        [SerializeField] private float range;

        public void ChangeAttackForDuration(Action<MeleeAttack> attack, float duration) {
            meleeAttack = attack;
            DoAfterDelay(() => meleeAttack = DefaultAttack, duration);
        }

        public override bool AttackNearestTarget() {
            if (WithinRange.Count < 1) return false;
            CurrentTarget = WithinRange.Find(d => d.Targetable);
            return InRange;
        }

        protected virtual void DealDamage() {
            meleeAttack(this);
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
            if (rangeCollider == null) CreateColliderForCylinder();
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
            meleeAttack = DefaultAttack;
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag);
            team = GetComponent<Team>();
        }

        private void Update() {
            if (CurrentTarget == null || !InRange || !(nextAttackPossible < Time.time)) return;

            if (!CurrentTarget.Targetable) {
                CurrentTarget = null;
                return;
            }
            DealDamage();
            nextAttackPossible = Time.time + AttackInterval;
        }

        private void OnTriggerEnter(Collider other) {
            var dmg = other.gameObject.GetComponent<Damageable>();
            if (dmg == null || team.SameTeam(dmg.gameObject)) return;
            WithinRange.Add(dmg);
        }

        private void OnTriggerExit(Collider other) {
            var dmg = other.gameObject.GetComponent<Damageable>();
            if (dmg == null) return;
            WithinRange.Remove(dmg);
        }

        #endregion
    }
}