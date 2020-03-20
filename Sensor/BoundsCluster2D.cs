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
		public event System.Action<IList<Cluster>> ClustersUpdated;
		public event System.Action<IList<Cluster>> ClusterdAdded;
		public event System.Action<IList<Cluster>> ClustersRemoved;

		Queue<Bounds> points;
		List<Cluster> clusters;
		List<Cluster> clustersAdded;
		List<Cluster> clustersRemoved;
		List<Cluster> clustersUpdated;
		Pooling.MemoryPool<Cluster> poolCluster;

		public BoundsCluster2D(Data data) {
			this.data = data;
			points = new Queue<Bounds>();
			clusters = new List<Cluster>();
			clustersAdded = new List<Cluster>();
			clustersRemoved = new List<Cluster>();
			clustersUpdated = new List<Cluster>();
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
					clustersRemoved.Add(clusters[oldestIndex]);
			}

			MakeClusters();
			RemoveOldClusters();
			Notify();

			clustersAdded.Clear();
			clustersRemoved.ForEach(c => poolCluster.Free(c));
			clustersRemoved.Clear();
			clustersUpdated.Clear();
		}

		public virtual void Clear() {
			clustersRemoved.AddRange(clusters);
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
					clustersAdded.Add(c);
				}

				c.Add(p);
				if (!clustersUpdated.Contains(c))
					clustersUpdated.Add(c);
			}
		}
		protected void RemoveOldClusters() {
			var t = Time.timeSinceLevelLoad - data.effectiveDuration;
			foreach (var c in clusters) {
				c.RemoveBeforeTime(t);
				if (c.Count == 0)
					clustersRemoved.Add(c);
			}

			foreach (var c in clustersRemoved)
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
			if (clustersUpdated.Count > 0)
				ClustersUpdated?.Invoke(clustersUpdated);
			if (clustersAdded.Count > 0)
				ClusterdAdded?.Invoke(clustersAdded);
			if (clustersRemoved.Count > 0)
				ClustersRemoved?.Invoke(clustersRemoved);
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