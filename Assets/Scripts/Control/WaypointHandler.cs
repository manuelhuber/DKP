using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Control {
    public class WaypointHandler : MonoBehaviour {
        [Header("Waypoints")] public GameObject InactiveWaypointMarkerPrefab;
        public GameObject ActiveWaypointMarkerPrefab;
        public Color WaypointLineColor;
        public float WaypointLineWidth;

        private NavMeshAgent agent;
        private GameObject currentDestination;
        private LineRenderer currentDestinationLineRenderer;
        private readonly List<GameObject> waypoints = new List<GameObject>();

        private void Start() {
            agent = GetComponent<NavMeshAgent>();
        }

        #region Waypoints

        /// <summary>
        /// Makes the next waypoint the current destination
        /// </summary>
        public void GoToNextWaypoint() {
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
        public void AddWaypoint(ClickLocation clickLocation) {
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
    }
}