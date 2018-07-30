using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Scoped {

    public class RenderTextureActivator : Scoped<RenderTexture> {
		protected RenderTexture prev;

        public RenderTextureActivator(RenderTexture next) :base(next) {
            prev = RenderTexture.active;
            RenderTexture.active = next;
        }

		protected override void Disposer(RenderTexture data) {
			RenderTexture.active = prev;
		}
	}
}
