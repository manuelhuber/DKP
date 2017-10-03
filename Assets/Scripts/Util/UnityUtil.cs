using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Util {
    public static class UnityUtil {
        /// <summary>
        /// Returns the component only from children with a specific tag. If multiple children are found the first one is chosen
        /// </summary>
        public static T FindComponentInChildrenWithTag<T>(GameObject parent, string tag) where T : Component {
            return parent.GetComponentsInChildren<T>().ToList().FirstOrDefault(o => o.CompareTag(tag));
        }

        /// <summary>
        /// Sets the position on a transform object 
        /// </summary>
        public static Transform SetAnchoredPosition(RectTransform transform, float x, float y) {
            var pos = transform.anchoredPosition;
            pos.x = x;
            pos.y = y;
            transform.anchoredPosition = pos;
            return transform;
        }

        public static IEnumerator DoAfterDelay(Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}