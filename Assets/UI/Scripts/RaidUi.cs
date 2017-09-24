using UnityEngine;

namespace UI.Scripts {
    public class RaidUi {
        private static GameObject _avatarFolder;


        public static GameObject GetCanvas() {
            return GameObject.FindGameObjectWithTag("Canvas");
        }

        public static GameObject GetAvatarFolder() {
            if (_avatarFolder == null) {
                _avatarFolder = GameObject.FindGameObjectWithTag("AvatarFolder");
            }
            return _avatarFolder;
        }

        public static AvatarRenderer GetAvatarRenderer() {
            return GetCanvas().GetComponent<AvatarRenderer>();
        }

        public static AbilityRenderer GetAbilityRenderer() {
            return GetCanvas().GetComponent<AbilityRenderer>();
        }
    }
}