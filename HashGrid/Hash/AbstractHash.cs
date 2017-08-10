using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
        
    public abstract class AbstractHash {
        public readonly Vector3 gridSize;
        public readonly float cellSize;
        public readonly int nx, ny, nz;

        public AbstractHash(float cellSize, int nx, int ny, int nz) {
            this.cellSize = cellSize;
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.gridSize = new Vector3(nx * cellSize, ny * cellSize, nz * cellSize);
        }

        public int CellId(Vector2 position) {
            return CellId (CellX (position.x), CellY (position.y));
        }
        public virtual int CellId(Vector3 position) {
            return CellId (CellX (position.x), CellY (position.y), CellZ (position.z));
        }
        public int CellId(int x, int y) {
            x = Mod (x, nx);
            y = Mod (y, ny);
            return x + y * nx;
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
