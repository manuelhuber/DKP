﻿using System;
using System.Collections.Generic;
using DKPCamera;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Control {
    internal class Waypoint {
        public GameObject Target;
        public bool IsMobile;
    }

    public class WaypointHandler : MonoBehaviour {
        [Header("Waypoints")] public GameObject InactiveWaypointMarkerPrefab;
        public GameObject ActiveWaypointMarkerPrefab;
        public Color WaypointLineColor;
        public float WaypointLineWidth;
        public Animator Animator;

        private NavMeshAgent agent;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();
        private Billboarding billboarding;

        public bool IsIdle() {
            return waypoints.Count < 1 && !agent.hasPath;
        }

        public void Stop() {
            ClearWaypoints();
            if (!agent.enabled) return;
            agent.ResetPath();
        }

        public void GoDirectlyTo(Vector3 location) {
            ClearWaypoints();
            AddWaypoint(location);
            GoToNextWaypoint();
        }

        public void DestroyCurrentWaypoint() {
            Destroy(currentDestination);
        }

        /// <summary>
        /// Adds a waypoint and renders a line to the previous wapoint
        /// </summary>
        public void AddWaypoint(Vector3 location) {
            GameObject marker;
            var markerWrapper = CreateMarker(InactiveWaypointMarkerPrefab, location, out marker);
            waypoints.Add(markerWrapper);

            // Connect waypoint to previous waypoint
            var previousWaypoint = waypoints.Count < 2 ? currentDestination : waypoints[waypoints.Count - 2];
            if (previousWaypoint == null) return;
            var lineRenderer = marker.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, waypoints[waypoints.Count - 1].transform.position);
            lineRenderer.SetPosition(1, previousWaypoint.transform.position);
            currentDestinationLineRenderer.enabled = true;
        }

        private GameObject CreateMarker(GameObject prefab, Vector3 location, out GameObject marker) {
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
            waypoints.ForEach(o => {
                o.SetActive(value);
                var line = o.GetComponentInChildren<LineRenderer>();
                line.enabled = value;
            });
            if (currentDestinationLineRenderer == null) return;
            currentDestinationLineRenderer.enabled = value;
        }


        /// <summary>
        /// Makes the next waypoint the current destination
        /// </summary>
        private void GoToNextWaypoint() {
            if (waypoints.Count <= 0) return;
            var next = waypoints[0];
            var nextPosition = next.transform.position;
            agent.SetDestination(nextPosition);
            GameObject foo;
            currentDestination = CreateMarker(ActiveWaypointMarkerPrefab, nextPosition, out foo);
            var showLine = currentDestinationLineRenderer == null || currentDestinationLineRenderer.enabled;
            currentDestinationLineRenderer = foo.GetComponent<LineRenderer>();
            if (currentDestinationLineRenderer) {
                currentDestinationLineRenderer.SetPosition(0, currentDestination.transform.position);
                currentDestinationLineRenderer.SetPosition(1, transform.position);
            }
            currentDestinationLineRenderer.enabled = showLine;

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

            DestroyCurrentWaypoint();
            agent.ResetPath();
            GoToNextWaypoint();
        }
    }
}