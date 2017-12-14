using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Extensions.RectExt {

    public static class RectExtension {

        public static Vector2 ClosestPoint(this Rect rect, Vector2 point) {

            var min = rect.min;
            var max = rect.max;
            var x0 = min.x;
            var y0 = min.y;
            var x1 = max.x;
            var y1 = max.y;

            var xp = point.x;
            var yp = point.y;

            if (yp < y0) {
                return new Vector2((xp < x0 ? x0 : (xp < x1 ? xp : x1)), y0);
            } else if (yp < y1) {
                if (xp < x0)
                    return new Vector2(x0, yp);
                if (x1 < xp)
                    return new Vector2(x1, yp);

                var minDist = xp - x0;
                var result = new Vector2(x0, yp);
                if (x1 - xp < minDist) {
                    minDist = x1 - xp;
                    result = new Vector2(x1, yp);
                }
                if (yp - y0 < minDist) {
                    minDist = yp - y0;
                    result = new Vector2(xp, y0);
                }
                if (y1 - yp < minDist) {
                    minDist = y1 - yp;
                    result = new Vector2(xp, y1);
                }
                return result;
            } else {
                return new Vector2((xp < x0 ? x0 : (xp < x1 ? xp : x1)), y1);
            }
        }
    }
}