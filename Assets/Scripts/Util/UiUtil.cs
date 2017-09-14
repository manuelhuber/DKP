using UnityEngine;

namespace Util {
    public class UiUtil {
        public static RectTransform RectTransfromAnchorTopLeft(RectTransform rect) {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            return rect;
        }

        public static RectTransform RectTransfromAnchorBottomCenter(RectTransform rect) {
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            return rect;
        }
    }
}