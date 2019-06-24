using System;
using System.Collections.Generic;
using System.Linq;

namespace nobnak.Gist {

    public static class EnumOperator<T> where T : IComparable, IFormattable, IConvertible {

        public static readonly string[] NAMES = Names;
        public static readonly T[] VALUES = Values.ToArray();
        public static readonly int[] INDICES = VALUES.Cast<int>().ToArray();
        public static readonly int MIN = INDICES.Min();
        public static readonly int MAX = INDICES.Max();

        public static string[] Names {
            get { return System.Enum.GetNames(typeof(T)); }
        }
        public static IEnumerable<T> Values {
            get { return System.Enum.GetValues(typeof(T)).Cast<T>(); }
        }

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

        public static int FindIndex(T value) {
            return VALUES.Select((v, i) => i).Where(i => VALUES[i].CompareTo(value) == 0).FirstOrDefault();
        }
    }
}
