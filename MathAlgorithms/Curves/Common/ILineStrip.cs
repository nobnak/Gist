using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.Common {

    public interface ILineStrip {
        float Length { get; }
        IList<Vector3> Points { get; }
        float FindParameter(float x, out int istart, out int iend);
        float FindParameter(float x);
    }
}
