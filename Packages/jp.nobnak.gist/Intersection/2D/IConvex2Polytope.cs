using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Intersection {

    public interface IConvex2Polytope {

        IEnumerable<Vector2> Normals();
        IEnumerable<Vector2> Vertices();

        Rect WorldBounds { get; }
        Matrix4x4 Model { get; }
    }
}
