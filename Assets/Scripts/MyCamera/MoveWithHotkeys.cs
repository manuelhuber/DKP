using DKPSettings;
using UnityEngine;

namespace MyCamera {
    public class MoveWithHotkeys : MonoBehaviour {
        private int speed;

        private void Start() {
            speed = GeneralSettings.HotkeyCameraScrollingSpeed;
        }

        private void Update() {
            float x = 0;
            float z = 0;
            if (Input.GetKey(Hotkeys.CameraRight)) {
                x += speed * Time.deltaTime; // move on +X axis
            }
            if (Input.GetKey(Hotkeys.CameraLeft)) {
                x -= speed * Time.deltaTime; // move on -X axis
            }
            if (Input.GetKey(Hotkeys.CameraUp)) {
                z += speed * Time.deltaTime; // move on +Z axis
            }
            if (Input.GetKey(Hotkeys.CameraDown)) {
                z -= speed * Time.deltaTime; // move on -Z axis
            }

            transform.position += new Vector3(x, 0, z);
        }
    }
}