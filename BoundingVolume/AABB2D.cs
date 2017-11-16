using Gist.Pooling;
using UnityEngine;

namespace Gist.BoundingVolume {

    public class AABB2D {
        public static readonly Vector2 DEFAULT_MIN = new Vector2(float.MaxValue, float.MaxValue);
        public static readonly Vector2 DEFAULT_MAX = new Vector2(float.MinValue, float.MinValue);
        
        protected Vector2 min;
        protected Vector2 max;

        public AABB2D(Vector2 min, Vector2 max) {
            Set(min, max);
        }
        public AABB2D() : this(DEFAULT_MIN, DEFAULT_MAX) { }

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

        public void Encapsulate(Vector2 bmin, Vector2 bmax) {
            for (var i = 0; i < 2; i++) {
                var a0 = min[i];
                var a1 = max[i];
                var b0 = bmin[i];
                var b1 = bmax[i];
                min[i] = (a0 < b0 ? a0 : b0);
                max[i] = (a1 > b1 ? a1 : b1);
            }
        }
        public void Encapsulate(AABB2D b) {
            Encapsulate(b.min, b.max);
        }
        public void Encapsulate(Vector2 point) {
            Encapsulate(point, point);
        }

        public bool Contains(Vector2 point) {
            return min.x <= point.x && point.x <= max.x
                && min.y <= point.y && point.y <= max.y;
        }

        public void Clear() {
            Set(DEFAULT_MIN, DEFAULT_MAX);
        }

        public void Set(Vector2 min, Vector2 max) {
            this.min = min;
            this.max = max;
        }
        public void Set(Bounds bb) {
            Set(bb.min, bb.max);
        }

        #region Object
        public override string ToString() {
            return string.Format("AABB2D(center={0}, size={1})", Center, Size);
        }
        #endregion

        #region Converter
        public static implicit operator AABB2D(Rect bb) {
            return new AABB2D(bb.min, bb.max);
        }
        public static implicit operator Rect(AABB2D aa) {
            var b = new Rect();
            b.min = aa.min;
            b.max = aa.max;
            return b;
        }
        #endregion

        #region MemoryPool
        public static AABB2D New() {
            return new AABB2D();
        }
        public static void Reset(AABB2D aabb) {
            aabb.Clear();
        }
        public static void Delete(AABB2D aabb) {
        }
        public static IMemoryPool<AABB2D> CreateAABBPool() {
            return new MemoryPool<AABB2D>(New, Reset, Delete);
        }
        #endregion
    }
}
