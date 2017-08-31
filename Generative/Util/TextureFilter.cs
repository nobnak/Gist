using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {
    
    public static class TextureFilter {
        
        public static float Bilinear(Vector2 uv, int width, int height, System.Func<int, int, float> Value) {
            var lwidth = width - 1;
            var x = uv.x * lwidth;
            var y = uv.y * lwidth;
            var ix = Mathf.Clamp ((int)x, 0, lwidth);
            var iy = Mathf.Clamp ((int)y, 0, lwidth);
            var jx = Mathf.Clamp (ix + 1, 0, lwidth);
            var jy = Mathf.Clamp (iy + 1, 0, lwidth);
            var dx = x - ix;
            var dy = y - iy;

            return (1f - dx) * ((1f - dy) * Value (ix, iy) + dy * Value (ix, jy))
                + dx * ((1f - dy) * Value (jx, iy) + dy * Value (jx, jy));
        }
        public static Vector3 Bilinear(Vector2 uv, int width, int height, System.Func<int, int, Vector3> Value) {
            var lwidth = width - 1;
            var x = uv.x * lwidth;
            var y = uv.y * lwidth;
            var ix = Mathf.Clamp ((int)x, 0, lwidth);
            var iy = Mathf.Clamp ((int)y, 0, lwidth);
            var jx = Mathf.Clamp (ix + 1, 0, lwidth);
            var jy = Mathf.Clamp (iy + 1, 0, lwidth);
            var dx = x - ix;
            var dy = y - iy;

            return (1f - dx) * ((1f - dy) * Value (ix, iy) + dy * Value (ix, jy))
                + dx * ((1f - dy) * Value (jx, iy) + dy * Value (jx, jy));
        }
    }
}
