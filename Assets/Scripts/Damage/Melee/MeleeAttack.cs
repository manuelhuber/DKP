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

        public float ColliderRadius {
            get { return rangeCollider.radius; }
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
        public float AnimationOffset;

        public bool StopAttack;

        [SerializeField] private float range;

        private Action<MeleeAttack> defaultAttack = MeleeAttackVariants.SingleTargetDamage;
        private Action<MeleeAttack> meleeAttack;

        private float nextAttackPossible;
        private bool attackAnimationInProgress;

        private Animator animator;
        private CapsuleCollider rangeCollider;
        private Team team;

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
            attackAnimationInProgress = false;
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

        protected virtual void Animate() {
            var lookPos = CurrentTarget.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

            if (attackAnimationInProgress || !(nextAttackPossible - AnimationOffset < Time.time)) return;
            attackAnimationInProgress = true;
            animator.SetTrigger("Attack");
        }

        #region UnityLifecycle

        private void Awake() {
            CreateColliderForCylinder();
            UpdateCollider();
            meleeAttack = DefaultAttack;
        }

        private void Start() {
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PcControl.PlayerAnimationTag) ??
                       GetComponent<Animator>();
            team = GetComponent<Team>();
        }

        private void Update() {
            if (CurrentTarget == null || !InRange || StopAttack) return;
            Animate();
            if (nextAttackPossible > Time.time) return;
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
            WithinRange.Remove(other.gameObject.GetComponent<Damageable>());
        }

        #endregion
    }
}