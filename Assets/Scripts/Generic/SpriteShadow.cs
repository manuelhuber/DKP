using UnityEngine;
using UnityEngine.Rendering;

public class SpriteShadow : MonoBehaviour {
    private void Awake() {
        if (GetComponent<Renderer>()) GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.TwoSided;
        foreach (Renderer renderer in GetComponentsInChildren(typeof(Renderer))) {
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;
        }
    }
}