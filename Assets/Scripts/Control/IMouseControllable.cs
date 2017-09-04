using UnityEngine;

namespace Control {
    public interface IMouseControllable {
        void OnSelect();
        void OnDeselect();
        void OnLeftClick();

        /// <summary>
        /// When the unit receives a right click command
        /// </summary>
        /// <param name="target">
        /// The game object that was hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">
        /// The location of the first hit on the terrain (ignoring hits with units etc)
        /// </param>
        void OnRightClick(GameObject target, Vector3 positionOnTerrain);


        /// <summary>
        /// When the unit receives a shift-right click command
        /// </summary>
        /// <param name="target">
        /// The game object that was hit by the click 
        /// </param>
        /// <param name="positionOnTerrain">
        /// The location of the first hit on the terrain (ignoring hits with units etc)
        /// </param>
        void OnRightShiftClick(GameObject target, Vector3 positionOnTerrain);
    }
}