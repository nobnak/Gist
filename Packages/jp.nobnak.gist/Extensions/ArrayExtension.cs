using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace nobnak.Gist.Extensions.Array {

    public static class ArrayExtension {

        public static string ToStringElements(this IList data, string elementFormat = "{0}") {
            var buf = new StringBuilder("{");
            foreach (var v in data) {
                buf.AppendFormat(elementFormat, v);
                buf.Append(", ");
            }
            buf.Append("}");
            return buf.ToString();
        }

		public static void AddRange<T>(this IList<T> dst, IEnumerable<T> src) {
			foreach (var s in src)
				dst.Add(s);
		}
		public static T[] Swap<T>(this T[] array, int i, int j) {
			var tmp = array[i];
			array[i] = array[j];
			array[j] = tmp;
			return array;
		}
		public static T[] Shuffle<T>(this T[] array, int seed = -1) {
			var oldState = Random.state;
			if (seed >= 0)
				Random.InitState(seed);
			for (var i = array.Length - 1; i >= 0; --i) {
				var j = Random.Range(0, i + 1);
				if (i != j)
					array.Swap(i, j);
			}
			Random.state = oldState;
			return array;
		}
	}
}
