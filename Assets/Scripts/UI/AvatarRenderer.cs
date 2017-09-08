using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI {
    public class AvatarRenderer : MonoBehaviour {
        public float SpaceBetweenAvatars = 10;

        private int avatarCount;
        private RectTransform folder;
        private GameObject canvas;
        private GameObject folderGo;

        public void InitializeAvater(GameObject prefab, out Slider healthbar, string avaterName) {
            var avatar = Instantiate(prefab);
            avatar.transform.SetParent(folder.transform);
//            folder.GetComponent<RectTransform>().position = new Vector3(-1, -1, -1);

            // Set anchor top left
            var rectTransform = PositionUtil.RectTransfromAnchorTopLeft(avatar.GetComponent<RectTransform>());

            // calculate position based on number of avatars
            var pos = rectTransform.position;
            var offsetTop = SpaceBetweenAvatars + (rectTransform.rect.height + SpaceBetweenAvatars) * avatarCount;
            pos.y = pos.y - offsetTop;
            rectTransform.position = pos;

            // set the out-parameter
            healthbar = avatar.GetComponentInChildren<Slider>();

            // Add name
            avatar.GetComponentInChildren<Text>().text = avaterName;

            avatarCount++;

            Debug.Log(folderGo.GetComponent<RectTransform>().position);
        }

        private void Awake() {
            // Create avatar prefab as canvas child
            canvas = GameObject.FindWithTag("Canvas");

            folderGo = new GameObject("Avatars");
            folderGo.transform.position = new Vector3(0, 0, 0);

            folder = folderGo.AddComponent<RectTransform>();
            folder.SetParent(canvas.transform);
            PositionUtil.RectTransfromAnchorTopLeft(folder);
            var folderPosition = folder.position;
            folderPosition.x = SpaceBetweenAvatars;
            folder.position = folderPosition;
        }
    }
}