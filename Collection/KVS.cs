using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace nobnak.Gist.Collection {

	public class KVS {

		public class Row<K> {
			public K key;

			#region interface

			#region object
			public override string ToString() {
				var tmp = new StringBuilder();
				tmp.Append($"<{GetType().Name} : ");
				foreach (var f in GetFields()) {
					tmp.Append($"{f.GetValue(this)},\t");
				}
				tmp.Append($">");
				return tmp.ToString();
			}
			#endregion

			public IEnumerable<FieldInfo> GetFields() {
				var bflags = BindingFlags.Instance | BindingFlags.Public;
				var typeOfR = GetType();
				var fieldOfKey = typeOfR.GetField(nameof(key));
				var fields = typeOfR.GetFields(bflags);
				var ikeyfield = System.Array.IndexOf(fields, fieldOfKey);
				if (ikeyfield < 0)
					throw new System.InvalidProgramException();
				if (ikeyfield > 0) {
					fields[ikeyfield] = fields[0]; ;
					fields[0] = fieldOfKey;
				}
				for (var j = 0; j < fields.Length; j++)
					yield return fields[j];
			}
			#endregion
		}
		public class Table<K, R>
			: IEnumerable<R>, IReadOnlyDictionary<K, R>
			where R : Row<K> {

			protected Dictionary<K, int> keyToIndex = new Dictionary<K, int>();
			protected Dictionary<int, K> indexToKey = new Dictionary<int, K>();
			protected List<R> rows = new List<R>();

			#region interface

			#region object
			public override string ToString() {
				var tmp = new StringBuilder();
				var count = rows.Count;
				tmp.AppendLine($"Table of {GetType().Name} : count={count}");
				if (count > 0) {
					var fields = rows[0].GetFields().ToArray();
					for (var i = 0; i < rows.Count; i++) {
						var r = rows[i];
						tmp.Append($"{i} : ");
						for (var j = 0; j < fields.Length; j++) {
							var f = fields[j];
							tmp.Append($"{f.GetValue(r)},\t");
						}
						tmp.AppendLine();
					}
				}
				return tmp.ToString();
			}
			#endregion

			#region IEnumerable
			public IEnumerator<R> GetEnumerator() {
				return rows.GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
			#endregion

			#region IReadonlyDictionary
			public IEnumerable<K> Keys => keyToIndex.Keys;
			public IEnumerable<R> Values => rows;
			public bool TryGetValue(K key, out R value) {
				var found = keyToIndex.TryGetValue(key, out int index);
				value = (found ? rows[index] : default);
				return found;
			}
			IEnumerator<KeyValuePair<K, R>> IEnumerable<KeyValuePair<K, R>>.GetEnumerator() {
				return rows.Select(v => new KeyValuePair<K, R>(v.key, v)).GetEnumerator();
			}
			#endregion

			public bool ContainsKey(K k) {
				return keyToIndex.ContainsKey(k);
			}
			public int Count { get => rows.Count; }
			public K KeyAt(int index) {
				return indexToKey[index];
			}
			public int IndexAt(K key) {
				return keyToIndex[key];
			}

			public R Get(K key) {
				if (!TryGetValue(key, out R value))
					throw new KeyNotFoundException($"Key({key}) Not Found");
				return value;
			}
			public void Set(R row) {
				var key = row.key;
				if (!keyToIndex.TryGetValue(key, out int index)) {
					index = rows.Count;
					rows.Add(row);
				} else {
					rows[index] = row;
#if UNITY_EDITOR
					Debug.LogWarning("Replace item with key={Key} : {row}");
#endif
				}

				keyToIndex[key] = index;
				indexToKey[index] = key;
			}
			public R this[K key] {
				get => Get(key);
			}
			public bool Remove(K key) {
				var found = keyToIndex.TryGetValue(key, out int index);
				if (found) {
					var ilast = rows.Count - 1;
					var rlast = rows[ilast];

					_RemoveKeyAt(index);
					if (index < ilast) {
						_RemoveKeyAt(ilast);
						_SetKeyAt(index, rlast.key);
						rows[index] = rlast;
					}
					rows.RemoveAt(ilast);
				}
				return found;
			}
			public bool Remove(Row<K> row) {
				return Remove(row.key);
			}
			#endregion

			#region method
			protected void _SetKeyAt(int index, K key) {
				keyToIndex[key] = index;
				indexToKey[index] = key;
			}
			protected void _RemoveKeyAt(int index) {
				var key = indexToKey[index];
				keyToIndex.Remove(key);
				indexToKey.Remove(index);
			}
			#endregion
		}
	}
}
