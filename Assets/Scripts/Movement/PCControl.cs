using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Movement {
    public class PCControl : MonoBehaviour, IMouseControllable {
        public GameObject InactiveWaypointMarkerPrefab;
        public GameObject ActiveWaypointMarkerPrefab;
        public Color WaypointLineColor;
        public GameObject SelectionCirclePrefab;

        private NavMeshAgent agent;
        private GameObject selectionCircle;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();

        public void OnSelect() {
            ToggleWaypointRenderer(true);
            selectionCircle = Instantiate(SelectionCirclePrefab);
            selectionCircle.transform.SetParent(transform, false);
        }

        public void OnDeselect() {
            if (Settings.DisplayWaypointsPermanently) return;
            ToggleWaypointRenderer(false);
            Destroy(selectionCircle);
        }

        public void OnLeftClick() {
        }

        public void OnRightClick(GameObject target, Vector3 positionOnTerrain) {
            ClearWaypoints();
            AddWaypoint(positionOnTerrain);
            GoToNextWaypoint();
        }

        public void OnRightShiftClick(GameObject target, Vector3 positionOnTerrain) {
            AddWaypoint(positionOnTerrain);
        }

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update() {
            var arrived = agent.remainingDistance < agent.radius * 2;
            if (arrived) {
                if (currentDestination != null) Destroy(currentDestination);
                if (waypoints.Count < 1) return;
            }

            UpdateCurrentWaypointLine();

            if (!agent.pathPending && arrived && waypoints.Count > 0) {
                GoToNextWaypoint();
            }
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
    }
}