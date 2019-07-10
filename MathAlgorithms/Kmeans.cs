using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms {

    public static class Kmeans {

        public static void Generate(
            int k, IList<Vector3> flowers, IList<int> clusters,
            out IList<Vector3> centers, out IList<int> counts, int iterationLimit = 100) {

            var ccenters = new Vector3[k];
            var ccounts = new int[k];
            
            for (var l = 0; l < iterationLimit; l++) {
                var cc = GenerateCenters(k, flowers, clusters, ref ccenters, ref ccounts);

                var changes = 0;
                for (var i = 0; i < flowers.Count; i++) {
                    var pos = flowers[i];
                    var cj = -1;
                    var cmin = float.MaxValue;
                    for (var j = 0; j < cc.Length; j++) {
                        var sq = (cc[j] - pos).sqrMagnitude;
                        if (sq < cmin) {
                            cmin = sq;
                            cj = j;
                        }
                    }
                    if (cj >= 0 && clusters[i] != cj) {
                        changes++;
                        clusters[i] = cj;
                    }
                }

                if (changes == 0)
                    break;
            }

            centers = GenerateCenters(k, flowers, clusters, ref ccenters, ref ccounts);
            counts = ccounts;
        }

        static Vector3[] GenerateCenters(
            int k, IList<Vector3> flowers, IList<int> clusters,
            ref Vector3[] ccenters, ref int[] ccounts) {

            System.Array.Clear(ccenters, 0, ccenters.Length);
            System.Array.Clear(ccounts, 0, ccounts.Length);

            for (var i = 0; i < flowers.Count; i++) {
                var pos = flowers[i];
                var ci = clusters[i];
                ccenters[ci] += pos;
                ccounts[ci]++;
            }
            for (var i = 0; i < ccounts.Length; i++) {
                if (ccounts[i] > 0)
                    ccenters[i] /= ccounts[i];
            }

            return ccenters;
        }
    }
}
