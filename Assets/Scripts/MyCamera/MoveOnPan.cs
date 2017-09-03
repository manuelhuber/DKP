using UnityEngine;

namespace MyCamera {
    public class MoveOnPan : MonoBehaviour {
        public int Boundary = 50;
        public int Speed = 5;

        private int screenWidth;
        private int screenHeight;

        private void Start() {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        private void Update() {
            float x = 0;
            float z = 0;
            if (Input.GetKey(KeyCode.RightArrow) || Input.mousePosition.x > screenWidth - Boundary) {
                x += Speed * Time.deltaTime; // move on +X axis
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.mousePosition.x < Boundary) {
                x -= Speed * Time.deltaTime; // move on -X axis
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.mousePosition.y > screenHeight - Boundary) {
                z += Speed * Time.deltaTime; // move on +Z axis
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.mousePosition.y < Boundary) {
                z -= Speed * Time.deltaTime; // move on -Z axis
            }

            transform.position += new Vector3(x, 0, z);
        }
    }
}