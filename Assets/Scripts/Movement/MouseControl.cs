using UnityEngine;

public class MouseControl : MonoBehaviour {
    public LayerMask TerrainLayerMask;

    private MouseControllable selected;

    private void Update() {
        var rightClick = Input.GetMouseButtonDown(1);
        var leftClick = Input.GetMouseButtonDown(0);
        if (!rightClick && !leftClick) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        RaycastHit terrainHit;
        Physics.Raycast(ray, out terrainHit, 100, TerrainLayerMask);
        if (!Physics.Raycast(ray, out hit, 100)) return;

        if (rightClick) {
            if (selected == null) return;
            if (Input.GetKey(KeyCode.LeftShift)) {
                selected.OnRightShiftClick(hit.point, terrainHit.point);
            } else {
                selected.OnRightClick(hit.point, terrainHit.point);
            }
        } else {
            var newSelect = hit.transform.gameObject.GetComponent<MouseControllable>();
            if (newSelect != selected && selected != null) selected.OnDeselect();
            selected = newSelect;
            if (selected != null) selected.OnSelect();
        }
    }
}