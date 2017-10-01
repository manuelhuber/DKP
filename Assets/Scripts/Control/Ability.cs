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
        [Header("Hotkey")] public Sprite Icon;
        [Space] [Header("Ability")] public string Name;
        public float Cooldown;
        public string Tooltip;
        public abstract SpellTargetingType IndicatorType { get; }

        public SpellTargeting SpellTargeting;

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

        protected void PointTarget(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            var pointName = name + "Point";
            if (splat.SelectSpellIndicator(pointName) != null) return;
            var marker = Instantiate(SpellTargeting.TargetPrefab, splat.gameObject.transform);
            marker.name = pointName;
            var point = marker.GetComponent<Point>();
            point.Range = SpellTargeting.Range;
            point.RangeIndicator = GetRangeIndicator(caster);
            point.Scale = SpellTargeting.TargetSize;
            splat.RefreshIndicators();
            splat.SelectSpellIndicator(pointName);
        }

        protected void CancelTargeting(GameObject prefab, GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            splat.CancelSpellIndicator();
            splat.CancelRangeIndicator();
            splat.CancelStatusIndicator();
        }

        protected RangeIndicator GetRangeIndicator(GameObject caster) {
            var splat = caster.GetComponentInChildren<SplatManager>();
            var rangeName = name + "Range";
            var range = splat.GetRangeIndicator(rangeName);
            if (range != null) return range;
            var rangeObject = Instantiate(SpellTargeting.RangePrefab, splat.gameObject.transform);
            rangeObject.name = rangeName;
            range = rangeObject.GetComponent<RangeIndicator>();
            range.Scale = SpellTargeting.Range;
            return range;
        }
    }
}