using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist {

    public class HashGrid2D : MonoBehaviour {
        public enum UpdateModeEnum { Update = 0, Rebuild }

        public UpdateModeEnum updateMode;
        public float cellSize = 1f;
        public int gridWidth = 20;
        public Color gizmoColor = Color.white;

        public static HashGrid<MonoBehaviour> World;

        HashGrid<MonoBehaviour> _world;

        void Awake() {
            _world = new HashGrid<MonoBehaviour> (GetPosition, cellSize, gridWidth, gridWidth);
            World = _world;
        }
        void LateUpdate() {
            switch (updateMode) {
            default:
                _world.Update ();
                break;
            case UpdateModeEnum.Rebuild:
                _world.Rebuild (cellSize, gridWidth, gridWidth);
                break;
            }
        }
        void OnDrawGizmosSelected() {
            if (_world == null)
                return;
            
            var size = gridWidth * cellSize * new Vector3 (1f, 1f, 0f);
			var center = transform.position;
            var offset = cellSize * new Vector3 (
                             Mathf.FloorToInt (center.x / cellSize) + 0.5f, 
                             Mathf.FloorToInt (center.y / cellSize) + 0.5f, 
                             Mathf.FloorToInt (center.z / cellSize) + 0.5f);
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube (center + 0.5f * size, size);

			var cubeSize = 0.5f * cellSize * Vector3.one;
			var hash = _world.GridInfo;
			for (var y = 0; y < hash.ny; y++) {
				for (var x = 0; x < hash.nx; x++) {
                    var pos = cellSize * new Vector3 (x, y, 0f) + offset;
					var count = _world.Stat (pos);
					if (count > 0) {
						var h = Mathf.Clamp01((float)count / 100);
						Gizmos.color = Jet (h, 0.5f * Mathf.Clamp01 (count / 10f));
                        Gizmos.DrawCube (pos, cubeSize);
                    }
                }
            }

            Gizmos.matrix = Matrix4x4.identity;
        }

        Vector2 GetPosition(MonoBehaviour m) {
            return (Vector2)transform.InverseTransformPoint (m.transform.position);
        }
		Color Jet(float x, float a) {
			return new Color(
				Mathf.Clamp01(Mathf.Min(4f * x - 1.5f, -4f * x + 4.5f)),
				Mathf.Clamp01(Mathf.Min(4f * x - 0.5f, -4f * x + 3.5f)),
				Mathf.Clamp01(Mathf.Min(4f * x + 0.5f, -4f * x + 2.5f)),
				a);
		}

        public class HashGrid<T> : System.IDisposable, IEnumerable<T> where T : class {
            System.Func<T, Vector2> _GetPosition;
            List<T>[] _grid;
            List<T> _points;
            List<Vector2> _positions;
            Hash _hash;

            public HashGrid(System.Func<T, Vector2> GetPosition, float cellSize, int nx, int ny) {
                this._GetPosition = GetPosition;
                this._points = new List<T> ();
                this._positions = new List<Vector2> ();
                Rebuild (cellSize, nx, ny);
            }

            public Hash GridInfo { get { return _hash; } }

            public void Add(T point) {
                _points.Add (point);
                AddOnGrid(point, _GetPosition(point));
            }
            public void Remove(T point) {
                RemoveOnGrid(point, _GetPosition(point));
                _points.Remove (point);
            }
            public T Find(System.Predicate<T> Predicate) {
                return _points.Find (Predicate);
            }
            public IEnumerable<S> Neighbors<S>(Vector2 center, float distance) where S : class, T {
                var r2 = distance * distance;
                foreach (var id in _hash.CellIds(center, distance)) {
                    var cell = _grid [id];
                    foreach (var p in cell) {
                        var s = p as S;
                        if (s == null)
                            continue;

                        var d2 = (_GetPosition (s) - center).sqrMagnitude;
                        if (d2 < r2)
                            yield return s;
                    }
                }
            }
            public void Rebuild(float cellSize, int nx, int ny) {
                _hash = new Hash (cellSize, nx, ny);
                var totalCells = nx * ny;
                if (_grid == null || _grid.Length != totalCells) {
                    _grid = new List<T>[totalCells];
                    for (var i = 0; i < _grid.Length; i++)
                        _grid [i] = new List<T> (100);
                }
                Update ();
            }
            public void Update() {
                var limit = _grid.Length;
                for (var i = 0; i < limit; i++)
                    _grid [i].Clear ();

                limit = _points.Count;
                _positions.Clear ();
                for (var i = 0; i < limit; i++)
                    _positions.Add(_GetPosition (_points [i]));
                Parallel.For (0, limit, (i) =>
                    AddOnGrid (_points [i], _positions [i])
                );

            }
            public int[,] Stat() {
                var counter = new int[_hash.nx, _hash.ny];
                for (var y = 0; y < _hash.ny; y++)
                    for (var x = 0; x < _hash.nx; x++)
                        counter [x, y] = _grid [_hash.CellId (x, y)].Count;
                return counter;
            }
            public int Stat(Vector2 pos) {
                var id = _hash.CellId (pos);
                var cell = _grid [id];
                return cell.Count;
            }

            void AddOnGrid (T point, Vector2 pos) {
                var id = _hash.CellId (pos);
                var cell = _grid [id];
                cell.Add(point);
            }
            void RemoveOnGrid (T point, Vector2 pos) {
                var id = _hash.CellId (pos);
                var cell = _grid [id];
                cell.Remove (point);
            }

            #region IDisposable implementation
            public void Dispose () {}
            #endregion

            #region IEnumerable implementation
            public IEnumerator<T> GetEnumerator () {
                return _points.GetEnumerator ();
            }
            #endregion

            #region IEnumerable implementation
            IEnumerator IEnumerable.GetEnumerator () {
                return this.GetEnumerator ();
            }
            #endregion

            public class Hash {
                public readonly Vector2 gridSize;
                public readonly float cellSize;
                public readonly int nx, ny;

                public Hash(float cellSize, int nx, int ny) {
                    this.cellSize = cellSize;
                    this.nx = nx;
                    this.ny = ny;
                    this.gridSize = new Vector2(nx * cellSize, ny * cellSize);
                }
                public IEnumerable<int> CellIds(Vector2 position, float radius) {
                    var fromx = CellX (position.x - radius);
                    var fromy = CellY (position.y - radius);
                    var widthx = CellX (position.x + radius) - fromx;
                    var widthy = CellY (position.y + radius) - fromy;
                    if (widthx < 0)
                        widthx += nx;
                    if (widthy < 0)
                        widthy += ny;

                    for (var y = 0; y <= widthy; y++)
                        for (var x = 0; x <= widthx; x++)
                            yield return CellId (x + fromx, y + fromy);
                }
                public int CellId(Vector2 position) {
                    return CellId (CellX (position.x), CellY (position.y));
                }
                public int CellId(int x, int y) {
                    x = Mod (x, nx);
                    y = Mod (y, ny);
                    return x + y * nx;
                }
                public int CellX(float posX) {
                    posX -= gridSize.x * Mathf.CeilToInt (posX / gridSize.x);
                    return (int)(posX / cellSize);
                }
                public int CellY(float posY) {
                    posY -= gridSize.y * Mathf.CeilToInt (posY / gridSize.y);
                    return (int)(posY / cellSize);
                }
                public int Mod(int x, int mod) {
                    return x - Mathf.FloorToInt ((float)x / mod) * mod;
                }
            }
        }
	}
}