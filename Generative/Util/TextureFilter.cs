using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {
    
    public static class TextureFilter {
        
        public static float Bilinear1(Vector2 uv, int width, int height, System.Func<int, int, float> Value) {
            var lwidth = width - 1;
            var x = uv.x * lwidth;
            var y = uv.y * lwidth;

            var ix = (int)x;
            var iy = (int)y;
            ix = (ix < 0 ? 0 : (ix <= lwidth ? ix : lwidth));
            iy = (iy < 0 ? 0 : (iy <= lwidth ? iy : lwidth));

            var jx = ix + 1;
            var jy = iy + 1;
            jx = (jx <= lwidth ? jx : lwidth);
            jy = (jy <= lwidth ? jy : lwidth);

            var dx = x - ix;
            var dy = y - iy;

            return (1f - dx) * ((1f - dy) * Value (ix, iy) + dy * Value (ix, jy))
                + dx * ((1f - dy) * Value (jx, iy) + dy * Value (jx, jy));
        }
        public static Vector3 Bilinear3(Vector2 uv, int width, int height, System.Func<int, int, Vector3> Value) {
            var lwidth = width - 1;
            var x = uv.x * lwidth;
            var y = uv.y * lwidth;

            var ix = (int)x;
            var iy = (int)y;
            ix = (ix < 0 ? 0 : (ix <= lwidth ? ix : lwidth));
            iy = (iy < 0 ? 0 : (iy <= lwidth ? iy : lwidth));

            var jx = ix + 1;
            var jy = iy + 1;
            jx = (jx <= lwidth ? jx : lwidth);
            jy = (jy <= lwidth ? jy : lwidth);

            var dx1 = x - ix;
            var dy1 = y - iy;
            var dx0 = 1f - dx1;
            var dy0 = 1f - dy1;

            var vii = Value (ix, iy);
            var vij = Value (ix, jy);
            var vji = Value (jx, iy);
            var vjj = Value (jx, jy);

            return  new Vector3 (
                dx0 * (dy0 * vii.x + dy1 * vij.x) + dx1 * (dy0 * vji.x + dy1 * vjj.x),
                dx0 * (dy0 * vii.y + dy1 * vij.y) + dx1 * (dy0 * vji.y + dy1 * vjj.y),
                dx0 * (dy0 * vii.z + dy1 * vij.z) + dx1 * (dy0 * vji.z + dy1 * vjj.z));
        }
    }
}
