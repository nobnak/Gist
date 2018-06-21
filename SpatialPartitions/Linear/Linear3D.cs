using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.SpatialPartition {
	public class Linear3D : ISpatialPartition3D {

		protected List<int> listId = new List<int>();
		protected List<Vector3> listPosition = new List<Vector3>();

		#region ISpatialPartition3d
		public void Build(IEnumerable<int> ids, IEnumerable<Vector3> positions) {
			listId.Clear();
			listPosition.Clear();

			listId.AddRange(ids);
			listPosition.AddRange(positions);
		}

		public void Add(int id, Vector3 pos) {
			listId.Add(id);
			listPosition.Add(pos);
		}
		public int Neareset(Vector3 center) {
			var sqdist = float.MaxValue;
			var id = -1;
			for (var i = 0; i < listPosition.Count; i++) {
				var pos = listPosition[i];
				var tmpsq = (pos - center).sqrMagnitude;
				if (tmpsq < sqdist)
					id = listId[i];
			}
			return id;
		}

		public IEnumerable<int> RadialSearch(Vector3 center, float radius) {
			var sqrad = radius * radius;
			for (var i = 0; i < listPosition.Count; i++) {
				var pos = listPosition[i];
				if ((pos - center).sqrMagnitude < sqrad)
					yield return listId[i];
			}
		}

		public void Remove(int id) {
			var index = listId.IndexOf(id);
			if (index >= 0) {
				listId.RemoveAt(index);
				listPosition.RemoveAt(index);
			}

		}
		#endregion
	}
}
