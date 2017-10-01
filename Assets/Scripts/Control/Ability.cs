using System;
using UnityEditor;
using UnityEngine;

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
        public abstract RangeIndicatorType IndicatorType { get; }
        
        public DkpRangeIndicator DkpRangeIndicator;

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
        public virtual bool OnLeftClickDown(ClickLocation click) {
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
        public virtual bool OnLeftClickUp(ClickLocation click) {
            return true;
        }

        public virtual void OnCancel() {
        }

        public virtual void OnUpdate() {
        }
    }
}