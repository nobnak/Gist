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

            GL.PushMatrix();
            GL.LoadPixelMatrix();

            var height = screen.y * size;
            var offset = new Vector2(gap, gap + height);
            foreach (var tex in targets) {
                var aspect = (float)tex.width / tex.height;
                var width = height * aspect;

                var rect = new Rect(offset.x, offset.y, width, -height);
                using (new RenderTextureActivator(destination)) {
                    Graphics.DrawTexture(rect, tex, mat);
                }

                offset.x += gap + height;
            }
            GL.PopMatrix();
        }
    }
}
