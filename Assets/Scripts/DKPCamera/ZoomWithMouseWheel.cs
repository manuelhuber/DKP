using System;
using UnityEngine;

namespace DKPCamera {
    public class ZoomWithMouseWheel : MonoBehaviour {
        public float ZoomMultiplier;
        public float ZoomMax = 40;
        public float ZoomMin = 10;

        private void Update() {
            var scrollValue = Input.GetAxis("Mouse ScrollWheel");
            if (!(Math.Abs(scrollValue) > 0.01)) return;
            var size = Camera.main.orthographicSize - scrollValue * ZoomMultiplier;
            size = Mathf.Max(size, ZoomMin);
            size = Mathf.Min(size, ZoomMax);
            Camera.main.orthographicSize = size;
        }
    }
}