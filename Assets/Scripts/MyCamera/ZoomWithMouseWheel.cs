using System;
using UnityEngine;

namespace MyCamera {
    public class ZoomWithMouseWheel : MonoBehaviour {
        public float ZoomMultiplier;
        public float FoVMax = 40;
        public float FoVMin = 10;

        private void Update() {
            var scrollValue = Input.GetAxis("Mouse ScrollWheel");
            if (!(Math.Abs(scrollValue) > 0.01)) return;
            var fov = Camera.main.fieldOfView - scrollValue * ZoomMultiplier;
            fov = Mathf.Max(fov, FoVMin);
            fov = Mathf.Min(fov, FoVMax);
            Camera.main.fieldOfView = fov;
        }
    }
}