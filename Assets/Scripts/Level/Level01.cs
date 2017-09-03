using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level01 : MonoBehaviour {
    public GameObject FirePrefab;
    public float FireInterval;

    private float NextFire;

    private void Awake() {
        NextFire = FireInterval;
    }

    private void Update() {
        if (NextFire < Time.time) {
            NextFire += FireInterval;
        }
    }
}