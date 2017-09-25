using System;
using UnityEngine;

namespace Control {
    public class ClickLocation {
        /// The game object hit by the click
        public GameObject Target;

        /// The location of the hit on the terrain (ignoring hits with units etc)
        public Vector3 Location;
    }

    /// <summary>
    /// A GameObject that can be selected and receive commands via mouse clicks
    /// </summary>
    public abstract class MouseControllable : MonoBehaviour {
        // TODO: improve select/focus lifecycle. what if a unit is selected and then becomes focused? or the other way around?
        // maybe "onSelect", "onDeselect", "onGainFocus", "onLoseFocus"

        public abstract void OnSelect();
        public abstract void OnFocusSelect();
        public abstract void OnDeselect();

        /// <summary>
        /// A left click up happens while the unit is actively selected (actively selected = being the single selected
        /// object or purposefully focused in a group of selections)
        /// </summary>
        /// <returns>
        /// true if this action "consumes the click" - meaning no additional action should be cause by this click.
        /// By default a left click up deselects the current selection and either selects the clicked unit or selects 
        /// all drag-selected units.
        /// Example:
        /// Usually a mouse controllable doesn't want anything special to happen on a left click, so it returns false
        /// and the default behavior will happen (and the unit will be deselected).
        /// But if it is in a "spell casting" mode a left click should cast a spell at the click location without 
        /// deselecting the current selection. This can be achieved by returning true to preventing the default 
        /// deselection behaviour. 
        /// </returns>
        public virtual bool OnLeftClickUp(ClickLocation click) {
            return false;
        }

        /// <summary>
        /// A left click up happens while the unit is selected (actively selected = being the single selected
        /// object or purposefully focused in a group of selections)
        /// </summary>
        /// <returns>
        /// true if the default click behaviour should be ignored.
        /// The default is to start a drag-selection
        /// </returns>
        public virtual bool OnLeftClickDown(ClickLocation click) {
            return false;
        }

        /// <summary>
        /// A right click happens while the unit is selected
        /// </summary>
        /// <returns>
        /// true if the default click behaviour should be ignored. For example see "OnLeftClick"
        /// Note: Currently there is no default right-click behaviour, so it doesn't matter
        /// </returns>
        public virtual bool OnRightClick(ClickLocation click) {
            return false;
        }


        /// <summary>
        /// A shift-right click happens while the unit is selected
        /// </summary>
        /// <returns>
        /// true if the default click behaviour should be ignored. For example see "OnLeftClick"
        /// Note: Currently there is no default right-click behaviour, so it doesn't matter
        /// </returns>
        public virtual bool OnRightShiftClick(ClickLocation click) {
            return false;
        }

        public abstract void OnButton(String buttonName);
    }
}