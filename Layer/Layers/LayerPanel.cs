using nobnak.Gist.Wrapper;
using UnityEngine;

namespace nobnak.Gist.Layers {

    [ExecuteInEditMode]
    public class LayerPanel : MonoBehaviour {
        public enum DepthModeEnum { Normalized = 0, Exact }

        public string defaultTextureName = "_MainTex";
        public Camera targetCam;

        public DepthModeEnum depthMode;
        [Header("Normalized Depth")]
    	[Range(0f, 0.99f)]
    	public float normalizedDepth = 0f;
        [Header("Exact Depth")]
        public float depth = 10f;

        public Vector2 size = Vector2.one;
        public Rect viewport = new Rect (0f, 0f, 1f, 1f);

        Block block;

        #region Unity
        void OnEnable() {
            Renderer rend;
            if ((rend = GetComponent<Renderer> ()) != null)
                block = new Block (rend);
        }
        void Update() {
            if (targetCam == null)
                return;

            var min = viewport.min;
            var max = viewport.max;
            var center = viewport.center;
            var depth = Depth ();

            var scale = targetCam.ViewportToWorldPoint (new Vector3 (max.x, max.y, depth))
                - targetCam.ViewportToWorldPoint (new Vector3 (min.x, min.y, depth));
    		scale = targetCam.transform.InverseTransformDirection (scale);
            scale.x /= size.x;
            scale.y /= size.y;
            scale.z = 1f;

            transform.position = targetCam.ViewportToWorldPoint (new Vector3 (center.x, center.y, depth));
            transform.rotation = targetCam.transform.rotation;
    		transform.localScale = scale;
        }
        #endregion

        public virtual void SetTexture(Texture tex) {
            SetTexture(defaultTextureName, tex);
        }
        public virtual void SetTexture(string textureName, Texture tex) {
            if (block != null)
                block.SetTexture (textureName, tex).Apply ();
        }

        float Depth() {
            switch (depthMode) {
            case DepthModeEnum.Normalized:
                return Mathf.Lerp (targetCam.nearClipPlane, targetCam.farClipPlane, normalizedDepth);
            default:
                return depth;
            }
        }
    }
}
