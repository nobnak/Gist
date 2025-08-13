using UnityEngine;
using System.Collections.Generic;

namespace nobnak.Gist.Extensions.Range {

    public static class RangeExtension {
    
        public static IEnumerable<T> Range<T>(this IEnumerable<T> seq, int offset, int length) {
            var iter = seq.GetEnumerator ();

            for (var i = 0; iter.MoveNext () && i < offset; i++)
                ;
            for (var i = 0; iter.MoveNext () && i < length; i++)
                yield return iter.Current;
        }
        public static IEnumerable<T> Range<T>(this IList<T> seq, int offset, int length) {
            for (var i = 0; i < length; i++)
                yield return seq [offset + i];
        }
        public static IEnumerable<T> Range<T>(this IReadOnlyList<T> seq, int offset, int length) {
            for (var i = 0; i < length; i++)
                yield return seq[offset + i];
        }
    }
}