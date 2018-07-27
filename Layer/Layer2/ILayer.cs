using UnityEngine;
using nobnak.Gist.Primitive;

namespace nobnak.Gist.Layer2 {

    public interface ILayer {

        DefferedMatrix LayerToWorld { get; }
        DefferedMatrix LocalToLayer { get; }
        DefferedMatrix LocalToWorld { get; }

        bool Raycast(Ray ray, out float t);
		Vector3 ProjectOn(Vector3 worldPos, float distance = 0f);

        Validator LayerValidator { get; }
    }
}