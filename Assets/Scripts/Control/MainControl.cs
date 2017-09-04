using System.Collections.Generic;
using Damage;
using DKPSettings;
using MyCamera;
using UnityEngine;
using Util;

namespace Control {
    /// <summary>
    /// The main methods of control
    /// Holds the "selected units" state
    /// </summary>
    public class MainControl : MonoBehaviour {
        public Color SelectionBorderColor;
        public Color SelectionFillColor;

        /// <summary>
        /// The layer mask of navigatable terrains. MouseControllable objects get these when receiving clicks
        /// </summary>
        public LayerMask TerrainLayerMask;

        /// <summary>
        /// All layers that should be clickable - either to be selected or be the target of clicks
        /// </summary>
        public LayerMask ClickableLayers;

        /// <summary>
        /// A group of all selected controllables
        /// </summary>
        private readonly List<MouseControllable> selected = new List<MouseControllable>();

        /// <summary>
        /// The focus of the selection - either because it's the only selected or because it has been chosen
        /// Only the focused selected receives left click events
        /// This needs to be an element from the selected list!
        /// </summary>
        private MouseControllable focusedSelected;

        private bool isSelecting;
        private Vector3 selectionStart;

        /// <summary>
        /// A script to focus the camera on an object
        /// </summary>
        private FocusOnObject cameraController;

        private void Start() {
            cameraController = GetComponent<FocusOnObject>();
        }

        private void Update() {
            HandleHotkeys();

            var rightClick = Input.GetMouseButton(1);
            var leftClickDown = Input.GetMouseButtonDown(0);
            var leftClickUp = Input.GetMouseButtonUp(0);
            if (!rightClick && !leftClickDown && !leftClickUp) return;

            GameObject target;
            Vector3 terrainHit;
            if (!GetClickLocation(out target, out terrainHit)) return;

            if (rightClick) {
                // Currently there is no default behaviour for right clicks so just call the handlers
                if (Input.GetKey(Hotkeys.AddModifier)) {
                    selected.ForEach(o => o.OnRightShiftClick(target, terrainHit));
                } else {
                    selected.ForEach(c => c.OnRightClick(target, terrainHit));
                }
            } else if (leftClickDown) {
                if (focusedSelected == null || !focusedSelected.OnLeftClickDown(target, terrainHit)) {
                    DefaultLeftClickDown();
                }
            } else if (leftClickUp) {
                if (focusedSelected == null || !focusedSelected.OnLeftClickUp(target, terrainHit)) {
                    DefaultLeftClickUp(target);
                }
            }
        }

        private void HandleHotkeys() {
            if (Input.GetKey(Hotkeys.CenterCamera) && focusedSelected != null) {
                cameraController.FocusOn(focusedSelected.transform.gameObject);
            }
            if (Input.GetKeyDown(Hotkeys.NextSelection) && selected.Count > 1) {
                var oldIndex = selected.IndexOf(focusedSelected);
                var newIndex = oldIndex + 1;
                if (focusedSelected != null) focusedSelected.OnSelect();
                focusedSelected = selected[newIndex == selected.Count ? 0 : newIndex];
                focusedSelected.OnFocusSelect();
            }
            if (Input.GetKeyDown(KeyCode.Keypad0)) {
                selected.ForEach(controllable => {
                    var health = controllable.transform.gameObject.GetComponent<Damageable>();
                    if (health != null) health.Revive(0, 2);
                });
            }
        }

        private void DefaultLeftClickDown() {
            isSelecting = true;
            selectionStart = Input.mousePosition;
        }

        /// <summary>
        /// This is where all the selecting happens
        /// </summary>
        /// <param name="hit"></param>
        private void DefaultLeftClickUp(GameObject hit) {
            var newSelect = hit.GetComponent<MouseControllable>();

            if (selected.Contains(newSelect) && cameraController != null) {
                cameraController.FocusOn(hit);
            }

            DeselectCurrentSelection();

            // Find objects within selection
            var objects = FindObjectsOfType<MouseControllable>();
            foreach (var o in objects) {
                if (!IsWithinSelectionBounds(o.gameObject)) continue;
                o.OnSelect();
                selected.Add(o);
            }

            // Add click target
            if (newSelect != null) {
                selected.Add(newSelect);
                newSelect.OnSelect();
                focusedSelected = newSelect;
            } else if (selected.Count > 0) {
                focusedSelected = selected[0];
            }

            if (focusedSelected != null) focusedSelected.OnFocusSelect();

            isSelecting = false;
        }

        /// <summary>
        /// Clear selection and focused selection
        /// </summary>
        private void DeselectCurrentSelection() {
            selected.ForEach(o => o.OnDeselect());
            selected.Clear();
            focusedSelected = null;
        }

        /// <summary>
        /// Casts a ray from the main camera through the cursor and outputs the first clickable object hit aswell as
        /// the first terrain hit
        /// </summary>
        /// <returns>true if anything has been hit</returns>
        private bool GetClickLocation(out GameObject target, out Vector3 terrainHit) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            RaycastHit terrainRaycastHit;
            Physics.Raycast(ray, out terrainRaycastHit, 100, TerrainLayerMask);
            if (!Physics.Raycast(ray, out hit, 100, ClickableLayers)) {
                target = null;
                terrainHit = new Vector3();
                return false;
            }
            target = hit.transform.gameObject;
            terrainHit = terrainRaycastHit.point;
            return true;
        }

        /// <summary>
        /// Checks if the object is within the selection
        /// </summary>
        private bool IsWithinSelectionBounds(GameObject theObject) {
            if (!isSelecting) return false;
            var mainCamera = Camera.main;
            var viewportBounds = ScreenUtil.GetViewportBounds(mainCamera, selectionStart, Input.mousePosition);
            return viewportBounds.Contains(mainCamera.WorldToViewportPoint(theObject.transform.position));
        }

        /// <summary>
        /// Draw the selection rectangle
        /// </summary>
        private void OnGUI() {
            if (!isSelecting) return;
            var rect = ScreenUtil.GetScreenRect(selectionStart, Input.mousePosition);
            DrawUtil.DrawScreenBorderedRect(rect, 2, SelectionBorderColor, SelectionFillColor);
        }
    }
}