using Gist.Pooling;
using UnityEngine;

namespace Gist.BoundingVolume {

    public class AABB {
        public static readonly Vector3 DEFAULT_MIN = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        public static readonly Vector3 DEFAULT_MAX = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        protected Vector3 min;
        protected Vector3 max;

        public AABB(Vector3 min, Vector3 max) {
            Set(min, max);
        }
        public AABB() : this(DEFAULT_MIN, DEFAULT_MAX) { }

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

        public void Encapsulate(Vector3 bmin, Vector3 bmax) {
            for (var i = 0; i < 3; i++) {
                var a0 = min[i];
                var a1 = max[i];
                var b0 = bmin[i];
                var b1 = bmax[i];
                min[i] = (a0 < b0 ? a0 : b0);
                max[i] = (a1 > b1 ? a1 : b1);
            }
        }
        public void Encapsulate(AABB b) {
            Encapsulate(b.min, b.max);
        }
        public void Encapsulate(Vector3 point) {
            Encapsulate(point, point);
        }

        public bool Contains(Vector3 point) {
            return min.x <= point.x && point.x <= max.x
                && min.y <= point.y && point.y <= max.y
                && min.z <= point.z && point.z <= max.z;
        }

        public void Clear() {
            Set(DEFAULT_MIN, DEFAULT_MAX);
        }

        public void Set(Vector3 min, Vector3 max) {
            this.min = min;
            this.max = max;
        }
        public void Set(Bounds bb) {
            Set(bb.min, bb.max);
        }

        #region Object
        public override string ToString() {
            return string.Format("AABB(center={0}, size={1})", Center, Size);
        }
        #endregion

        #region Converter
        public static implicit operator AABB(Bounds bb) {
            return new AABB(bb.min, bb.max);
        }
        public static implicit operator Bounds(AABB aa) {
            var b = new Bounds();
            b.SetMinMax(aa.min, aa.max);
            return b;
        }
        #endregion

        #region MemoryPool
        public static AABB New() {
            return new AABB();
        }
        public static void Reset(AABB aabb) {
            aabb.Clear();
        }
        public static void Delete(AABB aabb) {
        }
        public static IMemoryPool<AABB> CreateAABBPool() {
            return new MemoryPool<AABB>(New, Reset, Delete);
        }
        #endregion
    }
}
