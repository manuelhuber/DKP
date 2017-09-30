using System;
using UnityEngine;
using Util;

namespace Generic {
    public abstract class DkpMonoBehaviour : MonoBehaviour {
        public void DoAfterDelay(Action action, float delay) {
            StartCoroutine(UnityUtil.DoAfterDelay(action, delay));
        }
    }
}