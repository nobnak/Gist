//#define PARALLEL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.HashGridSystem.Storage {

    public class HashGrid3D<T> : System.IDisposable, IEnumerable<T> where T : class {
        System.Func<T, Vector3> _GetPosition;
        List<T>[] _grid;
        List<T> _points;
        List<Vector3> _positions;
        Hash _hash;

        public HashGrid3D(System.Func<T, Vector3> GetPosition, float cellSize, int nx, int ny, int nz) {
            this._GetPosition = GetPosition;
            this._points = new List<T> ();
            this._positions = new List<Vector3> ();
            Rebuild (cellSize, nx, ny, nz);
        }

        public Hash GridInfo { get { return _hash; } }
        public int Count { get { return _points.Count; } }

        public void Add(T point) {
            _points.Add (point);
            AddOnGrid(point, _GetPosition(point));
        }
        public void Remove(T point) {
            RemoveOnGrid(point, _GetPosition(point));
            _points.Remove (point);
        }
        public T IndexOf(int index) {
            return _points [index];
        }
        public T Find(System.Predicate<T> Predicate) {
            return _points.Find (Predicate);
        }
        public IEnumerable<S> Neighbors<S>(Vector3 center, float distance) where S : class, T {
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
        public void Rebuild(float cellSize, int nx, int ny, int nz) {
            _hash = new Hash (cellSize, nx, ny, nz);
            var totalCells = nx * ny * nz;
            if (_grid == null || _grid.Length != totalCells) {
                _grid = new List<T>[totalCells];
                for (var i = 0; i < _grid.Length; i++)
                    _grid [i] = new List<T> ();
            }
            Update ();
        }
        public void Update() {
            var limit = _grid.Length;
            #if PARALLEL
            Parallel.For(0, limit, (i) => _grid[i].Clear());
            #else
            for (var i = 0; i < limit; i++)
                _grid [i].Clear ();
            #endif

            limit = _points.Count;
            _positions.Clear ();
            for (var i = 0; i < limit; i++)
                _positions.Add(_GetPosition (_points [i]));

            #if PARALLEL
            Parallel.For (0, limit, Parallel_AddOnGrid);
            #else
            for (var i = 0; i < limit; i++)
                AddOnGrid (_points [i], _positions [i]);
            #endif
        }
        public IEnumerator UpdateAsync(MonoBehaviour m) {
            var limit = _grid.Length;
            yield return m.StartCoroutine (Parallel.ForAsync (0, limit, (i, arg) => _grid [i].Clear (), -1));

            limit = _points.Count;
            _positions.Clear ();
            for (var i = 0; i < limit; i++)
                _positions.Add(_GetPosition (_points [i]));

            yield return m.StartCoroutine (Parallel.ForAsync (0, limit, Parallel_AddOnGrid, -1));
        }
        public int Stat(int id) {
            return _grid [id].Count;
        }
        public int Stat(int x, int y, int z) {
            return Stat (_hash.CellId (x, y, z));
        }
        public int Stat(Vector3 pos) {
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
        void AddOnGrid (T point, Vector3 pos) {
            var id = _hash.CellId (pos);
            var cell = _grid [id];
            cell.Add (point);
        }
        void RemoveOnGrid (T point, Vector3 pos) {
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
            public readonly Vector3 gridSize;
            public readonly float cellSize;
            public readonly int nx, ny, nz;

            public Hash(float cellSize, int nx, int ny, int nz) {
                this.cellSize = cellSize;
                this.nx = nx;
                this.ny = ny;
                this.nz = nz;
                this.gridSize = new Vector3(nx * cellSize, ny * cellSize, nz * cellSize);
            }
            public IEnumerable<int> CellIds(Vector3 position, float radius) {
                var fromx = CellX (position.x - radius);
                var fromy = CellY (position.y - radius);
                var fromz = CellZ (position.z - radius);
                var widthx = CellX (position.x + radius) - fromx;
                var widthy = CellY (position.y + radius) - fromy;
                var widthz = CellZ (position.z + radius) - fromz;
                if (widthx < 0)
                    widthx += nx;
                if (widthy < 0)
                    widthy += ny;
                if (widthz < 0)
                    widthz += nz;

                for (var z = 0; z <= widthz; z++)
                    for (var y = 0; y <= widthy; y++)
                        for (var x = 0; x <= widthx; x++)
                            yield return CellId (x + fromx, y + fromy, z + fromz);
            }
            public int CellId(Vector3 position) {
                return CellId (CellX (position.x), CellY (position.y), CellZ (position.z));
            }
            public int CellId(int x, int y, int z) {
                x = Mod (x, nx);
                y = Mod (y, ny);
                z = Mod (z, nz);
                return x + (y + z * ny) * nx;
            }
            public int CellX(float posX) {
                posX -= gridSize.x * Mathf.CeilToInt (posX / gridSize.x);
                return (int)(posX / cellSize);
            }
            public int CellY(float posY) {
                posY -= gridSize.y * Mathf.CeilToInt (posY / gridSize.y);
                return (int)(posY / cellSize);
            }
            public int CellZ(float posZ) {
                posZ -= gridSize.z * Mathf.CeilToInt (posZ / gridSize.z);
                return (int)(posZ / cellSize);
            }
            public int Mod(int x, int mod) {
                return x - Mathf.FloorToInt ((float)x / mod) * mod;
            }
        }
    }
}
