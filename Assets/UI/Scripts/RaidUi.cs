using UnityEngine;

namespace UI.Scripts {
    public class RaidUi {
        public static GameObject GetCanvas() {
            return GameObject.FindGameObjectWithTag("Canvas");
        }

        public static AvatarRenderer GetAvatarRenderer() {
            return GetCanvas().GetComponent<AvatarRenderer>();
        }

        public static AbilityRenderer GetAbilityRenderer() {
            return GetCanvas().GetComponent<AbilityRenderer>();
        }
    }
}