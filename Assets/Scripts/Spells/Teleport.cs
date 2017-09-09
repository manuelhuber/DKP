using Control;
using UnityEngine;
using UnityEngine.AI;
using Util;

namespace Spells {
    public class Teleport : Ability {
        public float Distance;

        private GameObject caster;

        public override bool OnActivation(GameObject cas) {
            caster = cas;
            return false;
        }

        public override bool OnLeftClickUp(ClickLocation click) {
            var agent = caster.GetComponent<NavMeshAgent>();
            if (agent == null) return false;
            agent.Warp(WarpLocation());
            var waypoints = caster.GetComponent<WaypointHandler>();
            if (waypoints != null) {
                waypoints.DestroyCurrentWaypoint();
            }
            return true;
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