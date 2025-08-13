using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

	public class CustomList<T> : IList<T> {

		protected List<T> list = new List<T>();

		#region interface

		#region IList
		public virtual int Count => list.Count;
		public virtual bool IsReadOnly => false;
		public virtual T this[int index] {
			get => list[index];
			set => list[index] = value;
		}
		public virtual int IndexOf(T item) {
			return list.IndexOf(item);
		}
		public virtual void Add(T item) {
			list.Add(item);
		}
		public virtual void Insert(int index, T item) {
			list.Insert(index, item);
		}
		public virtual void RemoveAt(int index) {
			list.RemoveAt(index);
		}
		public virtual void Clear() {
			list.Clear();
		}
		public virtual bool Contains(T item) {
			return list.Contains(item);
		}
		public virtual void CopyTo(T[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}
		public virtual bool Remove(T item) {
			return list.Remove(item);
		}
		public virtual IEnumerator<T> GetEnumerator() {
			return list.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return list.GetEnumerator();
		}
		#endregion

		#endregion
	}
}