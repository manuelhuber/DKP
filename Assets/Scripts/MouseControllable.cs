using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MouseControllable : MonoBehaviour {
    public GameObject InactiveWaypointMarker;
    public GameObject ActiveWaypointMarker;
    public Color WaypointLineColor;

    private NavMeshAgent agent;
    private GameObject currentDestination;
    private readonly List<GameObject> waypoints = new List<GameObject>();
    private bool abortWaypoint = false;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnLeftClick() {
        Debug.Log("I've been clicked");
    }

    public void OnRightClick(Vector3 position) {
        ClearWaypoints();
        AddWaypoint(position);
        abortWaypoint = true;
    }

    public void OnRightShiftClick(Vector3 position) {
        AddWaypoint(position);
    }

    private void Update() {
        if (!abortWaypoint && (waypoints.Count == 0 || agent.pathPending || agent.remainingDistance > 0.5)) return;
        abortWaypoint = false;
        GoToNextWaypoint();
    }

    private void GoToNextWaypoint() {
        Destroy(currentDestination);
        var next = waypoints[0];
        var nextPosition = next.transform.position;
        agent.SetDestination(nextPosition);
        currentDestination = Instantiate(ActiveWaypointMarker, nextPosition, Quaternion.identity);
        Destroy(next);
        waypoints.Remove(next);
    }

    private void ClearWaypoints() {
        Destroy(currentDestination);
        waypoints.ForEach(Destroy);
        waypoints.Clear();
    }

    private void AddWaypoint(Vector3 position) {
        var marker = Instantiate(InactiveWaypointMarker, position, Quaternion.identity);
        waypoints.Add(marker);
    }
}