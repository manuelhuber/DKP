using System.Collections.Generic;
using Damage;
using DKPSettings;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Control {
    internal class Waypoint {
        public GameObject Target;
        public bool IsMobile;
    }

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Team))]
    public class PcControl : MouseControllable {
        [Header("Waypoints")] public GameObject InactiveWaypointMarkerPrefab;
        public GameObject ActiveWaypointMarkerPrefab;
        public Color WaypointLineColor;
        public float WaypointLineWidth;

        [Space] [Header("Selection")] public GameObject SelectionCirclePrefab;
        public Material SelectionMaterial;
        public Material FocusMaterial;

        private NavMeshAgent agent;
        private GameObject selectionCircle;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();
        private Damageable health;
        private bool disabled;
        private Animator animator;
        private Attack attack;

        #region MouseControl

        public override void OnSelect() {
            ToggleWaypointRenderer(true);
            ToggleSelectionCircle(true, false);
        }

        public override void OnFocusSelect() {
            ToggleWaypointRenderer(true);
            ToggleSelectionCircle(true, true);
        }

        public override void OnDeselect() {
            ToggleSelectionCircle(false, false);
            if (GeneralSettings.DisplayWaypointsPermanently) return;
            ToggleWaypointRenderer(false);
        }

        public override bool OnRightClick(ClickLocation click) {
            if (disabled) return false;
            Damageable damageable;
            TargetAttackable(click.Target, out damageable);
            attack.SetTarget(damageable);
            ClearWaypoints();
            AddWaypoint(click);
            GoToNextWaypoint();
            return false;
        }

        public override bool OnRightShiftClick(ClickLocation click) {
            if (disabled) return false;
            AddWaypoint(click);
            return false;
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
            animator = UnityUtil.FindComponentInChildrenWithTag<Animator>(gameObject, "PlayerAnimation");
        }


        private void Update() {
            if (disabled) {
                disabled = health.IsDead();
                if (disabled) return;
                agent.enabled = true;
                animator.SetTrigger("Revive");
            }
            if (animator != null) animator.SetFloat("Speed", agent.velocity.magnitude);
            if (health.IsDead()) {
                agent.enabled = false;
                Destroy(currentDestination);
                disabled = true;
                return;
            }
            UpdateCurrentWaypointLine();
            var arrived = agent.hasPath && agent.remainingDistance <= agent.stoppingDistance;
            if (!agent.hasPath && waypoints.Count > 0) {
                GoToNextWaypoint();
                return;
            }
            if (!arrived) return;

            Destroy(currentDestination);
            agent.ResetPath();
            if (waypoints.Count < 1) return;
            GoToNextWaypoint();
        }

        #endregion

        #region Waypoints

        /// <summary>
        /// Makes the next waypoint the current destination
        /// </summary>
        private void GoToNextWaypoint() {
            var next = waypoints[0];
            var nextPosition = next.transform.position;
            agent.SetDestination(nextPosition);
            currentDestination = Instantiate(ActiveWaypointMarkerPrefab, nextPosition, Quaternion.identity);
            currentDestinationLineRenderer = currentDestination.GetComponent<LineRenderer>();
            currentDestinationLineRenderer.enabled = false;
            if (currentDestinationLineRenderer) {
                currentDestinationLineRenderer.SetPosition(0, currentDestination.transform.position);
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }

            Destroy(next);
            waypoints.Remove(next);
        }

        private void ClearWaypoints() {
            Destroy(currentDestination);
            waypoints.ForEach(Destroy);
            waypoints.Clear();
        }

        /// <summary>
        /// Adds a waypoint and renders a line to the previous wapoint
        /// </summary>
        private void AddWaypoint(ClickLocation clickLocation) {
            var markerLocation = clickLocation.Location;
            markerLocation.y += InactiveWaypointMarkerPrefab.transform.localScale.y / 2;
            var marker = Instantiate(
                InactiveWaypointMarkerPrefab,
                markerLocation,
                InactiveWaypointMarkerPrefab.transform.rotation
            );
            waypoints.Add(marker);
            // Connect waypoint to previous waypoint
            var lineRenderer = marker.AddComponent<LineRenderer>();
            lineRenderer.startColor = WaypointLineColor;
            lineRenderer.endColor = WaypointLineColor;
            lineRenderer.startWidth = WaypointLineWidth;
            lineRenderer.endWidth = WaypointLineWidth;
            Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
            lineRenderer.material = whiteDiffuseMat;

            var previousWaypoint = waypoints.Count < 2 ? currentDestination : waypoints[waypoints.Count - 2];
            if (previousWaypoint == null) return;
            lineRenderer.SetPosition(0, waypoints[waypoints.Count - 1].transform.position);
            lineRenderer.SetPosition(1, previousWaypoint.transform.position);
            currentDestinationLineRenderer.enabled = true;
        }

        private void UpdateCurrentWaypointLine() {
            if (currentDestinationLineRenderer != null && currentDestinationLineRenderer.enabled) {
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }
        }

        private void ToggleWaypointRenderer(bool value) {
            waypoints.ForEach(o => {
                o.SetActive(value);
                var line = o.GetComponent<LineRenderer>();
                line.enabled = value;
            });
        }

        #endregion

        private bool TargetAttackable(GameObject target, out Damageable damageable) {
            damageable = null;
            var team = target.GetComponent<Team>();
            if (team == null || team.TeamId == gameObject.GetComponent<Team>().TeamId) return false;
            damageable = target.gameObject.GetComponent<Damageable>();
            return damageable != null;
        }

        private void ToggleSelectionCircle(bool foo, bool focus) {
            selectionCircle.SetActive(foo);
            selectionCircle.GetComponent<Projector>().material =
                focus ? FocusMaterial : SelectionMaterial;
        }
        
    }
}