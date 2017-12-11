using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist {

    public class PositionClustering : System.IDisposable {
        public System.Action<List<Vector2>> OnUpdateCluster;

        Data data;
        List<Point> _points;
        List<int> _clusterIDs;
        List<Vector2> _clusters;
        List<Vector3> _tmp;

        public PositionClustering(Data data) {
            this.data = data;
            _points = new List<Point> ();
            _clusterIDs = new List<int> ();
            _clusters = new List<Vector2> ();
            _tmp = new List<Vector3> ();            
        }

        #region IDisposable implementation 
        public void Dispose () { }
        #endregion

        public void Receive(Vector2 p) {
            _points.Add (new Point (p));
        }
        public void RemoveBeforeTime (float t) {
            var lastIndexOfOld = -1;
            for (var i = 0; i < _points.Count; i++) {
                if (_points [i].time >= t)
                    break;
                lastIndexOfOld = i;
            }
            if (lastIndexOfOld >= 0)
                _points.RemoveRange (0, lastIndexOfOld + 1);
        }
        public bool FindNearestCluster(Vector2 p, out int index, out float sqrd) {
            sqrd = float.MaxValue;
            index = -1;
            for (var j = 0; j < _clusters.Count; j++) {
                var q = _clusters [j];
                var jsqr = (q - p).sqrMagnitude;
                if (jsqr < sqrd) {
                    sqrd = jsqr;
                    index = j;
                }
            }
            return index >= 0;
        }
        public virtual void ClearPoints() {
            _points.Clear ();
        }
        public virtual void ClearCluster() {
            _clusterIDs.Clear ();
            _clusters.Clear ();
            _tmp.Clear ();
        }
        public virtual void UpdateCluster () {
            ClearCluster ();
            MakeClusters();
            UpdateClusterPosition();
            OnUpdateCluster (_clusters);
        }

        public virtual IEnumerable<Point> GetPointEnumerator() {
            foreach (var pp in _points)
                yield return pp;
        }
        public virtual IEnumerable<Vector2> GetClusterEnumerator() {
            foreach (var c in _clusters)
                yield return c;
        }

        public virtual int ClusterCount {
            get { return _clusters.Count; }
        }

        protected virtual void NotifyOnUpdateCluster(List<Vector2> clusters) {
            if (OnUpdateCluster != null)
                OnUpdateCluster (clusters);
        }
        protected virtual void MakeClusters () {
            var qd = data.clusterDistance * data.clusterDistance;
            for (var j = 0; j < _points.Count; j++) {
                var p = _points [j];
                float sqr;
                int i;
                if (FindNearestCluster (p.pos, out i, out sqr) 
                        && (sqr < qd || _clusters.Count >= data.clusterCountLimit)) {
                    _clusters [i] = p.pos;
                }
                else
                    if (_clusters.Count < data.clusterCountLimit) {
                        i = _clusters.Count;
                        _clusters.Add (p.pos);
                    }
                _clusterIDs.Add (i);
            }
        }
        void UpdateClusterPosition () {
            for (var i = 0; i < _clusters.Count; i++)
                _tmp.Add (Vector3.zero);
            switch (data.clusterMode) {
            case Data.ClusterModeEnum.Latest:
                for (var i = 0; i < _points.Count; i++) {
                    var pos = _points [i].pos;
                    _tmp [_clusterIDs [i]] = new Vector3 (pos.x, pos.y, 1f);
                }
                break;
            case Data.ClusterModeEnum.Average:
                for (var i = 0; i < _points.Count; i++) {
                    var pos = _points [i].pos;
                    var sum = _tmp [_clusterIDs [i]];
                    _tmp [_clusterIDs [i]] = data.averageTail * sum + new Vector3 (pos.x, pos.y, 1f);
                }
                break;
            }
            for (var i = 0; i < _clusters.Count; i++) {
                var v = _tmp [i];
                _clusters [i] = new Vector2 (v.x / v.z, v.y / v.z);
            }
        }


        public struct Point {
            public readonly Vector2 pos;
            public readonly float time;

            public Point(Vector2 pos, float time) {
                this.pos = pos;
                this.time = time;
            }
            public Point(Vector2 pos) : this(pos, Time.timeSinceLevelLoad) {}
        }

        [System.Serializable]
        public class Data {
            public enum ClusterModeEnum { Latest = 0, Average }
            public ClusterModeEnum clusterMode = ClusterModeEnum.Latest;
            public float effectiveDuration = 3f;
            public float clusterDistance = 0.2f;
            public float clusterCountLimit = 10;
            public float averageTail = 0.7f;
        }


    }
}