using nobnak.Gist.MathAlgorithms.Curves.Common;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves {

    public class CurvedLineStrip : ILineStrip {
        public const int N_SUBDIVITION = 10;
        public const float DX = 1f / N_SUBDIVITION;

        protected List<Vector3> points = new List<Vector3>();
        protected List<float> cumulativeLength = new List<float>();
        protected List<float> pointParameters = new List<float>();

        protected float totalLength;

        public CurvedLineStrip(IParametricCurve curve) {
            var last = curve.ParameterLength * N_SUBDIVITION;
            var p0 = curve.PositionAt(0f);
            totalLength = 0f;
            cumulativeLength.Add(totalLength);
            pointParameters.Add(0f);
            points.Add(p0);

            for (var i = 1; i <= last; i++) {
                var t = i * DX;
                var p1 = curve.PositionAt(t);
                var v = p1 - p0;
                totalLength += v.magnitude;
                cumulativeLength.Add(totalLength);
                pointParameters.Add(t);
                points.Add(p1);
                p0 = p1;
            }
        }

        #region interface
        #region ILineStrip
        public IList<Vector3> Points { get { return points; } }
        public float Length { get { return totalLength;} }
        public float CumulativeLengthAt(float t) {
            var i1 = pointParameters.BinarySearch(t);
            if (i1 < 0)
                i1 = ~i1;
            if (i1 <= 0)
                return 0f;
            if (i1 >= pointParameters.Count)
                return totalLength;

            var i0 = i1 - 1;
            var dt = Mathf.Clamp01((t - pointParameters[i0]) / DX);
            return Mathf.Lerp(cumulativeLength[i0], cumulativeLength[i1], dt);
        }
        public float FindParameter(float dist, out int istart, out int iend) {
            istart = 0;
            iend = cumulativeLength.Count - 1;
            if (dist < cumulativeLength[istart]) {
                iend = istart + 1;
                return pointParameters[istart];
            }
            if (cumulativeLength[iend] <= dist) {
                istart = iend - 1;
                return pointParameters[iend];
            }

            int idiff;
            while ((idiff = iend - istart) >= 2) {
                var imid = istart + idiff / 2;
                if (dist < cumulativeLength[imid])
                    iend = imid;
                else
                    istart = imid;
            }

            var t = (dist - cumulativeLength[istart]) /
                (cumulativeLength[iend] - cumulativeLength[istart]);
            return Mathf.Lerp(pointParameters[istart], pointParameters[iend], t);
        }
        public float FindParameter(float dist) {
            int istart, iend;
            return FindParameter(dist, out istart, out iend);
        }
        #endregion
        #endregion
    }
}
