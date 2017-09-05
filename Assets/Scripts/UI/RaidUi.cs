using UnityEngine;

namespace UI {
    public class RaidUi {
        public static GameObject GetCanvas() {
            return GameObject.FindGameObjectWithTag("Canvas");
        }

        public static AvatarRenderer GetAvatarRenderer() {
            return GetCanvas().GetComponent<AvatarRenderer>();
        }
    }
}