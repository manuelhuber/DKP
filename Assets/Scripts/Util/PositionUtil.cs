using UnityEngine;

namespace Util {
    public static class PositionUtil {
        public static float DistanceCrowFlies(Vector3 one, Vector3 two) {
            one.y = 0;
            two.y = 0;
            return Vector3.Distance(one, two);
        }

        public static bool ProjectOnTerrain(Vector3 pos, out Vector3 hit) {
            hit = new Vector3();
            pos.y = 50;
            RaycastHit rayHit;
            var ray = new Ray(pos, Vector3.down);
            // bitshift is necessary to actually get the layer number 
            var actualLayer = 1 << LayerMask.NameToLayer("Terrain");
            var boo = Physics.Raycast(ray, out rayHit, 100, actualLayer);
            if (!boo) return false;
            hit = rayHit.point;
            return true;
        }
    }
}