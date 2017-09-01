using UnityEngine;

public class MouseControl : MonoBehaviour {

    private MouseControllable selected;

    private void Update() {
        var rightClick = Input.GetMouseButtonDown(1);
        var leftClick = Input.GetMouseButtonDown(0);
        if (!rightClick && !leftClick) return;

        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 100)) return;

        if (rightClick) {
            if (selected == null) return;
            if (Input.GetKey(KeyCode.LeftShift)) {
                selected.OnRightShiftClick(hit.point);
            } else {
                selected.OnRightClick(hit.point);
            }
        } else {
            selected = hit.transform.gameObject.GetComponent<MouseControllable>();
        }
    }
}