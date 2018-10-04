using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.SpatialPartition {
	public class Linear3D<T> : ISpatialPartition<T, Vector3> {

		protected List<T> entities = new List<T>();
		protected List<Vector3> positions = new List<Vector3>();

		#region ISpatialPartition3d
		public void Add(T entity) {
			entities.Add(entity);
			positions.Add(default(Vector3));
		}
		public void Remove(T entity) {
			var index = entities.IndexOf(entity);
			if (index >= 0) {
				entities.RemoveAt(index);
				positions.RemoveAt(index);
			}

		}
		public T Neareset(Vector3 center) {
			var sqdist = float.MaxValue;
			var j = -1;
			for (var i = 0; i < positions.Count; i++) {
				var pos = positions[i];
				var tmpsq = (pos - center).sqrMagnitude;
				if (tmpsq < sqdist)
					j = i;
			}
			return entities[j];
		}

		public IEnumerable<T> RadialSearch(Vector3 center, float radius) {
			var sqrad = radius * radius;
			for (var i = 0; i < positions.Count; i++) {
				var pos = positions[i];
				if ((pos - center).sqrMagnitude < sqrad)
					yield return entities[i];
			}
		}

		public void UpdatePosition(Func<T, Vector3> getPosition) {
			for (var i = 0; i < entities.Count; i++)
				positions[i] = getPosition(entities[i]);
		}

		#endregion
	}
}
