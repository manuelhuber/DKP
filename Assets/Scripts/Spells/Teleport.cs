using Control;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Spells {
    [CreateAssetMenu(menuName = "Spells/Teleport")]
    public class Teleport : Ability {
        public float Distance;
        public GameObject MarkerPrefab;

        private GameObject caster;
        private GameObject marker;

        public override bool OnActivation(GameObject c) {
            caster = c;
            marker = Instantiate(MarkerPrefab);
            marker.transform.SetParent(caster.transform, false);
            marker.transform.position = WarpLocation();
            return false;
        }

        public override void OnUpdate() {
            if (caster == null) {
                if (marker != null) Destroy(marker);
                return;
            }
            if (marker != null) marker.transform.position = WarpLocation();
        }

        public override bool OnLeftClickUp(ClickLocation click) {
            if (caster == null) return false;
            var agent = caster.GetComponent<NavMeshAgent>();
            if (agent == null) return false;
            agent.Warp(WarpLocation());
            Destroy(marker);

            var waypoints = caster.GetComponent<WaypointHandler>();
            if (waypoints != null) {
                waypoints.DestroyCurrentWaypoint();
            }
            return true;
        }

        public override void OnCancel() {
            Destroy(marker);
            caster = null;
        }

        private Vector3 WarpLocation() {
            GameObject go;
            Vector3 location;
            PositionUtil.GetClickLocation(out go, out location, 0);
            var currentPosition = caster.transform.position;
            var direction = location - currentPosition;
            var velocity = direction / direction.magnitude;
            var actualMovement = direction.magnitude < Distance ? direction : velocity * Distance;
            var newPosition = currentPosition + actualMovement;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(newPosition, out navMeshHit, 100, -1);
            return navMeshHit.position;
        }
    }
}