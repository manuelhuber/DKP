using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Scripts {
    public class AvatarRenderer : MonoBehaviour {
        public float SpaceBetweenAvatars = 10;

        private int avatarCount;

        public void InitializeAvater(GameObject prefab, out Slider healthbar, string avaterName) {
            var avatar = Instantiate(prefab);
            avatar.transform.SetParent(RaidUi.GetAvatarFolder().transform);
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
    }
}