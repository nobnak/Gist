using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.Sensor {

    public class PositionCluster3D : System.IDisposable {
        public System.Action<List<Cluster>> OnUpdateCluster;
		public System.Action<List<Cluster>> OnAddCluster;
		public System.Action<List<Cluster>> OnRemoveCluster;

        Data data;
        Queue<Point> points;
        List<Cluster> clusters;
		List<Cluster> clusterAdded;
		List<Cluster> clusterRemoved;
		Pooling.MemoryPool<Cluster> poolCluster;

        public PositionCluster3D(Data data) {
            this.data = data;
            points = new Queue<Point> ();
            clusters = new List<Cluster> ();
			clusterAdded = new List<Cluster>();
			clusterRemoved = new List<Cluster>();
			poolCluster = new Pooling.MemoryPool<Cluster>(
				() => new Cluster(),
				c => c.Reset(),
				c => { });
        }

        #region IDisposable implementation 
        public void Dispose () {
			if (poolCluster != null) {
				poolCluster.Dispose();
				poolCluster = null;
			}
		}
		#endregion

		#region public
		public void Receive(Vector3 p) {
			points.Enqueue(new Point(p));
        }
        public bool FindNearestCluster(Vector3 center, out int index, out float sqrd) {
            sqrd = float.MaxValue;
            index = -1;
            for (var j = 0; j < clusters.Count; j++) {
                var c = clusters [j];
                var jsqr = (c.latest.pos - center).sqrMagnitude;
                if (jsqr < sqrd) {
                    sqrd = jsqr;
                    index = j;
                }
            }
            return index >= 0;
        }
        public virtual void UpdateCluster () {
			clusterAdded.Clear();
			clusterRemoved.ForEach(c => poolCluster.Free(c));
			clusterRemoved.Clear();
			if (clusters.Count > data.clusterCountLimit) {
				var oldestIndex = FindOldestClusterIndex();
				if (oldestIndex >= 0)
					clusters.RemoveAt(oldestIndex);
			}

			MakeClusters();
			RemoveOldClusters();

			Notify();
		}

		public virtual void Clear() {
			clusterRemoved.AddRange(clusters);
			clusters.Clear();
		}
		public virtual IEnumerable<Point> IteratePoints() {
            foreach (var c in clusters)
                yield return c.latest;
        }
		public virtual IEnumerable<Vector3> IteratePositions() {
			foreach (var p in IteratePoints())
				yield return p.pos;
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
        protected virtual void MakeClusters () {
            var sqClusterDist = data.clusterDistance * data.clusterDistance;

			while (points.Count > 0) {
				float sqNearest;
                int i;
				Cluster c;

				var p = points.Dequeue();
				if (FindNearestCluster(p.pos, out i, out sqNearest)
						&& (sqNearest < sqClusterDist 
						|| clusters.Count >= data.clusterCountLimit)) {
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
			for (var i = 0; i < clusters.Count;) {
				var c = clusters[i];
				c.RemoveBeforeTime(t);
				if (c.Count == 0) {
					clusters.RemoveAt(i);
					clusterRemoved.Add(c);
				} else {
					i++;
				}
			}
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
		public struct Point {
            public readonly Vector3 pos;
            public readonly float time;
			public readonly object[] args;

            public Point(Vector3 pos, float time, params object[] args) {
                this.pos = pos;
                this.time = time;
				this.args = args;
            }
            public Point(Vector3 pos) : this(pos, Time.timeSinceLevelLoad) {}
        }

		public class Cluster {
			public readonly List<Point> points = new List<Point>();
			public Point latest;

			public Cluster() {
				Reset();
			}

			public int Count {
				get { return points.Count; }
			}
			public Point Latest {
				get { return latest; }
			}
			public void Add(Point p) {
				points.Add(p);
				if (latest.time < p.time) {
					latest = p;
				}
			}
			public void Reset() {
				points.Clear();
				latest = new Point(default(Vector3), float.MinValue);
			}
			public void RemoveBeforeTime(float t) {
				var lastIndexOfOld = -1;
				for (var i = 0; i < points.Count; i++) {
					if (points[i].time >= t)
						break;
					lastIndexOfOld = i;
				}
				if (lastIndexOfOld >= 0)
					points.RemoveRange(0, lastIndexOfOld + 1);
			}

			public static Vector3 operator - (Cluster a, Cluster b) {
				return b.latest.pos - a.latest.pos;
			}
		}

        [System.Serializable]
        public class Data {
            public float effectiveDuration = 3f;
            public float clusterDistance = 0.2f;
            public int clusterCountLimit = 100;
        }
		#endregion
	}
}