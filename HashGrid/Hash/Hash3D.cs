using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
    
    public class Hash3D : AbstractHash {
        
        public Hash3D(float cellSize, int nx, int ny, int nz) 
            : base(cellSize, nx, ny, nz) {
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
    }
}
