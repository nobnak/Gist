using nobnak.Gist.GLTools;
using nobnak.Gist.Scoped;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.VisualisationExt {
    public static class VisualizationExtension {

        public static void Draw(
            this IEnumerable<Texture> targets, 
            RenderTexture destination = null,
            float size = 0.2f,
            float gap = 10f,
            Material mat = default
            ) {

            var screen = (destination == null)
                ? new Vector2(Screen.width, Screen.height)
                : new Vector2(destination.width, destination.height);
			var screenAspect = screen.x / screen.y;

            GL.PushMatrix();
            GL.LoadPixelMatrix();
            var height = screen.y * size;
            var offset = new Vector2(gap, gap);
            foreach (var tex in targets) {
                var srcWidth = (float)tex.width;
                var srcHeight = (float)tex.height;

                var aspect = (float)srcWidth / srcHeight;
                var width = height * aspect;

#if false
                using (new RenderTextureActivator(destination)) {
                    var rect = new Rect(offset.x, offset.y, width, -height);
                    Graphics.DrawTexture(rect, tex, mat);
                }
#else
                var outputWidth = size * aspect / screenAspect;
                var outputHeight = size;
                var woffset = offset.x / screen.x;
                var hoffset = offset.y / screen.y;
                Copy.DrawTexture(tex, destination, outputWidth, outputHeight, woffset, hoffset);
#endif

                offset.x += gap + width;
            }
            GL.PopMatrix();
        }
    }
}
