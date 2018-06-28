using System.Collections.Generic;
using nobnak.Gist.Pooling;
using nobnak.Gist.Primitive;
using UnityEngine;

namespace nobnak.Gist.Intersection {

    public class AABB3 : IConvex3Polytope {
        public static readonly Vector3 DEFAULT_MIN = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        public static readonly Vector3 DEFAULT_MAX = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        protected Vector3 min;
        protected Vector3 max;

        public AABB3(Vector3 min, Vector3 max) {
            Set(min, max);
        }
        public AABB3() : this(DEFAULT_MIN, DEFAULT_MAX) { }
		public AABB3(Bounds b) : this(b.min, b.max) { }

        public bool Empty { get { return min.x > max.x || min.y > max.y || min.z > max.z; } }
        public Vector3 Min {  get { return min; } }
        public Vector3 Max {  get { return max;  } }
        public Vector3 Center {
            get {
                var s = Size;
                return new Vector3(min.x + 0.5f * s.x, min.y + 0.5f * s.y, min.z + 0.5f * s.z);
            }
        }
        public Vector3 Size {
            get {
                return Empty ? Vector3.zero : new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
            }
        }
        public float SurfaceArea {
            get {
                var s = Size;
                return 2f * (s.x * s.y + s.y * s.z + s.z * s.x);
            }
        }

        public AABB3 Encapsulate(Vector3 bmin, Vector3 bmax) {
            for (var i = 0; i < 3; i++) {
                var a0 = min[i];
                var a1 = max[i];
                var b0 = bmin[i];
                var b1 = bmax[i];
                min[i] = (a0 < b0 ? a0 : b0);
                max[i] = (a1 > b1 ? a1 : b1);
            }
			return this;
        }
        public AABB3 Encapsulate(AABB3 b) {
            return Encapsulate(b.min, b.max);
        }
        public AABB3 Encapsulate(Vector3 point) {
            return Encapsulate(point, point);
        }

        public bool Contains(Vector3 point) {
            return min.x <= point.x && point.x <= max.x
                && min.y <= point.y && point.y <= max.y
                && min.z <= point.z && point.z <= max.z;
        }

        public AABB3 Clear() {
            return Set(DEFAULT_MIN, DEFAULT_MAX);
        }

        public AABB3 Set(Vector3 min, Vector3 max) {
            this.min = min;
            this.max = max;
			return this;
		}
		public AABB3 Set(Bounds bb) {
            return Set(bb.min, bb.max);
        }
		public AABB3 Set(FastBounds fb) {
			return Set(fb.Min, fb.Max);
		}

        #region IConvex3Polytope
        public IEnumerable<Vector3> Normals() {
            yield return Vector3.right;
            yield return Vector3.up;
            yield return Vector3.forward;
        }

        public IEnumerable<Vector3> Edges() {
            return Normals();
        }

        public IEnumerable<Vector3> Vertices() {
            for (var i = 0; i < 8; i++)
                yield return new Vector3(
                    (i & 1) == 0 ? min.x : max.x,
                    (i & 2) == 0 ? min.y : max.y,
                    (i & 4) == 0 ? min.z : max.z);
        }

        public FastBounds LocalBounds() {
            return this;
        }
        public FastBounds WorldBounds() {
            return this;
        }

        public IConvex3Polytope DrawGizmos() {
            var size = max - min;
            Gizmos.DrawWireCube(min + 0.5f * size, size);
            return this;
        }
        #endregion

        #region Object
        public override string ToString() {
            return string.Format("AABB(center={0}, size={1})", Center, Size);
        }
		#endregion

		#region Converter
		/*
        public static implicit operator AABB3(Bounds bb) {
            return new AABB3(bb.min, bb.max);
        }
		*/
		public static implicit operator Bounds(AABB3 aa) {
			var b = new Bounds();
			b.SetMinMax(aa.min, aa.max);
			return b;
		}
		public static implicit operator FastBounds(AABB3 aa) {
			return new FastBounds(aa.min, aa.max);
		}
		#endregion

		#region MemoryPool
		public static AABB3 New() {
            return new AABB3();
        }
        public static void Reset(AABB3 aabb) {
            aabb.Clear();
        }
        public static void Delete(AABB3 aabb) {
        }
        public static IMemoryPool<AABB3> CreateAABBPool() {
            return new MemoryPool<AABB3>(New, Reset, Delete);
        }
        #endregion
    }
}
