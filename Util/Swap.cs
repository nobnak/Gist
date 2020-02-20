using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Util {

    public static class Swap {

        public static void Do<T>(ref T a, ref T b) {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}
