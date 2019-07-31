using nobnak.Gist.Scoped;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Cameras {

	[ExecuteAlways]
	public class SSTextureViewer : MonoBehaviour {

		[SerializeField]
		protected ViewSettings settings = new ViewSettings();
		[SerializeField]
		protected List<Texture> textures = new List<Texture>();

		#region interface
		public List<Texture> Textures { get => textures; }
		public ViewSettings CurrentSettings { get; protected set; }

		public void SetTexture(Texture tex, int index) {
			if (index < 0)
				return;
			while (textures.Count <= index)
				textures.Add(tex);
			textures[index] = tex;
		}
		public void SetTexture(Texture tex) {
			SetTexture(tex, settings.defaultIndex);
		}
		#endregion

		#region Unity
		private void OnRenderImage(RenderTexture source, RenderTexture destination) {
			Graphics.Blit(source, destination);

			if (settings.index < 0 || settings.index >= textures.Count)
				return;
			var c = Camera.current;
			var w = c.pixelWidth;
			var h = c.pixelHeight;
			using (new RenderTextureActivator(destination)) {
				GL.PushMatrix();
				GL.LoadPixelMatrix(0f, w, h, 0f);

				var tex = textures[settings.index];
				var size = new Rect(0f, 0f, w, h);
				if (tex != null)
					Graphics.DrawTexture(size, tex);

				GL.PopMatrix();
			}
		}
		#endregion

		#region classes
		public enum ViewMode { Full = 0, PIP }
		[System.Serializable]
		public class ViewSettings {
			public ViewMode viewmode;
			public int index;
			public int defaultIndex;
		}
		#endregion
	}
}
