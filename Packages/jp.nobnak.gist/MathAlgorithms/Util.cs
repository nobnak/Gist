using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms {

    public static class Util {

        public static double SignificantFigure(this double value, int digits) {
            if (-double.Epsilon <= value && value <= double.Epsilon)
                return 0;

            var significandGen = (decimal)Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
            return (double)(significandGen * Math.Round((decimal)value / significandGen, digits));
        }
        public static float SignificantFigure(this float value, int digits) {
            return (float)SignificantFigure((double)value, digits);
        }
    }
}
