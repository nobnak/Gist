using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.ComponentExt {

    public static class ComponentExtension {
        public static IEnumerable<T> AggregateComponentsInChildren<T>(this Transform parent) {

            if (parent == null)
                yield break;

            var found = false;
            foreach (var comp in parent.GetComponents<T>()) {
                found = true;
                yield return comp;
            }
            if (found)
                yield break;

            for (var i = 0; i < parent.childCount; i++) {
                var child = parent.GetChild(i);
                foreach (var c in child.AggregateComponentsInChildren<T>())
                    yield return c;
            }
        }
        public static bool IsVisibleLayer(this Component c) {
            return (Camera.current.cullingMask & (1 << c.gameObject.layer)) != 0;
        }
    }

}