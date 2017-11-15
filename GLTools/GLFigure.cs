using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist {
	public class GLFigure : System.IDisposable {

		public const float TWO_PI_RAD = 2f * Mathf.PI;
		public const int SEGMENTS = 36;
        public const float FAN_START_ANGLE = 90f;

        public static readonly Vector3[] QUAD = new Vector3[]{
            new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f), new Vector3( 0.5f, -0.5f, 0f)            
        };

        GLMaterial glmat;

		public GLFigure() {
            glmat = new GLMaterial();
        }

        #region Static
        static GLFigure _instance;

        public static GLFigure Instance {
            get {
                return (_instance == null ? (_instance = new GLFigure()) : _instance);
            }
        }
        #endregion
        #region IDisposable implementation
        public void Dispose() {
            if (glmat != null) {
                glmat.Dispose();
                glmat = null;
            }
        }
        #endregion

        public void DrawCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
			DrawCircle (cameraMat * modelMat, color);
		}
		public void FillCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
			FillCircle (cameraMat * modelMat, color);
        }
        public void DrawFan(Vector3 center, Quaternion look, Vector2 size, Color color, float fromAngle, float toAngle) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            DrawFan (cameraMat * modelMat, color, fromAngle, toAngle);
        }
        public void FillFan(Vector3 center, Quaternion look, Vector2 size, Color color, float fromAngle, float toAngle) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            FillFan (cameraMat * modelMat, color, fromAngle, toAngle);
        }
        public void DrawQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            DrawQuad (cameraMat * modelMat, color);
        }
        public void FillQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            FillQuad (cameraMat * modelMat, color);
        }

		public void DrawCircle (Matrix4x4 modelViewMat, Color color) {
            if (!StartDraw (modelViewMat, color, GL.LINES))
                return;
            try {
    			var dr = TWO_PI_RAD / SEGMENTS;
    			var v = new Vector3 (0.5f, 0f, 0f);
    			for (var i = 0; i <= SEGMENTS; i++) {
    				GL.Vertex (v);
                    v = PositionFromAngle((i + 1) * dr, 1f);
    				GL.Vertex (v);
                }
            } finally {
                EndDraw ();
            }
		}
        public void FillCircle(Matrix4x4 modelViewMat, Color color) {
            if (!StartDraw (modelViewMat, color, GL.TRIANGLES))
                return;
            try {
    			var dr = TWO_PI_RAD / SEGMENTS;
                var v = PositionFromAngle (0f, 1f);
    			for (var i = 0; i < SEGMENTS; i++) {
    				GL.Vertex (v);
    				GL.Vertex (Vector3.zero);
                    v = PositionFromAngle((i + 1) * dr, 1f);
    				GL.Vertex (v);
                }
            } finally {
                EndDraw ();
            }
		}

        public void DrawFan(Matrix4x4 modelViewMat, Color color, float fromAngle, float toAngle) {
            if (!StartDraw (modelViewMat, color, GL.LINES))
                return;

            try {
                var radFrom = (fromAngle + FAN_START_ANGLE) * Mathf.Deg2Rad;
                var radTo = (toAngle + FAN_START_ANGLE) * Mathf.Deg2Rad;
                var dr = (radTo - radFrom) / SEGMENTS;
                var v = PositionFromAngle (radFrom, 2f);
                GL.Vertex (Vector3.zero);
                GL.Vertex (v);
                for (var i = 0; i <= SEGMENTS; i++) {
                    GL.Vertex (v);
                    v = PositionFromAngle ((i + 1) * dr + radFrom, 2f);
                    GL.Vertex (v);
                }
                GL.Vertex (Vector3.zero);
                GL.Vertex (v);
            } finally {
                EndDraw ();
            }
        }
        public void FillFan(Matrix4x4 modelViewMat, Color color, float fromAngle, float toAngle) {
            if (!StartDraw (modelViewMat, color, GL.TRIANGLES))
                return;
            try {
                var radFrom = (fromAngle + FAN_START_ANGLE) * Mathf.Deg2Rad;
                var radTo = (toAngle + FAN_START_ANGLE) * Mathf.Deg2Rad;
                var dr = (radTo - radFrom) / SEGMENTS;
                var v = PositionFromAngle (radFrom, 2f);
                for (var i = 0; i < SEGMENTS; i++) {
                    GL.Vertex (v);
                    GL.Vertex (Vector3.zero);
                    v = PositionFromAngle ((i + 1) * dr + radFrom, 2f);
                    GL.Vertex (v);
                }
            } finally { 
                EndDraw ();
            }
        }

        public void DrawQuad(Matrix4x4 modelViewMat, Color color) {
            if (!StartDraw (modelViewMat, color, GL.LINES))
                return;
            try {
                var v = QUAD [0];
                for (var i = 0; i < QUAD.Length; i++) {
                    GL.Vertex (v);
                    v = QUAD [(i + 1) % QUAD.Length];
                    GL.Vertex (v);
                }
            } finally {
                EndDraw ();
            }
        }
        public void FillQuad(Matrix4x4 modelViewMat, Color color) {
            if (!StartDraw (modelViewMat, color, GL.QUADS))
                return;
            try {
                for (var i = 0; i < QUAD.Length; i++)
                    GL.Vertex (QUAD [i]);
            } finally {
                EndDraw ();
            }
        }
		public void DrawLines(IEnumerable<Vector3> vertices, Transform trs, Color color) {
			DrawLines (vertices, Camera.current.worldToCameraMatrix * trs.localToWorldMatrix, color);
		}
        public void DrawLines(IEnumerable<Vector3> vertices, Matrix4x4 modelViewMat, Color color) {
            if (!StartDraw (modelViewMat, color, GL.LINES))
                return;
            try {
    			var iter = vertices.GetEnumerator ();
    			while (iter.MoveNext ()) {
    				var vfrom = iter.Current;
    				if (!iter.MoveNext ())
    					break;
    				var vto = iter.Current;
    				GL.Vertex (vfrom);
    				GL.Vertex (vto);
    			}
            } finally {
                EndDraw ();
            }
		}

        Vector3 PositionFromAngle(float rad, float size) {
            return new Vector3(0.5f * size * Mathf.Cos (rad), 0.5f * size * Mathf.Sin (rad), 0f);            
        }

        bool StartDraw (Matrix4x4 modelViewMat, Color color, int mode) {
            if (glmat == null)
                return false;
            GL.PushMatrix ();
            GL.LoadIdentity ();
            GL.MultMatrix (modelViewMat);
            GL.Begin (mode);
            glmat.Color(color);
            return true;
        }

        static void EndDraw () {
            GL.End ();
            GL.PopMatrix ();
        }
	}
}