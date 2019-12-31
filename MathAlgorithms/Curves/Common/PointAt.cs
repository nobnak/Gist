using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.Common {

    public struct ParametricPoint {

        public readonly IParametricCurve curve;
        public readonly float t;

        public ParametricPoint(IParametricCurve curve, float t) {
            this.curve = curve;
            this.t = t;
        }

        public Vector3 Position {
            get { return curve.PositionAt(t); }
        }
        public Vector3 Direction {
            get { return curve.DirectionAt(t); }
        }
        public float Curvature {
            get { return curve.CurvatureAt(t); }
        }
    }
}
