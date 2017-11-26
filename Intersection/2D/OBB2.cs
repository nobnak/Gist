using Gist.Extensions.AABB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Intersection {

    public class OBB2 : IConvex2Polytope {
        public static readonly Vector2[] VERTICES = new Vector2[] {
            0.5f * new Vector2(-1,-1), 0.5f * new Vector2(-1,1),
            0.5f * Vector2.one, 0.5f * new Vector2(1,-1)
        };
        public static readonly Rect QUAD = new Rect(-0.5f, -0.5f, 1f, 1f);

        protected Matrix4x4 model;
        protected Rect bounds;

        public OBB2(Vector2 center, Vector2 size, Vector2 xaxis) {
            var yaxis = new Vector2(-xaxis.y, xaxis.x);
            xaxis *= size.x;
            yaxis *= size.y;

            model = Matrix4x4.zero;
            model[0] = xaxis.x; model[4] = yaxis.x; model[12] = center.x;
            model[1] = xaxis.y; model[5] = yaxis.y; model[13] = center.y;
            model[15] = 1f;

            bounds = QUAD.EncapsulateInTargetSpace(model);
        }

        public IEnumerable<Vector2> Normals() {
            yield return model.MultiplyVector(Vector2.right);
            yield return model.MultiplyVector(Vector2.up);
        }

        public IEnumerable<Vector2> Vertices() {
            foreach (var v in VERTICES)
                yield return model.MultiplyPoint3x4(v);
        }

        public Rect WorldBounds {
            get { return bounds; }
        }

        public Matrix4x4 Model { get { return model; } }
    }
}
