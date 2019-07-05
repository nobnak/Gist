using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.LinqExt {

	public static class LinqExtension {

		public static void ForEach<T>(this IEnumerable<T> ts, System.Action<T> action) {
			foreach (var t in ts)
				action(t);
		}
	}
}