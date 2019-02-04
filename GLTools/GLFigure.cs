using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace nobnak.Gist {
	public class GLFigure : System.IDisposable {

		public const float TWO_PI_RAD = 2f * Mathf.PI;
		public const int SEGMENTS = 36;
        public const float FAN_START_ANGLE = 90f;

        public static readonly Vector3[] QUAD = new Vector3[]{
            new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f,  0.5f, 0f),
            new Vector3( 0.5f,  0.5f, 0f), new Vector3( 0.5f, -0.5f, 0f)
        };

		public Material LineMat { get; set; }
        public GLMaterial DefaultLineMat { get; protected set; }
        public Color CurrentColor { get; set; }

		public Matrix4x4 ProjectionMatrix {
			get { return projectionMatrix; }
			set {
				EnabledCustomProjectionMatrix = true;
				projectionMatrix = value;
			}
		}

		public bool EnabledCustomProjectionMatrix { get; set; }

		protected Matrix4x4 projectionMatrix;

		public GLFigure() {
            DefaultLineMat = new GLMaterial();
        }


		#region IDisposable implementation
		public void Dispose() {
            if (DefaultLineMat != null) {
                DefaultLineMat.Dispose();
                DefaultLineMat = null;
            }
        }
		#endregion

		#region public
		public void DrawCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            DrawCircle(cameraMat * modelMat);
		}
		public void FillCircle(Vector3 center, Quaternion look, Vector2 size, Color color) {
			var scale = new Vector3 (size.x, size.y, 1f);
			var modelMat = Matrix4x4.TRS (center, look, scale);
			var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            FillCircle(cameraMat * modelMat);
        }
        public void DrawFan(Vector3 center, Quaternion look, Vector2 size, Color color, float fromAngle, float toAngle) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            DrawFan(cameraMat * modelMat, fromAngle, toAngle);
        }
        public void FillFan(Vector3 center, Quaternion look, Vector2 size, Color color, float fromAngle, float toAngle) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            FillFan(cameraMat * modelMat, fromAngle, toAngle);
        }
        public void DrawQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            DrawQuad(cameraMat * modelMat);
        }
        public void FillQuad(Vector3 center, Quaternion look, Vector2 size, Color color) {
            var scale = new Vector3 (size.x, size.y, 1f);
            var modelMat = Matrix4x4.TRS (center, look, scale);
            var cameraMat = Camera.current.worldToCameraMatrix;
            CurrentColor = color;
            FillQuad (cameraMat * modelMat);
        }

        [System.Obsolete]
        public void DrawCircle(Matrix4x4 modelViewMat, Color color) {
            CurrentColor = color;
            DrawCircle(modelViewMat);
        }
        [System.Obsolete]
        public void FillCircle(Matrix4x4 modelViewMat, Color color) {
            CurrentColor = color;
            FillCircle(modelViewMat);
        }
        [System.Obsolete]
        public void DrawFan(Matrix4x4 modelViewMat, Color color, float fromAngle, float toAngle) {
            CurrentColor = color;
            DrawFan(modelViewMat, fromAngle, toAngle);
        }
        [System.Obsolete]
        public void FillFan(Matrix4x4 modelViewMat, Color color, float fromAngle, float toAngle) {
            CurrentColor = color;
            FillFan(modelViewMat, fromAngle, toAngle);
        }
        [System.Obsolete]
        public void DrawQuad(Matrix4x4 modelViewMat, Color color) {
            CurrentColor = color;
            DrawQuad(modelViewMat);
        }
        [System.Obsolete]
        public void FillQuad(Matrix4x4 modelViewMat, Color color) {
            CurrentColor = color;
            FillQuad(modelViewMat);
        }
        [System.Obsolete]
        public void DrawLines(IEnumerable<Vector3> vertices, Matrix4x4 modelViewMat, Color color) {
            CurrentColor = color;
            DrawLines(vertices, modelViewMat);
        }
        [System.Obsolete]
        public void DrawLines(IEnumerable<Vector3> vertices, Transform trs, Color color) {
            CurrentColor = color;
            DrawLines(vertices, MakeModelViewMatrix(trs));
        }

        public void DrawCircle (Matrix4x4 modelViewMat) {
            if (!StartDraw (modelViewMat, GL.LINES))
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
        public void FillCircle(Matrix4x4 modelViewMat) {
            if (!StartDraw (modelViewMat, GL.TRIANGLES))
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
        public void DrawFan(Matrix4x4 modelViewMat, float fromAngle, float toAngle) {
            if (!StartDraw (modelViewMat, GL.LINES))
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
        public void FillFan(Matrix4x4 modelViewMat, float fromAngle, float toAngle) {
            if (!StartDraw (modelViewMat, GL.TRIANGLES))
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
        public void DrawQuad(Matrix4x4 modelViewMat) {
            if (!StartDraw (modelViewMat, GL.LINES))
                return;
            try {
				foreach (var v in IterateQuadLines())
					GL.Vertex (v);
            } finally {
                EndDraw ();
            }
        }
        public void FillQuad(Matrix4x4 modelViewMat) {
            if (!StartDraw (modelViewMat, GL.QUADS))
                return;
            try {
                for (var i = 0; i < QUAD.Length; i++)
                    GL.Vertex (QUAD [i]);
            } finally {
                EndDraw ();
            }
        }

		public void DrawQuad(Matrix4x4 modelViewMat, float width) {
			DrawLines(IterateQuadLines(), modelViewMat, width);
		}

		public void DrawLines(IEnumerable<Vector3> vertices, Matrix4x4 modelViewMat) {
			if (!StartDraw (modelViewMat, GL.LINES))
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

		public void DrawLines(IEnumerable<Vector3> vertices, Matrix4x4 modelViewMat, float width) {
			if (!StartDraw(Matrix4x4.identity, GL.TRIANGLE_STRIP))
				return;
			try {
				var half = 0.5f * width;
				var iter = vertices.GetEnumerator();
				while (iter.MoveNext()) {
					var vfrom = modelViewMat.MultiplyPoint3x4(iter.Current);
					if (!iter.MoveNext())
						break;
					var vto = modelViewMat.MultiplyPoint3x4(iter.Current);

					var span = vto - vfrom;
					span.z = 0f;
					var tan = half * span.normalized;
					var nor = new Vector3(-tan.y, tan.x, 0f);

					GL.Vertex(vfrom + nor - tan);
					GL.Vertex(vto + nor + tan);
					GL.Vertex(vfrom - nor - tan);
					GL.Vertex(vto - nor + tan);
				}
			} finally {
				EndDraw();
			}
		}
        public void DrawLines(IEnumerable<Vector3> vertices, Transform trs) {
            Matrix4x4 mv = MakeModelViewMatrix(trs);
            DrawLines(vertices, mv);
        }

        public void DrawLineStrip(IEnumerable<Vector3> vertices, Matrix4x4 modelView) {
            if (!StartDraw(modelView, GL.LINE_STRIP))
                return;
            try {
                foreach (var v in vertices)
                    GL.Vertex(v);
            } finally {
                EndDraw();
            }
        }
        public void DrawLineStrip(IEnumerable<Vector3> vertices, Transform trs) {
            DrawLineStrip(vertices, MakeModelViewMatrix(trs));
        }
        public void ResetProjectionMatrix() {
			EnabledCustomProjectionMatrix = false;
		}
        #endregion

        #region private
        protected static Matrix4x4 MakeModelViewMatrix(Transform trs) {
            var mv = Camera.current.worldToCameraMatrix;
            if (trs != null)
                mv *= trs.localToWorldMatrix;
            return mv;
        }
        protected Vector3 PositionFromAngle(float rad, float size) {
            return new Vector3(0.5f * size * Mathf.Cos (rad), 0.5f * size * Mathf.Sin (rad), 0f);
        }

        protected bool StartDraw(Matrix4x4 modelViewMat, int mode, Material mat, int pass = 0) {
			return StartDraw(modelViewMat, mode, mat, CurrentColor, pass,
				EnabledCustomProjectionMatrix, ProjectionMatrix);
        }
		protected bool StartDraw(Matrix4x4 modelViewMat, int mode, int pass = 0) {
			if (LineMat == null && DefaultLineMat.IsDisposed)
				return false;
			var lineMat = (LineMat != null ? LineMat : DefaultLineMat.LineMat);
			return StartDraw(modelViewMat, mode, lineMat, pass);
		}
		protected IEnumerable<Vector3> IterateQuadLines() {
			var vfrom = QUAD[0];
			for (var i = 0; i < QUAD.Length; i++) {
				var vto = QUAD[(i + 1) % QUAD.Length];
				yield return vfrom;
				yield return vto;
				vfrom = vto;
			}
		}
		#endregion

		#region Static
		static GLFigure _instance;

		public static GLFigure Instance {
			get {
				return (_instance == null ? (_instance = new GLFigure()) : _instance);
			}
		}
		static bool StartDraw(Matrix4x4 modelViewMat, int mode, Material mat, Color color,
			int pass = 0,
			bool enabledProjection = false,
			Matrix4x4 projection = default(Matrix4x4)) {

			if (mat == null)
				return false;

			GL.PushMatrix();
			GL.LoadIdentity();
			GL.MultMatrix(modelViewMat);
			if (enabledProjection)
				GL.LoadProjectionMatrix(projection);
			mat.SetPass(pass);

			GL.Begin(mode);
			//mat.color = color;
			GL.Color(color);
			return true;
		}
		static void EndDraw () {
            GL.End ();
            GL.PopMatrix ();
        }
		#endregion

	}
}