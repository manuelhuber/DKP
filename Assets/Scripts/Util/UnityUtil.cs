using System;
using System.Linq;
using UnityEngine;

namespace Util {
    public static class UnityUtil {
        public static T FindComponentInChildrenWithTag<T>(GameObject parent, String tag) where T : Component {
            var all = parent.GetComponentsInChildren<T>().ToList().Where(o => o.CompareTag(tag)).ToList();
            return all.Count == 0 ? null : all.First();
        }
    }
}