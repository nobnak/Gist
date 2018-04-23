using UnityEngine;

namespace nobnak.Gist.Resizable {

	[System.Serializable]
    public class TemporalResizableRenderTexture : ResizableRenderTexture {

		public TemporalResizableRenderTexture(FormatRT format) {
			this.format = format;
		}
		public TemporalResizableRenderTexture() : this(new FormatRT()) { }

		#region public
		public override void Release() {
			ReleaseTexture();
			Invalidate();
		}
		#endregion

		#region private
		protected override void CreateTexture(int width, int height) {
            ReleaseTexture();

			if (width < 2 || height < 2) {
				Debug.LogFormat("Texture size too small : {0}x{1}", width, height);
				return;
			}

            tex = format.GetTexture(width, height);
            NotifyAfterCreateTexture ();
        }

        protected override void ReleaseTexture() {
            NotifyBeforeDestroyTexture ();
            RenderTexture.ReleaseTemporary(tex);
            tex = null;
        }
		#endregion
		
	}
}
