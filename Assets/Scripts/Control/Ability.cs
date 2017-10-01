using System;
using Abilities.Indicators.Scripts.Components;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

namespace Control {
    /// <summary>
    /// A hotkey setup
    /// </summary>
    [Serializable]
    public class Hotkey {
        public KeyCode HotkKeyCode;

        /// <summary>
        /// If the modifier button needs to be pressed to active this 
        /// </summary>
        public bool Modifier = false;

        public override string ToString() {
            return (Modifier ? "s" : "") + HotkKeyCode;
        }
    }

    /// <summary>
    /// A wrapper for an ability that holds some individual states (since the Ability is a ScriptableObject)
    /// </summary>
    public class ActiveAbility {
        public Ability Ability;
        public float RemainingCooldown;
        public Hotkey Hotkey;
    }

    // TODO: the lifecycle model is not great - there is some hardcoded stuff in the abilty handler and some convetion
    // that have to be upheld (like an ability can't only use the leftClickDown without consuming the clickUp aswell)
    public abstract class Ability : ScriptableObject {
        [Header("Infos")] public Sprite Icon;
        public string Name;
        [TextArea] public string Tooltip;
        public abstract SpellTargetingType IndicatorType { get; }
        [Header("Targeting")] public SpellTargeting SpellTargeting;
        [Header("Ability")] public float Cooldown;

        /// <summary>
        /// Easy access to range
        /// </summary>
        public float Range {
            get { return SpellTargeting.Range; }
        }

        private string TargetName {
            get { return name + "Target"; }
        }

        private string RangeName {
            get { return name + "Range"; }
        }

        /// <summary>
        /// Gets called when the player presses the associated hotkey
        /// </summary>
        /// <param name="caster">
        /// The game object of the caster
        /// </param>
        /// <returns>
        /// true if this ability is done after this function call 
        /// </returns>
        public virtual bool OnActivation(GameObject caster) {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="click">
        /// The click that caused the event
        /// </param>
        /// <returns>
        /// true if this ability is done 
        /// </returns>
        public virtual bool OnLeftClickDown(ClickLocation click, GameObject caster) {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="click">
        /// The click that caused the event
        /// </param>
        /// <returns>
        /// true if this ability is done 
        /// </returns>
        public virtual bool OnLeftClickUp(ClickLocation click, GameObject caster) {
            return true;
        }

        public virtual void OnCancel(GameObject caster) {
        }

        public virtual void OnUpdate(GameObject caster) {
        }

        protected void ActivatePointTarget(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            DoActivateTargetIndicator(splat);
        }

        protected void ActivateRange(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            DoActivateRange(splat);
        }

        protected void CancelTargeting(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            DoCancelTargeting(splat, caster);
        }

        protected void ActivateRangeIndicator(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            DoActivateRange(splat);
        }

        private void DoActivateTargetIndicator(SplatManager splat) {
            var point = GetTargetIndicator(splat);
            point.RangeIndicator = DoGetRangeIndicator(splat);
            splat.RefreshIndicators();
            splat.SelectSpellIndicator(TargetName);
        }

        private void DoActivateRange(SplatManager splat) {
            DoGetRangeIndicator(splat);
            splat.RefreshIndicators();
            splat.SelectRangeIndicator(RangeName);
        }

        private void DoCancelTargeting(SplatManager splat, GameObject caster) {
            splat.CancelSpellIndicator();
            splat.CancelRangeIndicator();
            splat.CancelStatusIndicator();
        }

        /// <summary>
        /// Returns the existing target indicator or creates one - the splat manager is not updated!
        /// </summary>
        private SpellIndicator GetTargetIndicator(SplatManager splat) {
            var spellIndicator = splat.GetSpellIndicator(TargetName);
            if (spellIndicator != null) return spellIndicator;

            // Create new one
            var marker = Instantiate(SpellTargeting.TargetPrefab, splat.gameObject.transform);
            marker.name = TargetName;
            spellIndicator = marker.GetComponent<SpellIndicator>();
            spellIndicator.Range = SpellTargeting.Range;
            spellIndicator.Scale = SpellTargeting.TargetSize;
            return spellIndicator;
        }

        /// <summary>
        /// Returns the existing range indicator or creates one - the splat manager is not updated!
        /// </summary>
        private RangeIndicator DoGetRangeIndicator(SplatManager splat) {
            var range = splat.GetRangeIndicator(RangeName);
            if (range != null) return range;

            // Create new one
            var rangeObject = Instantiate(SpellTargeting.RangePrefab, splat.gameObject.transform);
            rangeObject.name = RangeName;
            range = rangeObject.GetComponent<RangeIndicator>();
            range.Scale = SpellTargeting.Range;
            range.DefaultScale = SpellTargeting.Range;
            return range;
        }
    }
}