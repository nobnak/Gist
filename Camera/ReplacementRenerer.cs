using nobnak.Gist;
using nobnak.Gist.Events;
using nobnak.Gist.Resizable;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.Cameras {

    public class ReplacementRenerer : MonoBehaviour {
		public Tuner tuner = new Tuner();

        public Camera refCam;
        public Shader replacementShader;
		public TextureEvent OnUpdateVelocityTex = new TextureEvent();

        ManuallyRenderCamera manualCam;
        LODRenderTexture output;

        #region Unity
        void OnEnable() {
            manualCam = new ManuallyRenderCamera (refCam);
			output = new LODRenderTexture();

			var formatOutput = output.Format;
			formatOutput.depth = 24;
			formatOutput.textureFormat = RenderTextureFormat.ARGBFloat;
			output.Format = formatOutput;

			manualCam.AfterCopyFrom += (Camera obj) => {
				obj.cullingMask = tuner.MaskValue;

				if (tuner.overrideClearFlags) {
					obj.clearFlags = tuner.clearFlags;
					obj.backgroundColor = tuner.backgroundColor;
				}
			};
            output.AfterCreateTexture += (LODRenderTexture obj) => {
                obj.Texture.filterMode = FilterMode.Bilinear;
                obj.Texture.wrapMode = TextureWrapMode.Clamp;
				OnUpdateVelocityTex.Invoke(obj.Texture);
			};
        }
    	void Update () {
			var size = new Vector2Int(refCam.pixelWidth, refCam.pixelHeight);
			output.Lod = tuner.lod;
			output.Size = size;

            manualCam.RenderWithShader (output.Texture, replacementShader, null);
        }
        void OnDestroy() {
            if (output != null) {
                output.Dispose ();
                output = null;
            }
        }
		#endregion

		#region definition
		[System.Serializable]
		public class Tuner {
			public LayerMask[] masks = new LayerMask[0];
			public int lod = 0;

			public bool overrideClearFlags = false;
			public CameraClearFlags clearFlags = CameraClearFlags.SolidColor;
			public Color backgroundColor = Color.clear;

			#region interface
			public int MaskValue {
				get {
					var v = 0;
					foreach (var m in masks)
						v |= m.value;
					return v;
				}
			}
			#endregion
		}
		#endregion
	}
}
