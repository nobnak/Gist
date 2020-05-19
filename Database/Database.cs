using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Database {
	
	public abstract class Row : System.IDisposable {
		protected readonly RefCounter<Row> counter;

		public Row() {
			this.counter = new RefCounter<Row>(this);
		}

		#region interface

		#region IDisposable
		public virtual void Dispose() {
		}
		#endregion

		#endregion
	}

	public class Database<T> : System.IDisposable where T : Row {

		protected List<T> rows = new List<T>();

		#region interface

		#region IDisposable
		public void Dispose() {
		}
		#endregion

		public virtual int Count => rows.Count;
		public virtual T this[int index] {
			get => rows[index];
		}
		public virtual void Add(T item) {
			rows.Add(item);
		}
		public virtual bool Remove(T item) {
			return rows.Remove(item);
		}

		#endregion
	}
}