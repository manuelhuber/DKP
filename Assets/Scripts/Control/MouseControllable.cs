using UnityEngine;

namespace Control {
    /// <summary>
    /// A GameObject that can be selected and receive commands via mouse clicks
    /// </summary>
    public abstract class MouseControllable : MonoBehaviour {
        public abstract void OnSelect();
        public abstract void OnFocusSelect();
        public abstract void OnDeselect();

        /// <summary>
        /// A left click up happens while the unit is actively selected (actively selected = being the single selected
        /// object or purposefully focused in a group of selections)
        /// </summary>
        /// <param name="target">the exact position of the click
        /// The game object hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">the position of the click, ignoring non-terrain
        /// The location of the hit on the terrain (ignoring hits with units etc)
        /// </param>
        /// <returns>
        /// true if the default click behaviour should be ignored. By default a left click up deselects the current
        /// selection and either selects the clicked unit or selects all drag-selected units.
        /// Example:
        /// Usually a mouse controllable doesn't want anything special to happen on a left click, so it returns false
        /// and the unit will be deselected.
        /// But if it is in a "spell casting" mode a left click should cast a spell at the click location without 
        /// deselecting the current selection. This can be achieved by returning true to preventing the default 
        /// deselection. 
        /// </returns>
        public virtual bool OnLeftClickUp(GameObject target, Vector3 positionOnTerrain) {
            return false;
        }

        /// <summary>
        /// A left click up happens while the unit is selected (actively selected = being the single selected
        /// object or purposefully focused in a group of selections)
        /// </summary>
        /// <param name="target">the exact position of the click
        /// The game object hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">the position of the click, ignoring non-terrain
        /// The location of the hit on the terrain (ignoring hits with units etc)
        /// </param>
        /// <returns>
        /// true if the default click behaviour should be ignored.
        /// The default is to start a drag-selection
        /// </returns>
        public virtual bool OnLeftClickDown(GameObject target, Vector3 positionOnTerrain) {
            return false;
        }

        /// <summary>
        /// A right click happens while the unit is selected
        /// </summary>
        /// <param name="target">
        /// The game object hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">
        /// The location of the hit on the terrain (ignoring hits with units etc)
        /// </param>
        /// <returns>
        /// true if the default click behaviour should be ignored. For example see "OnLeftClick"
        /// Note: Currently there is no default right-click behaviour, so it doesn't matter
        /// </returns>
        public virtual bool OnRightClick(GameObject target, Vector3 positionOnTerrain) {
            return false;
        }


        /// <summary>
        /// A shift-right click happens while the unit is selected
        /// </summary>
        /// <param name="target">
        /// The game object hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">
        /// The location of the hit on the terrain (ignoring hits with units etc)
        /// </param>
        /// <returns>
        /// true if the default click behaviour should be ignored. For example see "OnLeftClick"
        /// Note: Currently there is no default right-click behaviour, so it doesn't matter
        /// </returns>
        public virtual bool OnRightShiftClick(GameObject target, Vector3 positionOnTerrain) {
            return false;
        }
    }
}