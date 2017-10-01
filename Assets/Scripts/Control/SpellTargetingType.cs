using System;
using UnityEngine;

namespace Control {
    [Serializable]
    public enum SpellTargetingType {
        None,
        Self,
        Point,
        TwoPoints,
        Line
    }

    [Serializable]
    // !!! CAREFULL WHEN REFACTORING! The DkpRangeIndicatorEditor uses the property names in string literals !!! 
    public class SpellTargeting {
        public GameObject TargetPrefab;
        public float TargetSize;

        public GameObject RangePrefab;
        public float Range;
    }
}