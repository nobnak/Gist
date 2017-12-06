using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Extensions.ComponentExt {

    public static class ComponentExtension {
        public static IEnumerable<T> AggregateComponentsInChildren<T>(this Transform parent)
            where T : Component {

            if (parent == null)
                yield break;

            for (var i = 0; i < parent.childCount; i++) {
                var child = parent.GetChild(i);
                var comp = child.GetComponent<T>();
                if (comp != null)
                    yield return comp;
                else
                    foreach (var c in child.AggregateComponentsInChildren<T>())
                        yield return c;
            }
        }
    }

}