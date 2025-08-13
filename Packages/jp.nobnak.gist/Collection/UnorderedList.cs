using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

	public static class FixedIndexListExtension {
		public static void InsertBySwap<T>(this IList<T> list, int index, T item) {
			list.Add(list[index]);
			list[index] = item;
		}
		public static void RemoveAtBySwap<T>(this IList<T> list, int index) {
			var last = list.Count - 1;
			list[index] = list[last];
			list.RemoveAt(last);
		}
		public static bool RemvoeBySwap<T>(this IList<T> list, T item) {
			var index = list.IndexOf(item);
			var found = (index >= 0);
			if (found)
				list.RemoveAtBySwap(index);
			return found;
		}
	}

	public class FixedIndexList<T> : CustomList<T> {

		#region interface

		#region IList
		public override void Insert(int index, T item) {
			list.InsertBySwap(index, item);
		}
		public override bool Remove(T item) {
			return list.RemvoeBySwap(item);
		}
		public override void RemoveAt(int index) {
			list.RemoveAtBySwap(index);
		}
		#endregion

		#endregion
	}
}