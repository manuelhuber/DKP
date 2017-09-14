using DKPSettings;
using UnityEngine;

namespace DKPCamera {
    public class MoveOnPan : MonoBehaviour {
        public int Boundary = 50;

        private int speed;
        private int screenWidth;
        private int screenHeight;

        private void Awake() {
            speed = GeneralSettings.MouseCameraScrollingSpeed;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        private void Update() {
            float x = 0;
            float z = 0;
            if (Input.mousePosition.x > screenWidth - Boundary) {
                x += speed * Time.deltaTime; // move on +X axis
            }
            if (Input.mousePosition.x < Boundary) {
                x -= speed * Time.deltaTime; // move on -X axis
            }
            if (Input.mousePosition.y > screenHeight - Boundary) {
                z += speed * Time.deltaTime; // move on +Z axis
            }
            if (Input.mousePosition.y < Boundary) {
                z -= speed * Time.deltaTime; // move on -Z axis
            }

            transform.position += new Vector3(x, 0, z);
        }
    }
}