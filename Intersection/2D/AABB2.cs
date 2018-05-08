using System.Collections.Generic;
using nobnak.Gist.Pooling;
using UnityEngine;

namespace nobnak.Gist.Intersection {

    public class AABB2 : IConvex2Polytope {
        public static readonly Vector2 DEFAULT_MIN = new Vector2(float.MaxValue, float.MaxValue);
        public static readonly Vector2 DEFAULT_MAX = new Vector2(float.MinValue, float.MinValue);
        
        protected Vector2 min;
        protected Vector2 max;
        protected Rect bounds;

        public AABB2(Vector2 min, Vector2 max) {
            Set(min, max);
        }
        public AABB2() : this(DEFAULT_MIN, DEFAULT_MAX) { }
		public AABB2(Rect r) : this(r.min, r.max) { }

        public bool Empty { get { return min.x > max.x || min.y > max.y; } }
        public Vector2 Min {  get { return min; } }
        public Vector2 Max {  get { return max;  } }
        public Vector2 Center {
            get {
                var s = Size;
                return new Vector2(min.x + 0.5f * s.x, min.y + 0.5f * s.y);
            }
        }
        public Vector2 Size {
            get {
                return Empty ? Vector2.zero : new Vector2(max.x - min.x, max.y - min.y);
            }
        }
        public float SurfaceArea {
            get {
                var s = Size;
                return s.x * s.y;
            }
        }

        public AABB2 Encapsulate(Vector2 bmin, Vector2 bmax) {
            for (var i = 0; i < 2; i++) {
                var a0 = min[i];
                var a1 = max[i];
                var b0 = bmin[i];
                var b1 = bmax[i];
                min[i] = (a0 < b0 ? a0 : b0);
                max[i] = (a1 > b1 ? a1 : b1);
            }
			return this;
        }
        public AABB2 Encapsulate(AABB2 b) {
            return Encapsulate(b.min, b.max);
        }
        public AABB2 Encapsulate(Rect r) {
            return Encapsulate(r.min, r.max);
        }
        public AABB2 Encapsulate(Vector2 point) {
            return Encapsulate(point, point);
        }

        public bool Contains(Vector2 point) {
            return min.x <= point.x && point.x <= max.x
                && min.y <= point.y && point.y <= max.y;
        }

        public AABB2 Clear() {
            return Set(DEFAULT_MIN, DEFAULT_MAX);
        }

        public AABB2 Set(Vector2 min, Vector2 max) {
            this.min = min;
            this.max = max;
            this.bounds = new Rect(min, max - min);
			return this;
		}
		public AABB2 Set(Rect bb) {
            return Set(bb.min, bb.max);
		}

		#region IConvex2Polytope
		public IEnumerable<Vector2> Normals() {
			yield return Vector2.right;
			yield return Vector2.up;
		}
		public IEnumerable<Vector2> Vertices() {
			yield return min;
			yield return new Vector2(min.x, max.y);
			yield return max;
			yield return new Vector2(max.x, min.y);
		}
		public Rect WorldBounds {
			get { return bounds; }
		}
		public Matrix4x4 Model {
			get { return Matrix4x4.identity; }
		}
		#endregion

		#region Object
		public override string ToString() {
            return string.Format("AABB2(center={0}, size={1})", Center, Size);
        }
        #endregion

        #region Converter
		/*
        public static implicit operator AABB2(Rect bb) {
            return new AABB2(bb.min, bb.max);
        }
		*/
        public static implicit operator Rect(AABB2 aa) {
            var b = new Rect();
            b.min = aa.min;
            b.max = aa.max;
            return b;
        }
        #endregion

        #region MemoryPool
        public static AABB2 New() {
            return new AABB2();
        }
        public static void Reset(AABB2 aabb) {
            aabb.Clear();
        }
        public static void Delete(AABB2 aabb) {
        }
        public static IMemoryPool<AABB2> CreateAABBPool() {
            return new MemoryPool<AABB2>(New, Reset, Delete);
        }
        #endregion
    }
}
