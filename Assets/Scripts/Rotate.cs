using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public int Speed = 10;

    void Update() {
        transform.localRotation = Quaternion.Euler(45, Speed * Time.time, 45);
    }
}