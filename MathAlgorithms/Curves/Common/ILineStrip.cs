using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.Common {

    public interface ILineStrip {
        IList<Vector3> Points { get; }
        float Length { get; }
        float CumulativeLengthAt(float t);
        float FindParameter(float x, out int istart, out int iend);
        float FindParameter(float x);
    }
}
