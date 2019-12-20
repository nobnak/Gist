using nobnak.Gist.MathAlgorithms.Curves.Common;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves {

    public class CurvedLineStrip : ILineStrip {
        protected List<Vector3> points = new List<Vector3>();
        protected List<float> cummulativeLength = new List<float>();
        protected List<float> pointParameters = new List<float>();

        protected float totalLength;

        public CurvedLineStrip(IParametricCurve curve) {
            var subdivision = 10;
            totalLength = 0f;

            var dx = 1f / subdivision;
            var last = curve.ParameterLength * subdivision;
            var p0 = curve.PositionAt(0f);
            cummulativeLength.Add(totalLength);
            pointParameters.Add(0f);
            points.Add(p0);

            for (var i = 1; i <= last; i++) {
                var t = i * dx;
                var p1 = curve.PositionAt(t);
                var v = p1 - p0;
                totalLength += v.magnitude;
                cummulativeLength.Add(totalLength);
                pointParameters.Add(t);
                points.Add(p1);
                p0 = p1;
            }
        }

        #region interface
        #region ILineStrip
        public IList<Vector3> Points { get { return points; } }
        public float Length { get { return totalLength;} }
        public float FindParameter(float dist, out int istart, out int iend) {
            istart = 0;
            iend = cummulativeLength.Count - 1;
            if (dist < cummulativeLength[istart]) {
                iend = istart + 1;
                return 0f;
            }
            if (cummulativeLength[iend] <= dist) {
                istart = iend - 1;
                return 0f;
            }

            int idiff;
            while ((idiff = iend - istart) >= 2) {
                var imid = istart + idiff / 2;
                if (dist < cummulativeLength[imid])
                    iend = imid;
                else
                    istart = imid;
            }

            var t = (dist - cummulativeLength[istart]) /
                (cummulativeLength[iend] - cummulativeLength[istart]);
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
