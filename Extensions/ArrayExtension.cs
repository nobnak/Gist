using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Gist.Extensions.Array {

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
    }
}
