using System;
using System.Collections.Generic;
using DKPCamera;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Control {
    public class WaypointHandler : MonoBehaviour {
        [Header("Waypoints")] [SerializeField] private GameObject moveWaypointMarkerPrefab;
        [SerializeField] private GameObject attackWaypointMarkerPrefab;
        public Color MoveWaypointLineColor;
        public Color AttackWaypointLineColor;
        public float WaypointLineWidth;
        public Animator Animator;

        private NavMeshAgent agent;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();
        private Billboarding billboarding;
        private bool displayWaypoints;

        public bool IsIdle() {
            return waypoints.Count < 1 && !agent.hasPath;
        }

        public void Stop() {
            ClearWaypoints();
            if (!agent.enabled) return;
            agent.ResetPath();
        }

        public void GoDirectlyTo(Vector3 location, bool attackMove = false) {
            ClearWaypoints();
            AddWaypoint(location);
            GoToNextWaypoint(attackMove);
        }

        public void DestroyCurrentWaypoint() {
            Destroy(currentDestination);
            currentDestination = null;
        }

        /// <summary>
        /// Adds a waypoint and renders a line to the previous wapoint
        /// </summary>
        public void AddWaypoint(Vector3 location) {
            GameObject marker;
            var markerWrapper = CreateMarker(moveWaypointMarkerPrefab, location, out marker);
            waypoints.Add(markerWrapper);

            // Connect waypoint to previous waypoint
            var previousWaypoint = waypoints.Count < 2 ? currentDestination : waypoints[waypoints.Count - 2];
            if (previousWaypoint == null) return;
            var lineRenderer = marker.GetComponent<LineRenderer>();
//            lineRenderer.material = new Material(lineRenderer.material) {color = WaypointLineColor};
            lineRenderer.material.color = MoveWaypointLineColor;
            lineRenderer.SetPosition(0, waypoints[waypoints.Count - 1].transform.position);
            lineRenderer.SetPosition(1, previousWaypoint.transform.position);
        }

        private static GameObject CreateMarker(GameObject prefab, Vector3 location, out GameObject marker) {
            var markerWrapper = new GameObject("Waypoint");
            markerWrapper.transform.position = location;
            var markerLocation = location;
            markerLocation.y += prefab.transform.localScale.y / 2;
            marker = Instantiate(
                prefab,
                markerLocation,
                prefab.transform.rotation
            );
            marker.transform.SetParent(markerWrapper.transform);
            return markerWrapper;
        }

        public void ToggleWaypointRenderer(bool value) {
            displayWaypoints = value;
            waypoints.ForEach(o => {
                o.SetActive(value);
                var line = o.GetComponentInChildren<LineRenderer>();
                line.enabled = value;
            });
            if (currentDestinationLineRenderer == null) return;
            currentDestinationLineRenderer.enabled = value;
            UpdateCurrentWaypointLine();
        }

        /// <summary>
        /// Ports the player to the given destination
        /// </summary>
        /// <param name="dest">
        /// This needs to be a valid navMeshPosition!
        /// </param>
        /// <param name="clearCurrentWaypointRange">
        /// If the current waypoint is within this distance it will be 
        /// </param>
        public void Warp(Vector3 dest, float clearCurrentWaypointRange = 5) {
            if (agent.remainingDistance < clearCurrentWaypointRange) GoToNextWaypoint();
            agent.Warp(dest);
            if (currentDestination == null) return;
            agent.SetDestination(currentDestination.transform.position);
        }


        /// <summary>
        /// Makes the next waypoint the current destination
        /// </summary>
        private void GoToNextWaypoint(bool attackMove = false) {
            DestroyCurrentWaypoint();
            if (waypoints.Count <= 0) return;
            var next = waypoints[0];
            var nextPosition = next.transform.position;
            agent.SetDestination(nextPosition);
            GameObject foo;
            var prefab = attackMove ? attackWaypointMarkerPrefab : moveWaypointMarkerPrefab;
            currentDestination = CreateMarker(prefab, nextPosition, out foo);
            currentDestinationLineRenderer = foo.GetComponent<LineRenderer>();
            currentDestinationLineRenderer.material.color =
                attackMove ? AttackWaypointLineColor : MoveWaypointLineColor;
            if (currentDestinationLineRenderer) {
                currentDestinationLineRenderer.SetPosition(0, currentDestination.transform.position);
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }
            currentDestinationLineRenderer.enabled = displayWaypoints;

            Destroy(next);
            waypoints.Remove(next);
        }

        private void ClearWaypoints() {
            Destroy(currentDestination);
            waypoints.ForEach(Destroy);
            waypoints.Clear();
        }

        private void UpdateCurrentWaypointLine() {
            if (currentDestinationLineRenderer != null && currentDestinationLineRenderer.enabled) {
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }
        }

        private void Start() {
            agent = GetComponent<NavMeshAgent>();
            billboarding =
                UnityUtil.FindComponentInChildrenWithTag<Billboarding>(gameObject, PcControl.PlayerAnimationTag);
        }

        private void Update() {
            if (Animator != null) Animator.SetFloat("Speed", agent.velocity.magnitude);
            // Flip character if walking to the left
            if (billboarding != null && Math.Abs(agent.velocity.x) > 1) billboarding.FlipX = agent.velocity.x < 0;
            UpdateCurrentWaypointLine();

            if (!agent.hasPath && waypoints.Count > 0) {
                GoToNextWaypoint();
                return;
            }

            var arrived = agent.hasPath && agent.remainingDistance <= agent.stoppingDistance;
            if (!arrived) return;

            agent.ResetPath();
            GoToNextWaypoint();
        }
    }
}