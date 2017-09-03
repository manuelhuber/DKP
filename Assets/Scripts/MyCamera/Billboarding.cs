using UnityEngine;

namespace Generic {
    [ExecuteInEditMode]
    public class Billboarding : MonoBehaviour {
        public Camera Camera;
        public bool AutoInit = true;

        private void Awake() {
            if (AutoInit) {
                Camera = Camera.main;
            }
        }

        private void Update() {
            transform.LookAt(transform.position + Camera.transform.rotation * Vector3.forward,
                Camera.transform.rotation * Vector3.up);
        }
    }
}