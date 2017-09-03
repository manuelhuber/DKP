using UnityEngine;

public interface MouseControllable {
    void OnSelect();
    void OnDeselect();
    void OnLeftClick();

    /// <summary>
    /// When the unit is right clicked
    /// </summary>
    /// <param name="position">
    /// The location of the first hit (which is on the unit itself)
    /// </param>
    /// <param name="positionOnTerrain">
    /// The location of the first hit on the terrain (ignoring hits with units etc)
    /// </param>
    void OnRightClick(Vector3 position, Vector3 positionOnTerrain);


    /// <summary>
    /// When the unit is right clicked with shift held down
    /// </summary>
    /// <param name="position">
    /// The location of the first hit (which is on the unit itself)
    /// </param>
    /// <param name="positionOnTerrain">
    /// The location of the first hit on the terrain (ignoring hits with units etc)
    /// </param>
    void OnRightShiftClick(Vector3 position, Vector3 positionOnTerrain);
}