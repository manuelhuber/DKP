using UnityEngine;
using UnityEngine.UI;

namespace Control {
    public abstract class Ability : MonoBehaviour {
        [Header("Hotkey")] public Sprite Icon;
        public KeyCode Hotkey;
        public KeyCode Modifier;
        [Space] [Header("Ability")] public string Name;
        public float Cooldown;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// true if this ability is done after this function call 
        /// </returns>
        public virtual bool OnActivation(GameObject caster) {
            return true;
        }

        public virtual bool OnLeftClickDown(ClickLocation click) {
            return false;
        }

        public virtual bool OnLeftClickUp(ClickLocation click) {
            return false;
        }
    }
}