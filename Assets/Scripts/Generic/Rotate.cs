using UnityEngine;

namespace Generic {
    public class Rotate : MonoBehaviour {
        public int Speed = 10;

        private void Update() {
            transform.localRotation = Quaternion.Euler(Speed * Time.time, Speed * Time.time, Speed * Time.time);
        }
    }
}