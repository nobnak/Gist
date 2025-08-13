using nobnak.Gist.ObjectExt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Pooling {

	public class MaterialPool : System.IDisposable {
		protected Dictionary<Tuple<int>, Value> tupleToValue = new Dictionary<Tuple<int>, Value>();
		protected Dictionary<Material, Tuple<int>> materialToTuple = new Dictionary<Material, Tuple<int>>();


		#region IDisposable
		public void Dispose() {
			var mats = materialToTuple.Keys.ToArray();
			materialToTuple.Clear();
			tupleToValue.Clear();
			foreach (var m in mats)
				m.DestroySelf();
			Debug.LogFormat("Destroy {0} materials from pool", mats.Length);
		}
		#endregion

		public Material New(Material prefab, params Object[] keys) {
			var tuple = GenerateTuple(keys);

			Value value;
			if (!tupleToValue.TryGetValue(tuple, out value)) {
				value = new Value(prefab);
				tupleToValue[tuple] = value;
			}
			value.Increment();
			var m = value.Instanced;
			materialToTuple[m] = tuple;
			return m;
		}

		public void Free(Material used) {
			Tuple<int> tuple;
			if (!materialToTuple.TryGetValue(used, out tuple))
				return;

			var value = tupleToValue[tuple];
			if (value.Decrement() <= 0) {
				materialToTuple.Remove(used);
				tupleToValue.Remove(tuple);
				value.Dispose();
			}
		}

		public Tuple<int> GenerateTuple(params Object[] keys) {
			return new Tuple<int>(keys.Select(k => k.GetInstanceID()).ToArray());
		}

		#region classes
		public class Value : System.IDisposable {
			protected Material mat;
			protected int count;

			public Value(Material prefab) {
				this.mat = Object.Instantiate(prefab);
				this.count = 0;
			}

			public Material Instanced {
				get { return mat; }
			}
			public int Count {
				get { return count; }
			}

			public int Increment() {
				return ++count;
			}
			public int Decrement() {
				return --count;
			}

			public void Dispose() {
				mat.DestroySelf();
			}
		}
		#endregion
	}
}
