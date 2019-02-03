using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Extensions.GeometryExt {

	public static class GeometryExtension {
		public const float DEF_EPS = 0.001f;

		public static IList<int> RenameMapForMergedIndices(this Mesh mesh, float eps = DEF_EPS) {
			var size = mesh.bounds.size;
			var minlength = Mathf.Min(Mathf.Min(size.x, size.y), size.z);
			eps *= minlength;

			var vertices = mesh.vertices;
			var triangles = mesh.triangles;

			var renameMap = Enumerable.Range(0, vertices.Length).ToArray();
			for (var i = 0; i < vertices.Length; i++) {
				var vi = vertices[i];
				var minsq = float.MaxValue;
				for (var j = 0; j < i; j++) {
					var vj = vertices[j];
					var tmpsq = (vi - vj).sqrMagnitude;
					if (tmpsq < minsq) {
						minsq = tmpsq;
						if (tmpsq < (eps * eps))
							renameMap[i] = j;
					}
				}
			}

			return renameMap;
		}

		public static IList<int> MergeIndices(this Mesh mesh, float eps = DEF_EPS) {
			var triangles = mesh.triangles;
			var rem = mesh.RenameMapForMergedIndices(eps);
			for (var i = 0; i < triangles.Length; i++)
				triangles[i] = rem[triangles[i]];
			return triangles;
		}

		public static int[,] CountEdgesOnTriangles(this IList<int> triangles, int vertexCount) {
			var counter = new int[vertexCount, vertexCount];
			for (var t = 0; t < triangles.Count; t += 3) {
				for (var o = 0; o < 3; o++) {
					var i0 = triangles[t + o];
					var i1 = triangles[t + (o + 1) % 3];
					counter[i0, i1]++;
				}
			}
			return counter;
		}

		public static bool Raycast(this Ray ray,
			Vector3 center, Vector3 forward,
			out float distance, float eps = DEF_EPS) {

			distance = default(float);

			var det = Vector3.Dot((Vector3)forward, ray.direction);
			if (-eps < det && det < eps)
				return false;

			distance = Vector3.Dot((Vector3)forward, (Vector3)(center - ray.origin)) / det;
			return true;
		}
		public static bool Raycast(this Transform tr,
			Ray ray, out float distance, float eps = DEF_EPS) {

			distance = default(float);
			return ray.Raycast(tr.position, tr.forward, out distance, eps);
		}
		public static bool Raycast(this Component c,
			Ray ray, out float distance, float eps = DEF_EPS) {
			return c.transform.Raycast(ray, out distance, eps);
		}
		public static IEnumerable<T> VertexToEdges<T>(this IEnumerable<T> vertices, bool closed = true) {
			var iter = vertices.GetEnumerator();
			if (!iter.MoveNext())
				yield break;

			var first = iter.Current;
			var prev = first;
			while (iter.MoveNext()) {
				yield return prev;
				prev = iter.Current;
				yield return prev;
			}

			if (closed) {
				yield return prev;
				yield return first;
			}
		}

		public static bool TryEstimateDepthOfParallelorism(
			Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, out Vector4 z, float z0 = 1f) {

			var hmat = Matrix4x4.identity;
			hmat[0] = p1.x; hmat[4] = -p2.x; hmat[8] = p3.x;
			hmat[1] = p1.y; hmat[5] = -p2.y; hmat[9] = p3.y;
			hmat[2] = 1f; hmat[6] = -1f; hmat[10] = 1f;
			var hinv = hmat.inverse;

			var h0 = new Vector3(p0.x, p0.y, z0);
			var z1 = hinv.MultiplyPoint(h0);
			z = new Vector4(z0, z1.x, z1.y, z1.z);

			var det = hmat.determinant;
			return det <= -Mathf.Epsilon || Mathf.Epsilon < det;
		}
		public static bool TryBuildParallelorism(
			Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3,
			out Vector3 h0, out Vector3 h1, out Vector3 h2, out Vector3 h3
			) {
			Vector4 z;

			var z0 = 1f;
			var res = TryEstimateDepthOfParallelorism(p0, p1, p2, p3, out z, z0);
			h0 = new Vector3(p0.x, p0.y, z0);
			h1 = z.x * new Vector3(p1.x, p1.y, 1f);
			h2 = z.y * new Vector3(p2.x, p2.y, 1f);
			h3 = z.z * new Vector3(p3.x, p3.y, 1f);

			return res;
		}
		public static bool TryBuildParallelorism(this Vector2[] input, Vector3[] output) {
			if (input == null || input.Length != 4 || output == null || output.Length != 4)
				return false;
			Vector4 w;
			var res = TryEstimateDepthOfParallelorism(
				input[0], input[1], input[2], input[3], out w);
			for (var i = 0; i < 4; i++) {
				var v = input[i];
				output[i] = new Vector3(v.x, v.y, w[i]);
			}
			return res;
		}
	}
}