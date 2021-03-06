﻿using System;
using System.Collections.Generic;
using System.Linq;
using Damage;
using DKPCamera;
using DKPSettings;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;

namespace Control {
    /// <summary>
    /// The main methods of control
    /// Holds the "selected units" state
    /// </summary>
    public class MainControl : MonoBehaviour {
        public Color SelectionBorderColor;
        public Color SelectionFillColor;
        public string[] ButtonNames;

        [Tooltip("Maximum time in seconds between same hotkey strokes to count as a double tap")]
        public double DoubleTapTime = 1;


        /// <summary>
        /// All layers that should be clickable - either to be selected or be the target of clicks
        /// </summary>
        public LayerMask ClickableLayers;

        public readonly Dictionary<int, List<MouseControllable>> ControlGroups =
            new Dictionary<int, List<MouseControllable>>();

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

        /// <summary>
        /// Last controllgroup hotkey pressed
        /// </summary>
        private int doubleTapGroup;

        /// <summary>
        /// Time at which last controllgroup hotkey was pressed
        /// </summary>
        private float doubleTapTime;


        public bool GetClickLocation(out GameObject target, out ClickLocation click) {
            click = new ClickLocation();
            Vector3 terrainHit;
            if (!PositionUtil.GetCursorLocation(out target, out terrainHit, ClickableLayers)
                || EventSystem.current.IsPointerOverGameObject()) return false;

            click = new ClickLocation {Target = target, Location = terrainHit};
            return true;
        }

        private void HandleHotkeys(ClickLocation click) {
            // Remove selection
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (isSelecting) {
                    isSelecting = false;
                } else {
                    DeselectCurrentSelection();
                }
            }

            foreach (var buttonName in ButtonNames) {
                if (Input.GetButtonDown(buttonName)) {
                    selected.ForEach(controllable => controllable.OnButton(buttonName, click));
                }
            }

            // Center Camera
            if (Input.GetKey(Hotkeys.CenterCamera) && focusedSelected != null) {
                cameraController.FocusOn(focusedSelected.transform.gameObject);
            }

            // Tab selection
            if (Input.GetKeyDown(Hotkeys.NextSelection) && selected.Count > 1) {
                var oldIndex = selected.IndexOf(focusedSelected);
                var newIndex = oldIndex + 1;
                if (focusedSelected != null) focusedSelected.OnSelect();
                focusedSelected = selected[newIndex == selected.Count ? 0 : newIndex];
                focusedSelected.OnFocusSelect();
            }

            // Revive TODO: remove
            if (Input.GetKeyDown(KeyCode.Keypad0)) {
                selected.ForEach(controllable => {
                    var health = controllable.transform.gameObject.GetComponent<Damageable>();
                    if (health != null) health.Revive(0, 2);
                });
            }

            // Control Groups
            if (Input.GetButtonUp("Select All")) {
                DeselectCurrentSelection();
                AddToSelection(FindObjectsOfType<MouseControllable>().ToList(), true);
            }
            for (var i = 1; i <= 5; i++) {
                if (!Input.GetButtonUp("Select " + i)) continue;
                DeselectCurrentSelection();
                AddToSelection(ControlGroups[i], true);
                if (ValidDoubleTap(i)) {
                    cameraController.FocusOn(ControlGroups[i][0].gameObject);
                }
                doubleTapGroup = i;
                doubleTapTime = Time.time;
            }
        }

        private bool ValidDoubleTap(int i) {
            return doubleTapGroup == i && (doubleTapTime + DoubleTapTime) > Time.time;
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
            var clickTarget = hit.GetComponent<MouseControllable>();

            if (selected.Contains(clickTarget) && cameraController != null) {
                cameraController.FocusOn(hit);
            }

            DeselectCurrentSelection();

            // Add objects within selection
            FindObjectsOfType<MouseControllable>()
                .Where(unit => IsWithinSelectionBounds(unit.gameObject)).ToList()
                .ForEach(AddToSelection);

            // Add click target
            if (clickTarget != null) {
                AddToSelection(clickTarget);
                AddToSelection(focusedSelected);
                focusedSelected = clickTarget;
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
        /// Checks if the object is within the selection
        /// </summary>
        private bool IsWithinSelectionBounds(GameObject theObject) {
            if (!isSelecting) return false;
            var mainCamera = Camera.main;
            var viewportBounds = ScreenUtil.GetViewportBounds(mainCamera, selectionStart, Input.mousePosition);
            return viewportBounds.Contains(mainCamera.WorldToViewportPoint(theObject.transform.position));
        }

        private void AddToSelection(List<MouseControllable> controllables, bool focus) {
            controllables.ForEach(AddToSelection);
            if (!focus || selected.Count <= 0) return;
            focusedSelected = selected[0];
            focusedSelected.OnFocusSelect();
        }

        private void AddToSelection(MouseControllable controllable) {
            if (controllable == null) return;
            selected.Add(controllable);
            controllable.OnSelect();
        }

        private void Start() {
            cameraController = GetComponent<FocusOnObject>();
        }

        /// <summary>
        /// Draw the selection rectangle
        /// </summary>
        private void OnGUI() {
            if (!isSelecting) return;
            var rect = ScreenUtil.GetScreenRect(selectionStart, Input.mousePosition);
            DrawUtil.DrawScreenBorderedRect(rect, 2, SelectionBorderColor, SelectionFillColor);
        }

        private void Update() {
            GameObject target;
            ClickLocation click;
            GetClickLocation(out target, out click);
            HandleHotkeys(click);

            var rightClick = Input.GetMouseButtonUp(1);
            var leftClickDown = Input.GetMouseButtonDown(0);
            var leftClickUp = Input.GetMouseButtonUp(0);
            if (!rightClick && !leftClickDown && !leftClickUp) return;

            if (rightClick) {
                // Currently there is no default behaviour for right clicks so just call the handlers
                if (Input.GetKey(Hotkeys.AddModifier)) {
                    selected.ForEach(o => o.OnRightShiftClick(click));
                } else {
                    selected.ForEach(c => c.OnRightClick(click));
                }
            } else if (leftClickDown) {
                if (focusedSelected == null || !focusedSelected.OnLeftClickDown(click)) {
                    DefaultLeftClickDown();
                }
            } else if (leftClickUp) {
                if (focusedSelected == null || !focusedSelected.OnLeftClickUp(click)) {
                    DefaultLeftClickUp(target);
                } else isSelecting = false;
            }
        }
    }
}