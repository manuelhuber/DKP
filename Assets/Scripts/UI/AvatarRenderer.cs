using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI {
    public class AvatarRenderer : MonoBehaviour {
        public float SpaceBetweenAvatars = 10;

        private int avatarCount;
        private Transform folder;

        public void InitializeAvater(GameObject prefab, out Slider healthbar, string avaterName) {
            var avatar = Instantiate(prefab);
            avatar.transform.SetParent(folder);
            var rectTransform = UiUtil.RectTransfromAnchorTopLeft(avatar.GetComponent<RectTransform>());

            // calculate position based on number of avatars
            var offsetTop = (rectTransform.rect.height + SpaceBetweenAvatars) * -avatarCount;
            UnityUtil.SetAnchoredPosition(rectTransform, rectTransform.anchoredPosition.x, offsetTop);

            // set the out-parameter
            healthbar = avatar.GetComponentInChildren<Slider>();

            // Add name
            avatar.GetComponentInChildren<Text>().text = avaterName;

            avatarCount++;
        }

        private void Awake() {
            folder = GameObject.FindWithTag("AvatarFolder").transform;
        }
    }
}