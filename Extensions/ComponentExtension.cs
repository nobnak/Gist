using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.ComponentExt {

    public static class ComponentExtension {
        public static IEnumerable<T> AggregateComponentsInChildren<T>(this Transform parent, bool blockTransmission) {

            if (parent == null)
                yield break;

            var found = false;
            foreach (var comp in parent.GetComponents<T>()) {
                found = true;
                yield return comp;
            }
            if (found && blockTransmission)
                yield break;

            for (var i = 0; i < parent.childCount; i++) {
                var child = parent.GetChild(i);
                foreach (var c in child.AggregateComponentsInChildren<T>(blockTransmission))
                    yield return c;
            }
        }
        public static IEnumerable<T> AggregateComponentsInChildren<T>(this Transform parent) {
            return parent.AggregateComponentsInChildren<T>(true);
        }
        public static bool IsVisibleLayer(this Component c) {
            return Camera.current != null && c != null 
                && (Camera.current.cullingMask & (1 << c.gameObject.layer)) != 0;
        }

        public static void NotifySelf<Input>(this Component me, System.Action<Input> method) {
            foreach (var i in me.GetComponents<Input>())
                method(i);
        }
        public static IEnumerable<Output> NotifySelf<Input, Output>(this Component me, System.Func<Input, Output> method) {
            foreach (var i in me.GetComponents<Input>())
                yield return method(i);
        }

        public static void NotifySelfAndChildren<Input>(this Component me, System.Action<Input> method) {
            foreach (var i in me.GetComponentsInChildren<Input>())
                method(i);
        }
        public static IEnumerable<Output> NotifySelfAndChildren<Input, Output>(this Component me, System.Func<Input, Output> method) {
            foreach (var i in me.GetComponentsInChildren<Input>())
                yield return method(i);
        }
    }
}