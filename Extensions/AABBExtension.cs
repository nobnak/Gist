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
        public static Bounds EncapsulateInWorldSpace(this Transform tr, Bounds local) {
            var local2world = tr.localToWorldMatrix;
            return local.EncapsulateInTargetSpace (local2world);
        }
        public static Bounds EncapsulateInTargetSpace(this Bounds local, Matrix4x4 localToTargetMat) {
            var absMat = localToTargetMat.Absolute ();
            var center = localToTargetMat.MultiplyPoint3x4 (local.center);
            var extent = absMat.MultiplyVector (local.extents);
            return new Bounds (center, 2f * extent);
        }
        public static Bounds EncapsulateVertices(this IEnumerable<Vector3> vertices) { 
            var minx = float.MaxValue;
            var miny = float.MaxValue;
            var minz = float.MaxValue;
            var maxx = float.MinValue;
            var maxy = float.MinValue;
            var maxz = float.MinValue;

            foreach (var v in vertices) {
                if (v.x < minx)
                    minx = v.x;
                if (maxx < v.x)
                    maxx = v.x;

                if (v.y < miny)
                    miny = v.y;
                if (maxy < v.y)
                    maxy = v.y;

                if (v.z < minz)
                    minz = v.z;
                if (maxz < v.z)
                    maxz = v.z;
            }

            var size = new Vector3 (maxx - minx, maxy - miny, maxz - minz);
            var center = new Vector3 (minx + 0.5f * size.x, miny + 0.5f * size.y, minz + 0.5f * size.z);
            return new Bounds (center, size);
        }

        public static Matrix4x4 Absolute(this Matrix4x4 mat) {
            var absM = Matrix4x4.zero;
            for (var i = 0; i < 16; i++) {
                var j = mat [i];
                absM [i] = (j < 0 ? -j : j);
            }
            return absM;
        }
        #endregion

        #region Utils
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
        #endregion
    }
}