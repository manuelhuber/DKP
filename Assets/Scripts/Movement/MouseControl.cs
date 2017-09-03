using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Movement {
    public class MouseControl : MonoBehaviour {
        public Color SelectionBorderColor;
        public Color SelectionFillColor;

        public LayerMask TerrainLayerMask;

        private readonly List<IMouseControllable> selected = new List<IMouseControllable>();
        private bool isSelecting;
        private Vector3 selectionStart;

        private void Update() {
            var rightClick = Input.GetMouseButtonDown(1);
            var leftClick = Input.GetMouseButtonDown(0);
            var leftClickEnd = Input.GetMouseButtonUp(0);
            if (!rightClick && !leftClick && !leftClickEnd) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            RaycastHit terrainHit;
            Physics.Raycast(ray, out terrainHit, 100, TerrainLayerMask);
            if (!Physics.Raycast(ray, out hit, 100)) return;

            if (rightClick) {
                HandleRightClick(hit, terrainHit);
            } else if (leftClick) {
                isSelecting = true;
                selectionStart = Input.mousePosition;
            } else if (leftClickEnd) {
                HandleLeftClickEnd(hit);
            }
        }

        private void HandleLeftClickEnd(RaycastHit hit) {
            // Deselect all
            selected.ForEach(o => o.OnDeselect());
            selected.Clear();

            // Find objects within selection
            var objects = FindObjectsOfType<PCControl>();
            foreach (var o in objects) {
                if (!IsWithinSelectionBounds(o.gameObject)) continue;
                o.OnSelect();
                selected.Add(o);
            }

            // Add click target
            var newSelect = hit.transform.gameObject.GetComponent<IMouseControllable>();
            if (newSelect != null) {
                selected.Add(newSelect);
                newSelect.OnSelect();
            }

            isSelecting = false;
        }

        private void HandleRightClick(RaycastHit hit, RaycastHit terrainHit) {
            if (selected == null) return;
            if (Input.GetKey(KeyCode.LeftShift)) {
                selected.ForEach(o => o.OnRightShiftClick(hit.transform.gameObject, terrainHit.point));
            } else {
                selected.ForEach(c => c.OnRightClick(hit.transform.gameObject, terrainHit.point));
            }
        }

        public bool IsWithinSelectionBounds(GameObject @object) {
            if (!isSelecting) return false;

            var mainCamera = Camera.main;
            var viewportBounds =
                ScreenUtil.GetViewportBounds(mainCamera, selectionStart, Input.mousePosition);

            return viewportBounds.Contains(
                mainCamera.WorldToViewportPoint(@object.transform.position));
        }

        private void OnGUI() {
            if (!isSelecting) return;
            var rect = ScreenUtil.GetScreenRect(selectionStart, Input.mousePosition);
            DrawUtil.DrawScreenBorderedRect(rect, 2, SelectionBorderColor, SelectionFillColor);
        }
    }
}