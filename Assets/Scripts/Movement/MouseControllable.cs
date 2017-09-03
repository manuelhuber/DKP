using UnityEngine;

public interface MouseControllable {
    void OnSelect();
    void OnDeselect();
    void OnLeftClick();
    void OnRightClick(Vector3 position, Vector3 positionOnTerrain);
    void OnRightShiftClick(Vector3 position, Vector3 positionOnTerrain);
}