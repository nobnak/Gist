using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
    
    public class Hash2D : AbstractHash {

        public Hash2D(float cellSize, int nx, int ny) 
            : base(cellSize, nx, ny, 1) {
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
    }
}
