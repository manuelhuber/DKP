using UnityEngine;

namespace Util {
    public static class ScreenUtil {
        /// <summary>
        /// Returns a Rect from 2 corner positions, all relative to the screen space 
        /// </summary>
        public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2) {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        /// <summary>
        /// Creates a view-space bound from the given screen-space coordinates. 
        /// </summary>
        public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2) {
            var viewportPoint1 = Camera.main.ScreenToViewportPoint(screenPosition1);
            var viewportPoint2 = Camera.main.ScreenToViewportPoint(screenPosition2);
            var bottomLeftCorner = Vector3.Min(viewportPoint1, viewportPoint2);
            var topRightCorner = Vector3.Max(viewportPoint1, viewportPoint2);
            bottomLeftCorner.z = camera.nearClipPlane;
            topRightCorner.z = camera.farClipPlane;

            var bounds = new Bounds();
            bounds.SetMinMax(bottomLeftCorner, topRightCorner);
            return bounds;
        }
    }
}