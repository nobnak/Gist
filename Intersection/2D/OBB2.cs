using Gist.Extensions.RectExt;
using nobnak.Gist.Extensions.AABB;
using nobnak.Gist.Primitive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Intersection {

    public class OBB2 : IConvex2Polytope, IConvex2Distance {
        public static readonly Vector2[] VERTICES = new Vector2[] {
            0.5f * new Vector2(-1,-1), 0.5f * new Vector2(-1,1),
            0.5f * Vector2.one, 0.5f * new Vector2(1,-1)
        };
        public static readonly Rect LOCAL_BOUNDS = new Rect(-0.5f, -0.5f, 1f, 1f);
        public static readonly Vector2 LOCAL_MIN = LOCAL_BOUNDS.min;
        public static readonly Vector2 LOCAL_MAX = LOCAL_BOUNDS.max;

        protected DefferedMatrix model = new DefferedMatrix();
        protected Rect worldBounds;

        protected Vector2 xaxis;
        protected Vector2 yaxis;

        public OBB2() { }
        public OBB2(Vector2 center, Vector2 size, Vector2 xaxis) {
            Reset(center, size, xaxis);
        }

        public virtual void Reset(Vector2 center, Vector2 size, Vector2 xaxis) {
            var yaxis = new Vector2(-xaxis.y, xaxis.x);
            this.xaxis = xaxis;
            this.yaxis = yaxis;
            xaxis *= size.x;
            yaxis *= size.y;

            var localToWorld = Matrix4x4.identity;
            localToWorld[0] = xaxis.x; localToWorld[4] = yaxis.x; localToWorld[12] = center.x;
            localToWorld[1] = xaxis.y; localToWorld[5] = yaxis.y; localToWorld[13] = center.y;
            Reset(localToWorld);
        }
        public void Reset(Matrix4x4 localToWorld) {
            model.Reset(localToWorld);
            worldBounds = LOCAL_BOUNDS.EncapsulateInTargetSpace(localToWorld);
        }
        public static Matrix4x4 CalculateModelMatrix(Vector2 center, Vector2 size, Vector2 xaxis) {
            xaxis.Normalize();
            var yaxis = new Vector2(-xaxis.y, xaxis.x);
            xaxis *= size.x;
            yaxis *= size.y;

            var localToWorld = Matrix4x4.identity;
            localToWorld[0] = xaxis.x; localToWorld[4] = yaxis.x; localToWorld[12] = center.x;
            localToWorld[1] = xaxis.y; localToWorld[5] = yaxis.y; localToWorld[13] = center.y;
            return localToWorld;
        }

        #region IConvex2Polytope
        public IEnumerable<Vector2> Normals() {
            yield return xaxis;
            yield return yaxis;
        }

        public IEnumerable<Vector2> Vertices() {
            foreach (var v in VERTICES)
                yield return model.Matrix.MultiplyPoint3x4(v);
        }

        public Rect WorldBounds {
            get { return worldBounds; }
        }

        public Matrix4x4 Model { get { return model.Matrix; } }
        #endregion

        #region IConvex2Distance
        public virtual Vector2 ClosestPoint(Vector2 worldPoint) {
            var localPoint = (Vector2)model.InverseTransformPoint(worldPoint);
            var localClosestOne = LOCAL_BOUNDS.ClosestPoint(localPoint);
            var worldClosestOne = model.TransformPoint(localClosestOne);
            return worldClosestOne;
        }
        public virtual bool Contains(Vector2 worldPoint) {
            var localPoint = (Vector2)model.InverseTransformPoint(worldPoint);
            var result = Contains(LOCAL_MIN, LOCAL_MAX, localPoint);
            return result;
        }
        public static bool Contains(Vector2 min, Vector2 max, Vector2 point) {
            var px = point.x;
            var py = point.y;

            return min.x <= px && px < max.x && min.y <= py && py < max.y;
        }
        #endregion
    }
}
