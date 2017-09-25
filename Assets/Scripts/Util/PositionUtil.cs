using System;
using UnityEngine;
using UnityEngine.AI;

namespace Util {
    public static class PositionUtil {
        /// <summary>
        /// Calculates the distance between 2 points, ignoring verticality
        /// </summary>
        public static float BeelineDistance(Vector3 one, Vector3 two) {
            one.y = 0;
            two.y = 0;
            return Vector3.Distance(one, two);
        }

        /// <summary>
        /// Gets the highes terrain on the given location
        /// </summary>
        /// <param name="pos">starting position - only X and Z axis will be used</param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool HighestTerrain(Vector3 pos, out Vector3 hit) {
            // We assume there will never be terrain over height 50. If there ever is, this method no longer works
            pos.y = 50;
            return ProjectOnTerrainFromPosition(pos, out hit);
        }

        /// <summary>
        /// Casts a ray straight down from the given location, colliding only with terrain
        /// </summary>
        /// <param name="pos">starting position</param>
        /// <param name="hit">the location of the hit on terrain</param>
        /// <param name="distance">the maximum distance to check for terrain. Default value of 100 should be enough to always detect if there's any terrain</param>
        /// <returns>false if there is no terrain within the specified distance under the position</returns>
        public static bool ProjectOnTerrainFromPosition(Vector3 pos, out Vector3 hit, float distance = 100) {
            hit = new Vector3();
            RaycastHit rayHit;
            var ray = new Ray(pos, Vector3.down);
            var boo = Physics.Raycast(ray, out rayHit, distance, GetTerrainLayerMask());
            if (!boo) return false;
            hit = rayHit.point;
            return true;
        }

//        public static bool CanSeeAnyCorner(GameObject looker, GameObject target) {
//            RaycastHit hit;
//            RayFromTo(looker, target, out hit);
//        }
//
//        public static bool HasLoS(GameObject from, GameObject to) {
//        }

        public static bool RayFromTo(GameObject from, GameObject to, out RaycastHit hit) {
            return RayFromTo(from.transform.position, to.transform.position, out hit);
        }

        public static bool RayFromTo(Vector3 from, Vector3 to, out RaycastHit hit) {
            var ray = new Ray(from, to - from);
            return Physics.Raycast(ray, out hit);
        }

        public static bool RayFromToHitOnlyTerrain(Vector3 from, Vector3 to, out RaycastHit hit) {
            var ray = new Ray(from, to - from);
            return Physics.Raycast(ray, out hit, GetTerrainLayerMask());
        }

        /// <summary>
        /// Casts a ray from the main camera through the cursor and outputs the first clickable object hit aswell as
        /// the first terrain hit
        /// </summary>
        /// <returns>true if terrain has been hit</returns>
        public static bool GetCursorLocation(out GameObject target, out Vector3 terrainHit,
            LayerMask clickable) {
            terrainHit = new Vector3();
            target = null;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            RaycastHit terrainRaycastHit;
            if (!Physics.Raycast(ray, out terrainRaycastHit, 100, GetTerrainLayerMask())) return false;
            terrainHit = terrainRaycastHit.point;
            if (Physics.Raycast(ray, out hit, 100, clickable)) {
                target = hit.transform.gameObject;
            }
            return true;
        }

        /// <summary>
        /// Gets a valid navMesh position, from the starting position towards the cursor position up to the maximum 
        /// distance.
        /// If distance = infinity it will simply finds the valid navMesh position nearest to the cursor position   
        /// </summary>
        /// <param name="startingPos">
        /// The location from where to calculate the distance
        /// </param>
        /// <param name="maxDistance">
        /// The maximum distance that the position can be away from the starting position
        /// </param>
        /// <returns></returns>
        public static Vector3 GetValidNavMeshPosition(Vector3 startingPos, float maxDistance) {
            GameObject go;
            Vector3 location;
            GetCursorLocation(out go, out location, 0);
            var direction = location - startingPos;
            var newPosition = startingPos + Vector3.ClampMagnitude(direction, maxDistance);
            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(newPosition, out navMeshHit, 100, -1);
            return navMeshHit.position;
        }

        public static int GetTerrainLayerMask() {
            // bitshift is necessary to actually get the layer number 
            return 1 << LayerMask.NameToLayer("Terrain");
        }

        /// <summary>
        /// Neat wrapper for use in Aggregate functions
        /// Returns the gameobject that's closes to the given position (beeline distance)
        /// </summary>
        public static Func<GameObject, GameObject, GameObject> FindNearest(Vector3 pos) {
            return (a, b) => NearestGameObject(a, b, pos);
        }

        /// <summary>
        /// Returns the gameobject that's closes to the given position (beeline distance)
        /// </summary>
        public static GameObject NearestGameObject(GameObject one, GameObject two, Vector3 position) {
            var oldDistance = one == null
                ? double.PositiveInfinity
                : BeelineDistance(one.transform.position, position);
            var newDistance =
                BeelineDistance(two.transform.position, position);
            return oldDistance < newDistance ? one : two;
        }

        public static bool Facing(Transform facer, Transform target) {
            var toTarget = (target.position - facer.position).normalized;
            return Vector3.Dot(toTarget, facer.forward) > 0;
        }
    }
}