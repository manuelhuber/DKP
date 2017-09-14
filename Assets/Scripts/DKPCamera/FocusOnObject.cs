using UnityEngine;

namespace DKPCamera {
    public class FocusOnObject : MonoBehaviour {
        public void FocusOn(GameObject target) {
            // Cast a ray straight down the center of the camera view
            var cam = Camera.main.transform;
            var ray = new Ray(cam.position, cam.forward * 100);

            // Calculate the vertical difference
            var yDistance = target.transform.position.y - cam.position.y;
            /** 
            cam.forward.y is basically the progress on the y the ray makes with a length of 1. Example: if the vertical 
            distance is 10 and the cam.forward.y=0.5 the ray needs to be 20 long to end exactly on the plane
            */
            var hit = ray.GetPoint(yDistance / cam.forward.y);

            // get the X and Z difference between the hit and the target
            var xDifference = target.transform.position.x - hit.x;
            var zDifference = target.transform.position.z - hit.z;

            // move this object by the difference
            var newCamPos = transform.position;
            newCamPos.x += xDifference;
            newCamPos.z += zDifference;
            transform.position = newCamPos;
        }
    }
}