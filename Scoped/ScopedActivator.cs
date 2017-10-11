using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Scoped {

    public class ScopedRenderTextureActivator : ScopedActivator {
        protected RenderTexture prev;

        public ScopedRenderTextureActivator(RenderTexture next) {
            prev = RenderTexture.active;
            RenderTexture.active = next;
        }

        #region IDisposable implementation
        public override void Dispose () {
            RenderTexture.active = prev;
        }
        #endregion
    }

    public abstract class ScopedActivator : System.IDisposable {
        #region IDisposable implementation
        public abstract void Dispose ();
        #endregion
        
    }
}
