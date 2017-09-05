using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class AvatarRenderer : MonoBehaviour {
        public float SpaceBetweenAvatars = 10;

        private int avatarCount;

        public void InitializeAvater(GameObject prefab, out Slider healthbar, string avaterName) {
            // Create avatar prefab as canvas child
            var canvas = GameObject.FindWithTag("Canvas");
            var avatar = Instantiate(prefab);
            avatar.transform.SetParent(canvas.transform);

            // Set anchor top left
            var rectTransform = avatar.GetComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(0, 1);

            // calculate position based on number of avatars
            var pos = rectTransform.position;
            var offset = SpaceBetweenAvatars + (rectTransform.rect.height + SpaceBetweenAvatars) * avatarCount;
            pos.y = pos.y - offset;
            pos.x = SpaceBetweenAvatars;
            rectTransform.position = pos;

            // set the out-parameter
            healthbar = avatar.GetComponentInChildren<Slider>();

            // Add name
            avatar.GetComponentInChildren<Text>().text = avaterName;

            avatarCount++;
        }
    }
}