﻿using System.Linq;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Abilities.Indicators.Scripts.Components {
    /// <summary>
    /// Apply this to the GameObject which holds all your Splats. Make sure the origin is correctly centered at the base of the Character.
    /// </summary>
    public class SplatManager : MonoBehaviour {
        /// <summary>
        /// Determines whether the cursor should be hidden when a Splat is showing.
        /// </summary>
        public bool HideCursor;

        /// <summary>
        /// Returns all Spell Indicators belonging to the Manager.
        /// </summary>
        public SpellIndicator[] SpellIndicators { get; set; }

        /// <summary>
        /// Returns all Status Indicators belonging to the Manager.
        /// </summary>
        public StatusIndicator[] StatusIndicators { get; set; }

        /// <summary>
        /// Returns all Range Indicators belonging to the Manager.
        /// </summary>
        public RangeIndicator[] RangeIndicators { get; set; }

        /// <summary>
        /// Returns the currently selected Spell Indicator.
        /// </summary>
        public SpellIndicator CurrentSpellIndicator { get; private set; }

        /// <summary>
        /// Returns the currently selected Status Indicator.
        /// </summary>
        public StatusIndicator CurrentStatusIndicator { get; private set; }

        /// <summary>
        /// Returns the currently selected Range Indicator.
        /// </summary>
        public RangeIndicator CurrentRangeIndicator { get; private set; }

        void OnEnable() {
            RefreshIndicators();
        }

        public void RefreshIndicators() {
// Create a list of all the splats available to the manager
            SpellIndicators = GetComponentsInChildren<SpellIndicator>(true);
            StatusIndicators = GetComponentsInChildren<StatusIndicator>(true);
            RangeIndicators = GetComponentsInChildren<RangeIndicator>(true);

            // Make sure each Splat has a reference to its Manager
            SpellIndicators.ToList().ForEach(x => x.Manager = this);
            StatusIndicators.ToList().ForEach(x => x.Manager = this);
            RangeIndicators.ToList().ForEach(x => x.Manager = this);

            // Initialize the Splats
            SpellIndicators.ToList().ForEach(x => x.Initialize());
            StatusIndicators.ToList().ForEach(x => x.Initialize());
            RangeIndicators.ToList().ForEach(x => x.Initialize());

            // Make all Splats invisible to start with
            SpellIndicators.ToList().ForEach(x => x.gameObject.SetActive(false));
            StatusIndicators.ToList().ForEach(x => x.gameObject.SetActive(false));
            RangeIndicators.ToList().ForEach(x => x.gameObject.SetActive(false));
        }

        // This Update method and the "HideCursor" variable can be deleted if you do not need this functionality
        private void Update() {
            if (HideCursor) {
                Cursor.visible = CurrentSpellIndicator == null;
            }
        }

        /// <summary>
        /// Position of Current Spell Indicator or Mouse Ray if unavailable
        /// </summary>
        public Vector3 GetSpellCursorPosition() {
            return CurrentSpellIndicator != null
                ? CurrentSpellIndicator.transform.position
                : Splat.Get3DMousePosition();
        }

        /// <summary>
        /// Select and make visible the Spell Indicator given by name.
        /// </summary>
        public SpellIndicator SelectSpellIndicator(string splatName) {
            CancelSpellIndicator();
            var indicator = GetSpellIndicator(splatName);
            if (indicator == null) return null;

            if (indicator.RangeIndicator != null) {
                indicator.RangeIndicator.gameObject.SetActive(true);
                indicator.RangeIndicator.OnShow();
            }

            indicator.gameObject.SetActive(true);
            indicator.OnShow();
            CurrentSpellIndicator = indicator;
            return CurrentSpellIndicator;
        }

        /// <summary>
        /// Select and make visible the Status Indicator given by name.
        /// </summary>
        public StatusIndicator SelectStatusIndicator(string splatName) {
            CancelStatusIndicator();
            var indicator = GetStatusIndicator(splatName);
            if (indicator == null) return null;
            indicator.gameObject.SetActive(true);
            indicator.OnShow();
            CurrentStatusIndicator = indicator;
            return CurrentStatusIndicator;
        }

        /// <summary>
        /// Select and make visible the Range Indicator given by name.
        /// </summary>
        public RangeIndicator SelectRangeIndicator(string splatName) {
            CancelRangeIndicator();
            var indicator = GetRangeIndicator(splatName);
            if (indicator == null) return null;

            // If current spell indicator uses same Range indicator then cancel it.
            if (CurrentSpellIndicator != null && CurrentSpellIndicator.RangeIndicator == indicator) {
                CancelSpellIndicator();
            }

            indicator.gameObject.SetActive(true);
            indicator.OnShow();
            CurrentRangeIndicator = indicator;
            return CurrentRangeIndicator;
        }

        /// <summary>
        /// Return the Spell Indicator given by name.
        /// </summary>
        public SpellIndicator GetSpellIndicator(string splatName) {
            return SpellIndicators.FirstOrDefault(x => x.name == splatName);
        }

        /// <summary>
        /// Return the Status Indicator given by name.
        /// </summary>
        public StatusIndicator GetStatusIndicator(string splatName) {
            return StatusIndicators.FirstOrDefault(x => x.name == splatName);
        }

        /// <summary>
        /// Return the Range Indicator given by name.
        /// </summary>
        public RangeIndicator GetRangeIndicator(string splatName) {
            return RangeIndicators.FirstOrDefault(x => x.name == splatName);
        }

        /// <summary>
        /// Hide Spell Indicator
        /// </summary>
        public void CancelSpellIndicator() {
            if (CurrentSpellIndicator != null) {
                if (CurrentSpellIndicator.RangeIndicator != null) {
                    CurrentSpellIndicator.RangeIndicator.gameObject.SetActive(false);
                }

                CurrentSpellIndicator.OnHide();
                CurrentSpellIndicator.gameObject.SetActive(false);
                CurrentSpellIndicator = null;
            }
        }

        /// <summary>
        /// Hide Status Indicator
        /// </summary>
        public void CancelStatusIndicator() {
            if (CurrentStatusIndicator != null) {
                CurrentStatusIndicator.OnHide();
                CurrentStatusIndicator.gameObject.SetActive(false);
                CurrentStatusIndicator = null;
            }
        }

        /// <summary>
        /// Hide Range Indicator
        /// </summary>
        public void CancelRangeIndicator() {
            if (CurrentRangeIndicator != null) {
                CurrentRangeIndicator.OnHide();
                CurrentRangeIndicator.gameObject.SetActive(false);
                CurrentRangeIndicator = null;
            }
        }
    }
}