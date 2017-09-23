using System;
using System.Linq;
using UnityEngine;

namespace Util {
    public static class UnityUtil {
        /// <summary>
        /// Returns the component only from children with a specific tag. If multiple children are found the first one is chosen
        /// </summary>
        public static T FindComponentInChildrenWithTag<T>(GameObject parent, string tag) where T : Component {
            var all = parent.GetComponentsInChildren<T>().ToList().Where(o => o.CompareTag(tag)).ToList();
            return all.Count == 0 ? null : all.First();
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
    }
}