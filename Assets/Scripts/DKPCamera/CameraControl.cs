using UnityEngine;

namespace DKPCamera {
    public class CameraControl : MonoBehaviour {
        //
        // VARIABLES
        //

        public float TurnSpeed = 4.0f; // Speed of camera turning when mouse moves in along an axis
        public float PanSpeed = 4.0f; // Speed of the camera when being panned
        public float ZoomSpeed = 4.0f; // Speed of the camera going back and forth

        private Vector3 mouseOrigin; // Position of cursor when mouse dragging starts
        private bool isPanning; // Is the camera being panned?
        private bool isRotating; // Is the camera being rotated?
        private bool isZooming; // Is the camera zooming?

        //
        // UPDATE
        //

        private void Update() {
            // Get the left mouse button
            if (Input.GetMouseButtonDown(0)) {
                // Get mouse origin
                mouseOrigin = Input.mousePosition;
                isRotating = true;
            }

            // Get the right mouse button
            if (Input.GetMouseButtonDown(1)) {
                // Get mouse origin
                mouseOrigin = Input.mousePosition;
                isPanning = true;
            }

            // Get the middle mouse button
            if (Input.GetMouseButtonDown(2)) {
                // Get mouse origin
                mouseOrigin = Input.mousePosition;
                isZooming = true;
            }

            // Disable movements on button release
            if (!Input.GetMouseButton(0)) isRotating = false;
            if (!Input.GetMouseButton(1)) isPanning = false;
            if (!Input.GetMouseButton(2)) isZooming = false;

            // Rotate camera along X and Y axis
            if (isRotating) {
                var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

                transform.RotateAround(transform.position, transform.right, -pos.y * TurnSpeed);
                transform.RotateAround(transform.position, Vector3.up, pos.x * TurnSpeed);
            }

            // Move the camera on it's XY plane
            if (isPanning) {
                var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

                var move = new Vector3(pos.x * PanSpeed, pos.y * PanSpeed, 0);
                transform.Translate(move, Space.Self);
            }

            // Move the camera linearly along Z axis
            if (isZooming) {
                var pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

                var move = pos.y * ZoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            }
        }
    }
}