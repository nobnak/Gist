using System;
using System.Linq;

namespace nobnak.Gist {

    public static class EnumOperator<T> where T : IComparable {

        public static readonly string[] NAMES = System.Enum.GetNames(typeof(T));
        public static readonly T[] VALUES = System.Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        public static readonly int[] INDICES = VALUES.Cast<int>().ToArray();

        public static readonly int MIN = INDICES.Min();
        public static readonly int MAX = INDICES.Max();

        public static T Repeat(int i) {
            i = (i < MIN ? MAX : (i <= MAX ? i : MIN));
            return (T)(object)i;
        }

        public static T ValueAt(int index) {
            return VALUES[index];
        }
        public static string NameAt(int index) {
            return NAMES[index];
        }

        internal static int FindIndex(T value) {
            return VALUES.Select((v,i)=>i).Where(i => VALUES[i].CompareTo(value) == 0).FirstOrDefault();
        }
    }
}
