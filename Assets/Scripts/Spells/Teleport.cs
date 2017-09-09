using Control;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Spells {
    public class Teleport : Ability {
        public float Distance;

        private GameObject caster;
        private GameObject marker;

        public override bool OnActivation(GameObject cas) {
            caster = cas;
            marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.transform.SetParent(caster.transform, false);
            marker.transform.position = WarpLocation();
            return false;
        }

        public void Update() {
            if (caster == null) return;
            if (marker != null) marker.transform.position = WarpLocation();
        }

        public override bool OnLeftClickUp(ClickLocation click) {
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
            var heading = location - currentPosition;
            var vector = heading / heading.magnitude;
            var actualMovement = heading.magnitude < Distance ? heading : vector * Distance;
            var newPosition = currentPosition + actualMovement;
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(newPosition, out navMeshHit, 100, -1);
            return navMeshHit.position;
        }
    }
}