using nobnak.Gist.Extensions.CameraExt;
using nobnak.Gist.GLTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

[ExecuteAlways]
public class DrawLines : MonoBehaviour {

    [SerializeField] protected List<Transform> quads = new();
    [SerializeField] protected Color color = Color.magenta;
    [SerializeField]
    [Range(0f, 0.3f)] protected float lineWidth = 0.1f;

    GLFigure gl;

    private void OnEnable() {
        gl = new();
    }
    private void OnDisable() {
        gl?.Dispose();
    }
    private void OnRenderObject() {
        if (gl == null || quads.Count == 0 || !isActiveAndEnabled)
            return;

        var cam = Camera.current;
        if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView)
            return;

        var segments = new List<(Vector3 p0, Vector3 p1)>();
        foreach (var quad in quads) {
            for (var j = 0; j < GLFigure.QUAD.Length; j++) {
                var p0 = quad.TransformPoint(GLFigure.QUAD[j]);
                var p1 = quad.TransformPoint(GLFigure.QUAD[(j + 1) % GLFigure.QUAD.Length]);
                segments.Add((p0, p1));
            }
        }

        static IEnumerable<Vector3> GetVertices(List<(Vector3 p0, Vector3 p1)> segments) {
            foreach (var (p0, p1) in segments) {
                yield return p0;
                yield return p1;
            }
        }

        var worldCenter = transform.position;
        var width = lineWidth;
        width *= Mathf.Min(cam.GetHandleSize(worldCenter), 0.8f);

        var viewModel = cam.worldToCameraMatrix;
        gl.CurrentColor = color;
        gl.DrawLines(GetVertices(segments), viewModel, width);
    }
}