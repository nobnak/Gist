using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gist.Events;

namespace Gist {

    public interface IGenerativeTexture<T> {
        int Width { get; }
        int Height { get; }

        T GetTextureValue(int ix, int iy);
        T GetTextureValue (Vector2 uv);
    }
}
