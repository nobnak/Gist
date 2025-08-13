using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Intersection {

    public interface IConvex2Distance {

        Vector2 ClosestPoint(Vector2 point);
        bool Contains(Vector2 point);
    }
}
