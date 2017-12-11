using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.Intersection {
        
    public static class Intersection2 {
        public const float E = 1e-6f;

        public static void RangeAlongAxis(Vector2 axis, IEnumerable<Vector2> points, out float min, out float max) {
            min = float.MaxValue;
            max = float.MinValue;
            foreach (var p in points) {
                var v = Vector2.Dot (axis, p);
                if (v < min)
                    min = v;
                if (max < v)
                    max = v;
            }
        }

        public static bool Intersect(Vector2 axis, IEnumerable<Vector2> v0, IEnumerable<Vector2> v1) {
            float s0, e0, s1, e1;
            RangeAlongAxis(axis, v0, out s0, out e0);
            RangeAlongAxis(axis, v1, out s1, out e1);
            return s0 <= e1 && s1 <= e0;
        }
        public static bool Intersect(this IConvex2Polytope a, IConvex2Polytope b) {
            if (!a.WorldBounds.Overlaps (b.WorldBounds))
                return false;
            
            foreach (var ax in a.Normals())
                if (ax.sqrMagnitude > E && !Intersect (ax, a.Vertices (), b.Vertices ()))
                    return false;
                
            foreach (var bx in b.Normals())
                if (bx.sqrMagnitude > E && !Intersect (bx, a.Vertices (), b.Vertices ()))
                    return false;
            
            return true;
        }

        public static bool Contains(Vector2 axis, IEnumerable<Vector2> v0, Vector2 p) {
            float s0, e0, se1;
            RangeAlongAxis(axis, v0, out s0, out e0);
            se1 = Vector2.Dot (axis, p);
            return s0 <= se1 && se1 <= e0;
        }
        public static bool Contains(this IConvex2Polytope a, Vector2 point) {
            if (!a.WorldBounds.Contains (point))
                return false;

            foreach (var ax in a.Normals())
                if (ax.sqrMagnitude > E && !Contains (ax, a.Vertices (), point))
                    return false;

            return true;
        }
    }
}
