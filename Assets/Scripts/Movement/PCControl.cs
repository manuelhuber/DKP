using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Movement {
    public class PCControl : MonoBehaviour, IMouseControllable {
        public GameObject InactiveWaypointMarker;
        public GameObject ActiveWaypointMarker;
        public Color WaypointLineColor;

        private NavMeshAgent agent;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();
        private bool abortWaypoint;

        public void OnSelect() {
            waypoints.ForEach(o => {
                o.GetComponent<Renderer>().enabled = true;
                var line = o.GetComponent<LineRenderer>();
                line.enabled = true;
            });
        }

        public void OnDeselect() {
            if (Settings.DisplayWaypointsPermanently) return;
            waypoints.ForEach(o => {
                o.GetComponent<Renderer>().enabled = false;
                var line = o.GetComponent<LineRenderer>();
                line.enabled = false;
            });
        }

        public void OnLeftClick() {
            OnSelect();
        }

        public void OnRightClick(GameObject target, Vector3 positionOnTerrain) {
            ClearWaypoints();
            AddWaypoint(positionOnTerrain);
            abortWaypoint = true;
        }

        public void OnRightShiftClick(GameObject target, Vector3 positionOnTerrain) {
            AddWaypoint(positionOnTerrain);
        }

        private void Awake() {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update() {
            UpdateCurrentWaypointLine();

            var arrived = agent.remainingDistance < agent.radius * 2;

            if (arrived && currentDestination != null) Destroy(currentDestination);

            if (abortWaypoint || (!agent.pathPending && arrived && waypoints.Count > 0)) {
                abortWaypoint = false;
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
            currentDestination = Instantiate(ActiveWaypointMarker, nextPosition, Quaternion.identity);
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
            var marker = Instantiate(InactiveWaypointMarker, position, Quaternion.identity);
            waypoints.Add(marker);
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
    }
}