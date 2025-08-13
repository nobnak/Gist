using UnityEngine;
using System.Collections;
using nobnak.Gist.Extensions.AABB;
using System.Collections.Generic;
using nobnak.Gist.Primitive;

namespace nobnak.Gist.Intersection {

    public class OBB3 : IConvex3Polytope {
        public FastBounds localBounds;
        public FastBounds worldBounds;

        public Matrix4x4 modelMatrix;
        public Matrix4x4 modelITMatrix;

        public OBB3(FastBounds localBounds, Matrix4x4 modelMatrix) {
            this.localBounds = localBounds;

            this.modelMatrix = modelMatrix;
            this.modelITMatrix = modelMatrix.inverse.transpose;

            this.worldBounds = localBounds.EncapsulateInTargetSpace (modelMatrix);
		}
		public OBB3(Bounds localBounds, Matrix4x4 modelMatrix) : this((FastBounds)localBounds, modelMatrix) { }

			public Matrix4x4 ModelMatrix() {
            return modelMatrix;
        }
        #region IConvexPolyhedron implementation
        public IEnumerable<Vector3> Normals () {
            yield return modelITMatrix.MultiplyVector(Vector3.right);
            yield return modelITMatrix.MultiplyVector (Vector3.up);
            yield return modelITMatrix.MultiplyVector (Vector3.forward);
        }
        public IEnumerable<Vector3> Edges() {
            return Normals ();
        }
        public IEnumerable<Vector3> Vertices () {
            var extent = localBounds.Extents;
            var center = modelMatrix.MultiplyPoint (localBounds.Center);
            var x = modelMatrix.MultiplyVector(new Vector3(extent.x, 0f, 0f));
            var y = modelMatrix.MultiplyVector (new Vector3 (0f, extent.y, 0f));
            var z = modelMatrix.MultiplyVector(new Vector3(0f, 0f, extent.z));

            for (var i = 0; i < 8; i++)
                yield return center + ((i & 1) != 0 ? x : -x) + ((i & 2) != 0 ? y : -y) + ((i & 4) != 0 ? z : -z);
        }
        public FastBounds LocalBounds() {
            return localBounds;
        }
        public FastBounds WorldBounds() {
            return worldBounds;
        }
        public IConvex3Polytope DrawGizmos() {
            var aabb = WorldBounds ();

            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = ConvexPolyhedronSettings.GizmoAABBColor;
            Gizmos.DrawWireCube (aabb.Center, aabb.Size);

			Gizmos.color = ConvexPolyhedronSettings.GizmoLineColor;
            foreach (var v in Vertices())
                Gizmos.DrawSphere (v, ConvexPolyhedronSettings.GizmoVertexSize);

            var max = aabb.Max;
            var normalLength = 1f;
            Gizmos.color = Color.red;
            foreach (var n in Normals())
                Gizmos.DrawLine(max, max + normalLength * n.normalized);

            Gizmos.matrix = modelMatrix;

            Gizmos.color = ConvexPolyhedronSettings.GizmoLineColor;
            Gizmos.DrawWireCube (localBounds.Center, localBounds.Size);

            Gizmos.matrix = Matrix4x4.identity;

            return this;
        }
        #endregion

        #region Static
		public static OBB3 Create(Transform tr, FastBounds localBounds) {
			return new OBB3(localBounds, tr.localToWorldMatrix);
		}
		#endregion
	}
}
