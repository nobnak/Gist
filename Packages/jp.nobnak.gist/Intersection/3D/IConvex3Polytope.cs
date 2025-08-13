using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.Primitive;

namespace nobnak.Gist.Intersection {

    public interface IConvex3Polytope {

        IEnumerable<Vector3> Normals();
        IEnumerable<Vector3> Edges();
        IEnumerable<Vector3> Vertices();

		FastBounds LocalBounds();
		FastBounds WorldBounds();

		IConvex3Polytope DrawGizmos();
    }

	public static class ConvexPolyhedronSettings {
        public static Color GizmoAABBColor = new Color (1f, 1f, 1f, 0.1f);
		public static Color GizmoLineColor = new Color (0.6f, 0.8f, 0.2f);
		public static Color GizmoSurfaceColor = new Color(0.6f, 0.8f, 0.2f, 0f);

        public static float GizmoVertexSize = 0.2f;
	}
}
