using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public static class TransformExtension {
        public static Transform FindInChild(this Transform root, string name) {
            if (root.name == name)
                return root;

            for (var i = 0; i < root.childCount; i++) {
                var found = FindInChild (root.GetChild (i), name);
                if (found != null)
                    return found;
            }
            return null;
        }

        public static Matrix4x4 LocalToParent(this Transform tr) {
            return Matrix4x4.TRS(tr.localPosition, tr.localRotation, tr.localScale);
        }
    }
}