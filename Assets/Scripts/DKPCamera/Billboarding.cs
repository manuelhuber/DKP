using UnityEngine;

namespace DKPCamera {
    [ExecuteInEditMode]
    public class Billboarding : MonoBehaviour {
        public Camera Camera;
        public bool AutoInit = true;
        public bool FlipX;

        private void Awake() {
            if (AutoInit) {
                Camera = Camera.main;
            }
        }

        private void Update() {
            var flip = FlipX ? -1 : 1;
            transform.LookAt(transform.position + Camera.transform.rotation * Vector3.forward * flip,
                Camera.transform.rotation * Vector3.up);
        }
    }
}