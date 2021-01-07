using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Extensions.ComponentExt {

    public static class ComponentExtension {
        public static IEnumerable<T> Children<T>(this Transform root, bool ignoreGrandchildren = true) {

            if (root == null)
                yield break;

            var found = false;
            foreach (var comp in root.GetComponents<T>()) {
                found = true;
                yield return comp;
            }
            if (found && ignoreGrandchildren)
                yield break;

            for (var i = 0; i < root.childCount; i++) {
                var child = root.GetChild(i);
                foreach (var c in child.Children<T>(ignoreGrandchildren))
                    yield return c;
            }
        }
		public static IEnumerable<T> Children<T>(this Component root, bool ignoreGrandchildren = true) {
			if (root == null)
				yield break;

			foreach (var c in Children<T>(root.transform, ignoreGrandchildren))
				yield return c;
		}
        public static IEnumerable<T> Children<T>(this GameObject root, bool ignoreGrandchildren = true) {
            return root.transform.Children<T>(ignoreGrandchildren);
        }
		public static IEnumerable<T> Parent<T>(this Transform root, bool ignoreGrandparent = true) {
			if (root == null)
				yield break;

			var parent = root.parent;
			if (parent == null)
				yield break;

			var found = false;
			foreach (var c in parent.GetComponents<T>()) {
				found = true;
				yield return c;
			}

			if (!found || !ignoreGrandparent)
				foreach (var c in parent.Parent<T>())
					yield return c;
		}
		public static IEnumerable<T> Parent<T>(this Component root, bool ignoreGrandparent = true) {
			if (root == null)
				yield break;
			foreach (var c in root.transform.Parent<T>())
				yield return c;
		}


        public static void CallbackSelf<Input>(this Component me, System.Action<Input> method) {
            foreach (var i in me.GetComponents<Input>())
                method(i);
        }
        public static IEnumerable<Output> CallbackSelf<Input, Output>(this Component me, System.Func<Input, Output> method) {
            foreach (var i in me.GetComponents<Input>())
                yield return method(i);
        }

        public static void CallbackChildren<Input>(this Component me, 
			System.Action<Input> method, bool ignoreGrandchildren = true) {

            foreach (var i in me.Children<Input>(ignoreGrandchildren))
                method(i);
        }
        public static IEnumerable<Output> CallbackChildren<Input, Output>(
            this Component me, System.Func<Input, Output> method) {
            foreach (var i in me.GetComponentsInChildren<Input>())
                yield return method(i);
		}

        public static void CallbackParent<Input>(
            this Component me, System.Action<Input> method, bool ignoreGrandparent = true) {
            foreach (var i in me.Parent<Input>(ignoreGrandparent))
                method(i);
        }
        public static IEnumerable<Output> CallbackParent<Input, Output>(
           this Component me, System.Func<Input, Output> method, bool ignoreGrandparent = true) {
            foreach (var i in me.Parent<Input>(ignoreGrandparent))
                yield return method(i);
        }

        public static IEnumerable<Input> FindInterface<Input>() {
            return Object.FindObjectsOfType<MonoBehaviour>().OfType<Input>();
        }

        public static bool IsVisibleLayer(this Component c) {
			return Camera.current != null && c != null
				&& (Camera.current.cullingMask & (1 << c.gameObject.layer)) != 0;
		}
	}
}