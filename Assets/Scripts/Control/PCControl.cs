﻿using System;
using Damage;
using Damage.Common;
using DKPSettings;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Control {
    internal enum MovementMode {
        Idle,

        // The character moves to the specified location
        Move,

        // The character moves to the specified location and stops to attack any enemies in range
        AttackMove,

        // The character moves to and follows the specified enemy to attack
        Attack
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Team))]
    [RequireComponent(typeof(WaypointHandler))]
    public class PcControl : MouseControllable {
        [Space] [Header("Selection")] public GameObject SelectionCirclePrefab;
        public Material SelectionMaterial;
        public Material FocusMaterial;

        public const string PlayerAnimationTag = "PlayerAnimation";

        private NavMeshAgent agent;
        private GameObject selectionCircle;
        private Damageable health;
        private bool isDead;
        private Animator animator;
        private Attack attack;
        private WaypointHandler waypoints;
        private AbilityHandler abilities;

        private MovementMode currentMode;

        #region MouseControl

        public override void OnSelect() {
            waypoints.ToggleWaypointRenderer(true);
            ToggleSelectionCircle(true, false);
            abilities.Active = false;
        }

        public override void OnFocusSelect() {
            waypoints.ToggleWaypointRenderer(true);
            ToggleSelectionCircle(true, true);
            abilities.Active = true;
        }

        public override void OnDeselect() {
            ToggleSelectionCircle(false, false);
            if (GeneralSettings.DisplayWaypointsPermanently) return;
            waypoints.ToggleWaypointRenderer(false);
            abilities.Active = false;
        }

        public override bool OnLeftClickUp(ClickLocation click) {
            return abilities.OnLeftClickUp(click);
        }

        public override bool OnLeftClickDown(ClickLocation click) {
            return abilities.OnLeftClickDown(click);
        }

        public override bool OnRightClick(ClickLocation click) {
            if (isDead) return false;
            Damageable damageable;
            currentMode = TargetAttackable(click.Target, out damageable) ? MovementMode.Attack : MovementMode.Move;
            var setTarget = damageable == null || damageable.Targetable;
            if (setTarget && attack.SetTarget(damageable)) {
                waypoints.Stop();
                return true;
            }
            waypoints.GoDirectlyTo(click.Location);
            return false;
        }

        public override bool OnRightShiftClick(ClickLocation click) {
            if (isDead) return false;
            waypoints.AddWaypoint(click.Location);
            return false;
        }

        public override void OnButton(string buttonName, ClickLocation mouseLocation) {
            switch (buttonName) {
                case "Stop":
                    waypoints.Stop();
                    break;
                case "Attack Move":
                    currentMode = MovementMode.AttackMove;
                    waypoints.GoDirectlyTo(mouseLocation.Location, true);
                    break;
                default: return;
            }
        }

        private void CheckForDeath() {
            if (!health.IsDead()) return;
            waypoints.Stop();
            agent.enabled = false;
            attack.enabled = false;
            isDead = true;
            abilities.enabled = false;
        }

        private bool CheckForRevive() {
            isDead = health.IsDead();
            if (isDead) return false;
            agent.enabled = true;
            attack.enabled = true;
            animator.SetTrigger("Revive");
            abilities.enabled = true;
            return true;
        }

        private bool TargetAttackable(GameObject target, out Damageable damageable) {
            damageable = null;
            var team = target.GetComponent<Team>();
            if (team == null || team.TeamId == gameObject.GetComponent<Team>().TeamId) return false;
            damageable = target.gameObject.GetComponent<Damageable>();
            return damageable != null && damageable.Targetable;
        }

        private void ToggleSelectionCircle(bool foo, bool focus) {
            selectionCircle.SetActive(foo);
            selectionCircle.GetComponent<Projector>().material =
                focus ? FocusMaterial : SelectionMaterial;
        }

        #endregion

        #region UnityLifeCycle

        private void Awake() {
            selectionCircle = Instantiate(SelectionCirclePrefab);
            selectionCircle.transform.SetParent(transform, false);
            selectionCircle.SetActive(false);
        }

        private void Start() {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Damageable>();
            attack = GetComponent<Attack>();
            waypoints = GetComponent<WaypointHandler>();
            abilities = GetComponent<AbilityHandler>();
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, PlayerAnimationTag);
            waypoints.Animator = animator;
        }

        private void Update() {
            if (isDead && !CheckForRevive()) return;
            CheckForDeath();
            switch (currentMode) {
                case MovementMode.Attack:
                    if (attack.InRange) waypoints.Stop();
                    else if (attack.GetTarget() == null) currentMode = MovementMode.Idle;
                    else waypoints.GoDirectlyTo(attack.GetTarget().gameObject.transform.position);
                    break;
                case MovementMode.Move:
                    if (agent.enabled) agent.isStopped = false;
                    if (waypoints.IsIdle()) currentMode = MovementMode.Idle;
                    break;
                case MovementMode.AttackMove:
                    if (agent.enabled) agent.isStopped = attack.AttackNearestTarget();
                    if (waypoints.IsIdle()) currentMode = MovementMode.Idle;
                    break;
                case MovementMode.Idle:
                    attack.AttackNearestTarget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}