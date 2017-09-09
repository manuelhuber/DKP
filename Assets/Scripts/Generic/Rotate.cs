using UnityEngine;

public class Rotate : MonoBehaviour {
    public int Speed = 10;

    void Update() {
        transform.localRotation = Quaternion.Euler(Speed * Time.time, Speed * Time.time, Speed * Time.time);
    }
}