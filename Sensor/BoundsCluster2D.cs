using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.Primitive;
using System.Text;

namespace nobnak.Gist.Sensor {

	public class BoundsCluster2D : BoundsCluster2D<object> {
		public BoundsCluster2D(Data data) : base(data) { }
	}

	public class BoundsCluster2D<T> : BaseBoundsCluter2D {
		public System.Action<List<Cluster>> OnUpdateCluster;
		public System.Action<List<Cluster>> OnAddCluster;
		public System.Action<List<Cluster>> OnRemoveCluster;

		Queue<Bounds> points;
		List<Cluster> clusters;
		List<Cluster> clusterAdded;
		List<Cluster> clusterRemoved;
		Pooling.MemoryPool<Cluster> poolCluster;

		public BoundsCluster2D(Data data) {
			this.data = data;
			points = new Queue<Bounds>();
			clusters = new List<Cluster>();
			clusterAdded = new List<Cluster>();
			clusterRemoved = new List<Cluster>();
			poolCluster = new Pooling.MemoryPool<Cluster>(
				() => new Cluster(),
				c => c.Reset(),
				c => { });
		}

		#region IDisposable implementation
		public override void Dispose() {
			if (poolCluster != null) {
				poolCluster.Dispose();
				poolCluster = null;
			}
		}
		#endregion

		#region public
		public void Receive(FastBounds2D bb, params T[] args) {
			points.Enqueue(new Bounds(bb, args));
		}
		public bool FindNearestCluster(Vector2 center, out int index, out float sqrd) {
			sqrd = float.MaxValue;
			index = -1;
			for (var j = 0; j < clusters.Count; j++) {
				var c = clusters[j];
				var jsqr = (c.latest.center - center).sqrMagnitude;
				if (jsqr < sqrd) {
					sqrd = jsqr;
					index = j;
				}
			}
			return index >= 0;
		}
		public virtual void UpdateCluster() {
			if (clusters.Count > data.clusterCountLimit) {
				var oldestIndex = FindOldestClusterIndex();
				if (oldestIndex >= 0)
					clusterRemoved.Add(clusters[oldestIndex]);
			}

			MakeClusters();
			RemoveOldClusters();
			Notify();

			clusterAdded.Clear();
			clusterRemoved.ForEach(c => poolCluster.Free(c));
			clusterRemoved.Clear();
		}

		public virtual void Clear() {
			clusterRemoved.AddRange(clusters);
			clusters.Clear();
		}
		public virtual IEnumerable<Bounds> IteratePoints() {
			foreach (var c in clusters)
				yield return c.latest;
		}
		public virtual IEnumerable<Cluster> IterateClusters() {
			foreach (var c in clusters)
				yield return c;
		}

		public virtual int ClusterCount {
			get { return clusters.Count; }
		}
		#endregion

		#region private
		protected virtual void MakeClusters() {
			while (points.Count > 0) {
				float sqNearest;
				int i;
				Cluster c;

				var p = points.Dequeue();
				var pc = p.center;
				if (FindNearestCluster(pc, out i, out sqNearest)
					&& clusters[i].latest.bb.Contains(pc)
						|| (clusters.Count >= data.clusterCountLimit)) {
					c = clusters[i];
				} else {
					c = poolCluster.New();
					clusters.Add(c);
					clusterAdded.Add(c);
				}
				c.Add(p);
			}
		}
		protected void RemoveOldClusters() {
			var t = Time.timeSinceLevelLoad - data.effectiveDuration;
			foreach (var c in clusters) {
				c.RemoveBeforeTime(t);
				if (c.Count == 0)
					clusterRemoved.Add(c);
			}

			foreach (var c in clusterRemoved)
				clusters.Remove(c);
		}
		private int FindOldestClusterIndex() {
			var oldestTime = float.MaxValue;
			var oldestIndex = -1;
			for (var i = 0; i < clusters.Count; i++) {
				var c = clusters[i];
				if (c.latest.time < oldestTime) {
					oldestTime = c.latest.time;
					oldestIndex = i;
				}
			}

			return oldestIndex;
		}
		protected void Notify() {
			if (OnUpdateCluster != null)
				OnUpdateCluster(clusters);
			if (clusterAdded.Count > 0 && OnAddCluster != null)
				OnAddCluster(clusterAdded);
			if (clusterRemoved.Count > 0 && OnRemoveCluster != null)
				OnRemoveCluster(clusterRemoved);
		}
		#endregion

		#region classes
		public struct Bounds {
			public readonly FastBounds2D bb;
			public readonly Vector2 center;
			public readonly float time;
			public readonly T[] args;

			public Bounds(FastBounds2D bb, float time, params T[] args) {
				this.bb = bb;
				this.center = bb.Center;
				this.time = time;
				this.args = args;
			}
			public Bounds(FastBounds2D bb, params T[] args) : this(bb, Time.timeSinceLevelLoad, args) { }

			#region object
			public override string ToString() {
				return $"<{GetType().Name}:b={bb},c={center},t={time},nargs={args.Length}>";
			}
			#endregion
		}

		public class Cluster {
			public readonly List<Bounds> points = new List<Bounds>();
			public Bounds latest;

			public Cluster() {
				Reset();
			}

			public int Count {
				get { return points.Count; }
			}
			public Bounds Latest {
				get { return latest; }
			}
			public void Add(Bounds p) {
				points.Add(p);
				latest = p;
			}
			public void Reset() {
				points.Clear();
				latest = new Bounds(default(FastBounds2D), float.MinValue);
			}
			public void RemoveBeforeTime(float t) {
				while (points.Count > 0 && points[0].time < t)
					points.RemoveAt(0);
			}
		}
	}

	public abstract class BaseBoundsCluter2D : System.IDisposable {
		protected Data data;

		public abstract void Dispose();

		public Data CurrentData { get { return data; } }

		[System.Serializable]
        public class Data {
            public float effectiveDuration = 0.5f;
            public int clusterCountLimit = 100;
        }
		#endregion
	}
}