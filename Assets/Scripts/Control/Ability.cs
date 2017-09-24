using System;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// true if this ability is done after this function call 
        /// </returns>
        public virtual bool OnActivation(GameObject c) {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="click"></param>
        /// <returns>
        /// true if this ability is done 
        /// </returns>
        public virtual bool OnLeftClickDown(ClickLocation click) {
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="click"></param>
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