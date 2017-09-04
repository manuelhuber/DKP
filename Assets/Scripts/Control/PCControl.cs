using System.Collections.Generic;
using Damage;
using DKPSettings;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Control {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Damageable))]
    public class PCControl : MouseControllable {
        public GameObject InactiveWaypointMarkerPrefab;
        public GameObject ActiveWaypointMarkerPrefab;
        public Color WaypointLineColor;
        public GameObject SelectionCirclePrefab;
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

        public override bool OnRightClick(GameObject target, Vector3 positionOnTerrain) {
            if (disabled) return false;
            ClearWaypoints();
            AddWaypoint(positionOnTerrain);
            GoToNextWaypoint();
            return false;
        }

        public override bool OnRightShiftClick(GameObject target, Vector3 positionOnTerrain) {
            if (disabled) return false;
            AddWaypoint(positionOnTerrain);
            return false;
        }

        private void Awake() {
            selectionCircle = Instantiate(SelectionCirclePrefab);
            selectionCircle.transform.SetParent(transform, false);
        }

        private void Start() {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Damageable>();
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

        /// <summary>
        /// Makes the next waypoint the current destination
        /// </summary>
        private void GoToNextWaypoint() {
            var next = waypoints[0];
            var nextPosition = next.transform.position;
            agent.SetDestination(nextPosition);
            currentDestination = Instantiate(ActiveWaypointMarkerPrefab, nextPosition, Quaternion.identity);
            currentDestinationLineRenderer = currentDestination.GetComponent<LineRenderer>();
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
        /// <param name="position"></param>
        private void AddWaypoint(Vector3 position) {
            var marker = Instantiate(InactiveWaypointMarkerPrefab, position, Quaternion.identity);
            waypoints.Add(marker);
            // Connect waypoint to previous waypoint
            var lineRenderer = marker.GetComponent<LineRenderer>();
            var previousWaypoint = waypoints.Count < 2 ? currentDestination : waypoints[waypoints.Count - 2];
            if (lineRenderer == null || previousWaypoint == null) return;
            lineRenderer.SetPosition(0, waypoints[waypoints.Count - 1].transform.position);
            lineRenderer.SetPosition(1, previousWaypoint.transform.position);
        }

        private void UpdateCurrentWaypointLine() {
            if (currentDestinationLineRenderer != null) {
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }
        }

        private void ToggleWaypointRenderer(bool value) {
            waypoints.ForEach(o => {
                o.GetComponent<Renderer>().enabled = value;
                var line = o.GetComponent<LineRenderer>();
                line.enabled = value;
            });
        }

        private void ToggleSelectionCircle(bool foo, bool focus) {
            selectionCircle.SetActive(foo);
            selectionCircle.GetComponent<Projector>().material =
                focus ? FocusMaterial : SelectionMaterial;
        }
    }
}