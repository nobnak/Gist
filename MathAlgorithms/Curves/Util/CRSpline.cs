using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.CatmulRomSplineExt {

    public static class CRSpline {
        public static Vector3 Position(this float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var tm1 = t - 1f;
            var tm2 = tm1 * tm1;
            var t2 = t * t;

            var m1 = 0.5f * (p2 - p0);
            var m2 = 0.5f * (p3 - p1);

            return (1f + 2f * t) * tm2 * p1 + t * tm2 * m1 + t2 * (3 - 2f * t) * p2 + t2 * tm1 * m2;
        }
        public static Vector3 Velosity(this float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var tm1 = (t - 1f);
            var t6tm1 = 6f * t * tm1;

            var m1 = 0.5f * (p2 - p0);
            var m2 = 0.5f * (p3 - p1);

            return t6tm1 * p1 + (3f * t - 1f) * tm1 * m1 - t6tm1 * p2 + t * (3f * t - 2f) * m2;
        }
        public static Vector3 Acceleration(this float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var t2m1 = 2f * t - 1f;
            var t3 = 3f * t;

            var m1 = 0.5f * (p2 - p0);
            var m2 = 0.5f * (p3 - p1);
            return 6f * t2m1 * p1 + 2f * (t3 - 2f) * m1 - 6f * t2m1 * p2 + 2f * (t3 - 1f) * m2;
        }
        public static float Curvature(this float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var a = Acceleration(t, p0, p1, p2, p3);
            var v = Velosity(t, p0, p1, p2, p3);
            var vmag = v.magnitude;
            return Vector3.Cross(v, a).magnitude / (vmag * vmag * vmag);
        }
    }
}
