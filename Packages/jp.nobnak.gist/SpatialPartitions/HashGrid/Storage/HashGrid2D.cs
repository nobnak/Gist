#define PARALLEL

using nobnak.Gist.SpatialPartition;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.HashGridSystem.Storage {

    public class HashGrid2D<T> : System.IDisposable, IEnumerable<T>, ISpatialPartition<T, Vector2>
		where T : class {

        System.Func<T, Vector2> _GetPosition;
        List<T>[] _grid;
        List<T> _points;
        List<Vector2> _positions;
        Hash _hash;

        public HashGrid2D(System.Func<T, Vector2> GetPosition, float cellSize, int nx, int ny) {
            this._GetPosition = GetPosition;
            this._points = new List<T> ();
            this._positions = new List<Vector2> ();
            Rebuild (cellSize, nx, ny);
        }

		#region ISpatialPartition
		public void Add(T point) {
			_points.Add(point);
			AddOnGrid(point, _GetPosition(point));
		}
		public void Remove(T point) {
			RemoveOnGrid(point, _GetPosition(point));
			_points.Remove(point);
		}
		public void UpdatePosition(Func<T, Vector2> getPosition) {
			throw new NotImplementedException();
		}

		public IEnumerable<T> RadialSearch(Vector2 center, float radius) {
			throw new NotImplementedException();
		}

		public T Neareset(Vector2 center) {
			throw new NotImplementedException();
		}
		#endregion

		public Hash GridInfo { get { return _hash; } }
        public int Count { get { return _points.Count; } }

        public T IndexOf(int index) {
            return _points [index];
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
            #if PARALLEL
            Parallel.For(0, limit, (i, arg) => _grid[i].Clear(), -1);
            #else
            for (var i = 0; i < limit; i++)
                _grid [i].Clear ();
            #endif

            limit = _points.Count;
            _positions.Clear ();
            for (var i = 0; i < limit; i++)
                _positions.Add(_GetPosition (_points [i]));

            #if PARALLEL
            Parallel.For (0, limit, Parallel_AddOnGrid, -1);
            #else
            for (var i = 0; i < limit; i++)
                AddOnGrid (_points [i], _positions [i]);
            #endif
        }
        public int Stat(int id) {
            return _grid [id].Count;
        }
        public int Stat(int x, int y) {
            return Stat (_hash.CellId (x, y));
        }
        public int Stat(Vector2 pos) {
            return Stat (_hash.CellId (pos));
        }

        void Parallel_AddOnGrid(int i, int arg) {
            var point = _points [i];
            var pos = _positions [i];
            var id = _hash.CellId (pos);
            var cell = _grid [id];
            lock (cell) {
                cell.Add (point);
            }
        }
        void AddOnGrid (T point, Vector2 pos) {
            var id = _hash.CellId (pos);
            var cell = _grid [id];
            cell.Add (point);
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
