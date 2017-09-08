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
            // bitshift is necessary to actually get the layer number 
            var actualLayer = 1 << LayerMask.NameToLayer("Terrain");
            var boo = Physics.Raycast(ray, out rayHit, distance, actualLayer);
            if (!boo) return false;
            hit = rayHit.point;
            return true;
        }

        public static RectTransform RectTransfromAnchorTopLeft(RectTransform rect) {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            return rect;
        }
    }
}