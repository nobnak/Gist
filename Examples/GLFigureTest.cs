using UnityEngine;
using System.Collections;
using Gist;
using System.Collections.Generic;

namespace Gist {
	[ExecuteInEditMode]
	public class GLFigureTest : MonoBehaviour {
		public Data[] dataset;

		public Transform[] lines;

		GLFigure _fig;

		void OnEnable() {
			_fig = new GLFigure ();
		}
		void OnDisable() {
			if (_fig != null) {
				_fig.Dispose ();
				_fig = null;
			}
		}
		void OnRenderObject() {
			if (dataset == null || lines == null)
				return;
			
			for (var i = 0; i < dataset.Length; i++)
				dataset [i].Draw (_fig);

			if (lines.Length >= 2) {
				_fig.DrawLines (GenerateVertices(), transform, Color.white, GL.LINES);
			}
		}
		IEnumerable<Vector3> GenerateVertices() {
			Transform curr = lines[0];
			for (var i = 1; i < lines.Length; i++) {
				yield return curr.position;
				var next = lines[i];
				yield return next.position;
				curr = next;
			}
		}

		[System.Serializable]
		public class Data {
			public enum ShapeEnum { Circle = 0, Quad, Fan }
			public enum FillEnum { Edge = 0, Fill }

			public ShapeEnum shape;
			public FillEnum fill;
			public Color color;

			public Transform transform;

			public void Draw(GLFigure fig) {
				switch (shape) {
				case ShapeEnum.Circle:
					if (fill == FillEnum.Edge)
						fig.DrawCircle (transform.position, transform.rotation, transform.localScale, color);
					else
						fig.FillCircle (transform.position, transform.rotation, transform.localScale, color);
					break;
				case ShapeEnum.Quad:
					if (fill == FillEnum.Edge)
						fig.DrawQuad (transform.position, transform.rotation, transform.localScale, color);
					else
						fig.FillQuad (transform.position, transform.rotation, transform.localScale, color);
					break;
                case ShapeEnum.Fan:
                    if (fill == FillEnum.Edge)
                        fig.DrawFan (transform.position, transform.rotation, transform.localScale, color, -30f, 30f);
                    else
                        fig.FillFan (transform.position, transform.rotation, transform.localScale, color, -30f, 30f);
                    break;
				}
			}
		}
	}
}