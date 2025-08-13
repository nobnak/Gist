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
		#endregion

		#region sample insidde
		public static Vector3 SampleIn(this FastBounds fb, float uv_x, float uv_y, float uv_z) {
            var min = fb.Min;
			var max = fb.Max;
            var size = fb.Size;
			return new Vector3(
				Mathf.Lerp(min.x, max.x, uv_x),
				Mathf.Lerp(min.y, max.y, uv_y),
				Mathf.Lerp(min.z, max.z, uv_z));
        }
		public static Vector2 SampleIn(this FastBounds2D fb, float uv_x, float uv_y) {
			var min = fb.Min;
			var max = fb.Max;
			var size = fb.Size;
			return new Vector2(
				Mathf.Lerp(min.x, max.x, uv_x),
				Mathf.Lerp(min.y, max.y, uv_y));
		}
		public static Vector3 SampleIn(this FastBounds fb) {
			return fb.SampleIn(Random.value, Random.value, Random.value);
		}
		public static Vector2 SampleIn(this FastBounds2D fb) {
			return fb.SampleIn(Random.value, Random.value);
		}
		#endregion

		#region closest point search
		public static Vector2 ClosestPoint(this FastBounds2D fb, Vector2 point) {

			var min = fb.Min;
			var max = fb.Max;
			var x0 = min.x;
			var y0 = min.y;
			var x1 = max.x;
			var y1 = max.y;

			var xp = point.x;
			var yp = point.y;

			if (yp < y0) {
				return new Vector2((xp < x0 ? x0 : (xp < x1 ? xp : x1)), y0);
			} else if (yp < y1) {
				if (xp < x0)
					return new Vector2(x0, yp);
				if (x1 < xp)
					return new Vector2(x1, yp);

				var minDist = xp - x0;
				var result = new Vector2(x0, yp);
				if (x1 - xp < minDist) {
					minDist = x1 - xp;
					result = new Vector2(x1, yp);
				}
				if (yp - y0 < minDist) {
					minDist = yp - y0;
					result = new Vector2(xp, y0);
				}
				if (y1 - yp < minDist) {
					minDist = y1 - yp;
					result = new Vector2(xp, y1);
				}
				return result;
			} else {
				return new Vector2((xp < x0 ? x0 : (xp < x1 ? xp : x1)), y1);
			}
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