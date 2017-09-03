using UnityEngine;

namespace Util {
    public static class DrawUtil {
        private static Texture2D _whiteTexture;

        public static Texture2D WhiteTexture {
            get {
                if (_whiteTexture != null) return _whiteTexture;
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
                return _whiteTexture;
            }
        }

        public static void DrawScreenRect(Rect rect, Color color) {
            GUI.color = color;
            GUI.DrawTexture(rect, WhiteTexture);
            GUI.color = Color.white;
        }

        public static void DrawScreenBorder(Rect rect, float thickness, Color color) {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        public static void DrawScreenBorderedRect(Rect rect, float boarderWidth, Color borderColor, Color fillColor) {
            DrawScreenBorder(rect, boarderWidth, borderColor);
            DrawScreenRect(rect, fillColor);
        }
    }
}