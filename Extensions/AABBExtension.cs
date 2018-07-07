using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.Primitive;

namespace nobnak.Gist.Extensions.AABB {

    public static class AABBExtension {

		#region Extensions
		public static Bounds Encapsulate(this IEnumerable<Bounds> bounds) {
			var resmin = Min();
			var resmax = Max();
			foreach (var bb in bounds) {
				var bbmin = bb.min;
				var bbmax = bb.max;
				for (var i = 0; i < 3; i++) {
					resmin[i] = Mathf.Min(resmin[i], bbmin[i]);
					resmax[i] = Mathf.Max(resmax[i], bbmax[i]);
				}
			}
			return MinMaxBounds(resmin, resmax);
		}
		public static FastBounds Encapsulate(this IEnumerable<FastBounds> bounds) {
			var resmin_x = float.MaxValue;
			var resmin_y = float.MaxValue;
			var resmin_z = float.MaxValue;

			var resmax_x = float.MinValue;
			var resmax_y = float.MinValue;
			var resmax_z = float.MinValue;

			foreach (var bb in bounds) {
				resmin_x = Mathf.Min(resmin_x, bb.min_x);
				resmin_y = Mathf.Min(resmin_y, bb.min_y);
				resmin_z = Mathf.Min(resmin_z, bb.min_z);

				resmax_x = Mathf.Max(resmax_x, bb.max_x);
				resmax_y = Mathf.Max(resmax_y, bb.max_y);
				resmax_z = Mathf.Max(resmax_z, bb.max_z);
			}
			return new FastBounds(
				resmin_x, resmin_y, resmin_z,
				resmax_x, resmax_y, resmax_z);
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
        public static FastBounds EncapsulateInWorldSpace(this Transform tr, FastBounds local) {
            var local2world = tr.localToWorldMatrix;
            return local.EncapsulateInTargetSpace (local2world);
        }
        public static FastBounds EncapsulateInTargetSpace(this FastBounds local, Matrix4x4 localToTargetMat) {
            var absMat = localToTargetMat.Absolute ();
            var center = localToTargetMat.MultiplyPoint3x4 (local.Center);
            var extent = absMat.MultiplyVector (local.Extents);
            return new Bounds (center, 2f * extent);
        }
        public static Rect EncapsulateInTargetSpace(this Rect local, Matrix4x4 localToTargetMat) {
            var absMat = localToTargetMat.Absolute();
            var center = localToTargetMat.MultiplyPoint3x4(local.center);
            var size = absMat.MultiplyVector(local.size);
            var min = center - 0.5f * size;
            return new Rect(min, size);
        }
        public static FastBounds EncapsulateVertices(this IEnumerable<Vector3> vertices) { 
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

			return new FastBounds(minx, miny, minz, maxx, maxy, maxz);
        }

        public static Matrix4x4 Absolute(this Matrix4x4 mat) {
            var absM = Matrix4x4.zero;
            for (var i = 0; i < 16; i++) {
                var j = mat [i];
                absM [i] = (j < 0 ? -j : j);
            }
            return absM;
        }
        public static Vector3 SampleIn(this Bounds bounds) {
            var min = bounds.min;
            var size = bounds.size;
            return new Vector3(min.x + size.x * Random.value, min.y + size.y * Random.value, min.z + size.z * Random.value);
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