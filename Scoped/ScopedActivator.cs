using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Scoped {

    public class ScopedRenderTextureActivator : System.IDisposable {
        protected RenderTexture prev;

        public ScopedRenderTextureActivator(RenderTexture next) {
            prev = RenderTexture.active;
            RenderTexture.active = next;
        }

        #region IDisposable implementation
        public void Dispose () {
            RenderTexture.active = prev;
        }
        #endregion
    }
}
