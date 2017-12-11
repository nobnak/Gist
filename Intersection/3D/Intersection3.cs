using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.Intersection {
        
    public static class Intersection3 {
        public const float E = 1e-6f;

        public static void RangeAlongAxis(Vector3 axis, IEnumerable<Vector3> points, out float min, out float max) {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var p in points) {
                var v = Vector3.Dot (axis, p);
                if (v < min)
                    min = v;
                if (max < v)
                    max = v;
            }
        }

        public static bool Intersect(Vector3 axis, IEnumerable<Vector3> v0, IEnumerable<Vector3> v1) {
            float s0, e0, s1, e1;
            RangeAlongAxis(axis, v0, out s0, out e0);
            RangeAlongAxis(axis, v1, out s1, out e1);
            return s0 <= e1 && s1 <= e0;
        }
        public static bool Intersect(this IConvex3Polytope a, IConvex3Polytope b) {
            if (!a.WorldBounds ().Intersects (b.WorldBounds ()))
                return false;
            
            foreach (var ax in a.Normals())
                if (ax.sqrMagnitude > E && !Intersect (ax, a.Vertices (), b.Vertices ()))
                    return false;
                
            foreach (var bx in b.Normals())
                if (bx.sqrMagnitude > E && !Intersect (bx, a.Vertices (), b.Vertices ()))
                    return false;

            foreach (var ae in a.Edges()) {
                foreach (var be in b.Edges()) {
                    var cx = Vector3.Cross (ae, be);
                    if (cx.sqrMagnitude > E && !Intersect (cx, a.Vertices (), b.Vertices ()))
                        return false;
                }
            }
            return true;
        }

        public static bool Contains(Vector3 axis, IEnumerable<Vector3> v0, Vector3 p) {
            float s0, e0, se1;
            RangeAlongAxis(axis, v0, out s0, out e0);
            se1 = Vector3.Dot (axis, p);
            return s0 <= se1 && se1 <= e0;
        }
        public static bool Contains(this IConvex3Polytope a, Vector3 point) {
            if (!a.WorldBounds ().Contains (point))
                return false;

            foreach (var ax in a.Normals())
                if (ax.sqrMagnitude > E && !Contains (ax, a.Vertices (), point))
                    return false;

            return true;
        }
    }
}
