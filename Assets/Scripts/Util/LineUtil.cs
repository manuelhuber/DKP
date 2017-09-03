using UnityEngine;

namespace Util {
    public class LineUtil {
        public void WaypointLine(Vector3 start, Vector3 end) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, end);
        }
    }
}