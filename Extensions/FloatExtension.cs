using UnityEngine;
using System.Collections.Generic;

namespace nobnak.Gist.Extension.FloatArray {
    public static class FloatArrayExtension {

        public static int GreatestLowerBound(this float[] list, float value) {
            var index = GreatestLowerBound(list, 0, list.Length, value);
            return index;
        }

        private static int GreatestLowerBound(this float[] list, int leftmost, int length, float value) {
            if (length <= 1)
                return leftmost;

            var rightmost = leftmost + length - 1;
            var center = leftmost + ((length - 1) >> 1);
            var vCenter = list[center];
            var vNext = list[center + 1];

            if (value < vCenter)
                return GreatestLowerBound(list, leftmost, center - leftmost, value);
            if (vNext <= value)
                return GreatestLowerBound(list, center + 1, rightmost - center, value);
            return center;
        }

        public static void MakeCumulative(this float[] list) {
            var total = 0f;
            foreach (var v in list)
                total += v;
            var normalizer = 1f / total;

            var accum = 0f;
            for (var i = 0; i < list.Length; i++) {
                var val = list[i];
                list[i] = accum * normalizer;
                accum += val;
            }
        }
    }
}
