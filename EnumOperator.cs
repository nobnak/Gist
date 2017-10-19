using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gist {

    public static class EnumOperator<T> {
        public static readonly int MIN = System.Enum.GetValues(typeof(T)).Cast<int>().Min();
        public static readonly int MAX = System.Enum.GetValues(typeof(T)).Cast<int>().Max();

        public static T Repeat(int i) {
            i = (i < MIN ? MAX : (i <= MAX ? i : MIN));
            return (T)(object)i;
        }
    }
}
