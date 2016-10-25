using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gist;

namespace Gist {

    public class PositionClustering : Settings<PositionClustering.Data> {
        public Camera targetCam;

        List<Point> _points;
        List<int> _clusterIDs;
        List<Vector2> _clusters;
        List<Vector3> _tmp;

        GLFigure _fig;

        public virtual void OnUpdateCluster(List<Vector2> clusters) {
        }
        public void Receive(Vector2 p) {
            _points.Add (new Point (p));
        }

        protected virtual void Awake() {
            _fig = new GLFigure ();
            _points = new List<Point> ();
            _clusterIDs = new List<int> ();
            _clusters = new List<Vector2> ();
            _tmp = new List<Vector3> ();
        }
        protected virtual void OnDestroy() {
            if (_fig != null)
                _fig.Dispose ();
        }
        protected virtual void OnRenderObject() {
            if ((Camera.current.cullingMask & (1 << gameObject.layer)) == 0)
                return;
            if (targetCam == null || _points == null || _clusters == null)
                return;
            if (!data.debugPointsVisible)
                return;

            var rot = targetCam.transform.rotation;
            var size = data.debugInputSize * Vector2.one;
            foreach (var pp in _points) {
                var p = (Vector3)pp.pos;
                p.z = data.debugInputDepth;
                _fig.FillCircle (targetCam.ViewportToWorldPoint (p), rot, size, data.debugInputColor);
            }
            for (var i = 0; i < _clusters.Count; i++) {
                var p = (Vector3)_clusters [i];
                p.z = data.debugInputDepth;
                _fig.FillCircle (targetCam.ViewportToWorldPoint (p), rot, size, data.debugClusterColor);
            }
        }
        protected override void Update() {
            base.Update ();
            DebugInput();

            var t = Time.timeSinceLevelLoad - data.effectiveDuration;
            RemoveBeforeTime (_points, t);
            UpdateCluster();
        }

        protected virtual void ClearPoints() {
            _points.Clear ();
        }
        protected virtual void ClearCluster() {
            _clusterIDs.Clear ();
            _clusters.Clear ();
            _tmp.Clear ();
        }
        protected virtual void UpdateCluster () {
            ClearCluster ();
            MakeClusters();
            UpdateClusterPosition();
            OnUpdateCluster (_clusters);
        }

        void MakeClusters () {
            var qd = data.clusterDistance * data.clusterDistance;
            for (var j = 0; j < _points.Count; j++) {
                var p = _points [j];
                float sqr;
                int i;
                if (FindNearest (_clusters, p.pos, out i, out sqr) && (sqr < qd || _clusters.Count >= data.clusterCountLimit)) {
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

        void DebugInput () {
            if (data.debugInputEnabled) {
                if (Input.GetMouseButton (0)) {
                    var p = (Vector2)targetCam.ScreenToViewportPoint (Input.mousePosition);
                    Receive (p);
                }
                if (Input.GetMouseButtonDown (1)) {

                }
            }
        }

        static void RemoveBeforeTime (List<Point> list, float t) {
            var lastIndexOfOld = -1;
            for (var i = 0; i < list.Count; i++) {
                if (list [i].time >= t)
                    break;
                lastIndexOfOld = i;
            }
            if (lastIndexOfOld >= 0)
                list.RemoveRange (0, lastIndexOfOld + 1);
        }
        static bool FindNearest(List<Vector2> list, Vector2 p, out int index, out float sqrd) {
            sqrd = float.MaxValue;
            index = -1;
            for (var j = 0; j < list.Count; j++) {
                var q = list [j];
                var jsqr = (q - p).sqrMagnitude;
                if (jsqr < sqrd) {
                    sqrd = jsqr;
                    index = j;
                }
            }
            return index >= 0;
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

            public bool debugInputEnabled = false;
            public bool debugPointsVisible = false;
            public float debugInputDepth = 10f;
            public float debugInputSize = 1f;
            public Color debugInputColor = new Color(1f, 0f, 0f, 0.2f);
            public Color debugClusterColor = new Color(0f, 1f, 1f, 0.5f);
        }


    }
}