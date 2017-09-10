using UnityEngine;

namespace Util {
    public static class PositionUtil {
        /// <summary>
        /// Calculates the distance between 2 points, ignoring verticality
        /// </summary>
        public static float DistanceCrowFlies(Vector3 one, Vector3 two) {
            one.y = 0;
            two.y = 0;
            return Vector3.Distance(one, two);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool ProjectOnTerrainFromSky(Vector3 pos, out Vector3 hit) {
            pos.y = 50;
            return ProjectOnTerrainFromPosition(pos, out hit);
        }

        /// <summary>
        /// Gets the location of the terrain beneth the current position
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

        public static RectTransform RectTransfromAnchorTopLeft(RectTransform rect) {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            return rect;
        }

        public static RectTransform RectTransfromAnchorBottomCenter(RectTransform rect) {
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            return rect;
        }


        /// <summary>
        /// Casts a ray from the main camera through the cursor and outputs the first clickable object hit aswell as
        /// the first terrain hit
        /// </summary>
        /// <returns>true if terrain has been hit</returns>
        public static bool GetClickLocation(out GameObject target, out Vector3 terrainHit,
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

        public static int GetTerrainLayerMask() {
            // bitshift is necessary to actually get the layer number 
            return 1 << LayerMask.NameToLayer("Terrain");
        }
    }
}