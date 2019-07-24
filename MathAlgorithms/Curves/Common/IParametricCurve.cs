﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.MathAlgorithms.Curves.Common {

    public interface IParametricCurve {
        event System.Action Changed;

        bool Valid { get; }
        float ParameterLength { get; }

        Vector3 PositionAt(float t);
        Vector3 DirectionAt(float t);
        float CurvatureAt(float t);
        ILineStrip GetLineStrip();
    }
}