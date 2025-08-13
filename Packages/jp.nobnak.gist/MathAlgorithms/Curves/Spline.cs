using nobnak.Gist.MathAlgorithms.Curves.CatmulRomSplineExt;
using nobnak.Gist.MathAlgorithms.Curves.Common;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves {

    public class Spline : BaseCurve {
        public const float EPSILON = 1e-6f;

        protected int parameterLength;
        protected CurvedLineStrip lines;

        public Spline() {
            validator.Reset();
            validator.Validation += () => {
                parameterLength = Mathf.Max(points.Count - 1, 0);
                if (parameterLength <= 0) {
                    validator.Invalidate();
                    return;
                }
                lines = new CurvedLineStrip(this);
            };
        }

        #region interface
        #region IParametricCurve
        public override bool Valid {
            get {
                validator.Validate();
                return parameterLength > 0;
            }
        }
        public override float ParameterLength {
            get {
                validator.Validate();
                return parameterLength;
            }
        }
        public override ILineStrip GetLineStrip() {
            validator.Validate();
            return lines;
        }
        public override Vector3 PositionAt(float t) {
            validator.Validate();
            Vector3 p1, p2, p0, p3;
            var ft = Parse(t, out p1, out p2, out p0, out p3);
            return ft.Position(p0, p1, p2, p3);
        }

        public override Vector3 DirectionAt(float t) {
            validator.Validate();
            Vector3 p1, p2, p0, p3;
            var ft = Parse(t, out p1, out p2, out p0, out p3);
            return ft.Velosity(p0, p1, p2, p3);
        }
        public override float CurvatureAt(float t) {
            validator.Validate();
            Vector3 p1, p2, p0, p3;
            var ft = Parse(t, out p1, out p2, out p0, out p3);
            return ft.Curvature(p0, p1, p2, p3);
        }
        #endregion
        #endregion

        #region member
        protected int Index(int i) {
            validator.Validate();
            return Mathf.Clamp(i, 0, parameterLength);
        }
        protected float Parse(float t, 
            out Vector3 p1, out Vector3 p2, out Vector3 p0, out Vector3 p3) {
            t = Mathf.Clamp(t, 0f, parameterLength - EPSILON);
            var i1 = Mathf.FloorToInt(t);
            var i2 = i1 + 1;
            var i0 = Index(i1 - 1);
            var i3 = Index(i2 + 1);

            p1 = points[i1];
            p2 = points[i2];
            p0 = points[i0];
            p3 = points[i3];
            var ft = t - i1;
            return ft;
        }
        #endregion
    }
}
