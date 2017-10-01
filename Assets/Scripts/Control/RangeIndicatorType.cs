using System;
using UnityEngine;

namespace Control {
    [Serializable]
    public enum RangeIndicatorType {
        Self,
        Point,
        TwoPoints,
        Line
    }

    [Serializable]
    public class DkpRangeIndicator {
        public GameObject IndicatorPrefab;
        public float Range;
    }
}