using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist.Extensions.AABB {

    public static class AABBExtension {
        #region Extensions
        public static Bounds Encapsulate(this IEnumerable<Bounds> bounds) {
            var resmin = Min ();
            var resmax = Max ();
            foreach (var bb in bounds) {
                var bbmin = bb.min;
                var bbmax = bb.max;
                for (var i = 0; i < 3; i++) {
                    resmin [i] = Mathf.Min (resmin [i], bbmin [i]);
                    resmax [i] = Mathf.Max (resmax [i], bbmax [i]);
                }
            }
            return MinMaxBounds (resmin, resmax);
        }
        public static Bounds Encapsulate(this IEnumerable<Vector3> poss) {
            var resmin = Min ();
            var resmax = Max ();
            foreach (var p in poss) {
                for (var i = 0; i < 3; i++) {
                    resmin [i] = Mathf.Min (resmin [i], p[i]);
                    resmax [i] = Mathf.Max (resmax [i], p[i]);
                }
            }
            return MinMaxBounds (resmin, resmax);
        }
        #endregion

        public static Bounds MinMaxBounds(Vector3 min, Vector3 max) {
            var bb = new Bounds();
            bb.SetMinMax(min, max);
            return bb;
        }
        public static Vector3 Min() {
            return new Vector3 (float.MaxValue, float.MaxValue, float.MaxValue);
        }
        public static Vector3 Max() {
            return new Vector3 (float.MinValue, float.MinValue, float.MinValue);
        }
    }
}