using Control;
using UnityEngine;
using UnityEngine.AI;

namespace Abilities.Scripts.Portal {
    public class EnterPortal : MonoBehaviour {
        public GameObject Exit;

        private void OnTriggerStay(Collider other) {
            var agent = other.gameObject.GetComponent<NavMeshAgent>();
            if (agent == null) return;
            NavMeshHit hit;
            NavMesh.SamplePosition(
                Exit.transform.position,
                out hit,
                Exit.GetComponent<SphereCollider>().radius,
                -1);
            if (!hit.hit) return;
            var waypoints = other.gameObject.GetComponent<WaypointHandler>();
            if (waypoints != null) {
                waypoints.Warp(hit.position);
            } else {
                agent.Warp(hit.position);
            }
        }
    }
}