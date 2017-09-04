using System;
using System.Collections.Generic;
using MyCamera;
using UnityEngine;
using Util;

namespace Control {
    public class MouseControl : MonoBehaviour {
        public Color SelectionBorderColor;
        public Color SelectionFillColor;
        public LayerMask TerrainLayerMask;
        public LayerMask ClickableLayers;

        private readonly List<PCControl> selected = new List<PCControl>();
        private bool isSelecting;
        private Vector3 selectionStart;
        private FocusOnObject focusOnObject;

        private void Start() {
            focusOnObject = GetComponent<FocusOnObject>();
        }

        private void Update() {
            if (Input.GetKey(KeyCode.Space) && selected.Count > 0) {
                focusOnObject.FocusOn(selected[0].transform.gameObject);
            }

            var rightClick = Input.GetMouseButtonDown(1);
            var leftClick = Input.GetMouseButtonDown(0);
            var leftClickEnd = Input.GetMouseButtonUp(0);
            if (!rightClick && !leftClick && !leftClickEnd) return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            RaycastHit terrainHit;
            Physics.Raycast(ray, out terrainHit, 100, TerrainLayerMask);
            if (!Physics.Raycast(ray, out hit, 100, ClickableLayers)) return;

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
            var newSelect = hit.transform.gameObject.GetComponent<PCControl>();

            if (selected.Contains(newSelect) && focusOnObject != null) {
                focusOnObject.FocusOn(hit.transform.gameObject);
            }

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