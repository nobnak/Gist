using UnityEngine;
using System.Collections;
using nobnak.Gist.Extensions.ComponentExt;

namespace nobnak.Gist {
    [ExecuteInEditMode]
    public class ScreenNoiseMap : MonoBehaviour {
        public enum DebugModeEnum { None = 0, ShowNormal, ShowHeight }
        public const string KW_OUTPUT_NORMAL = "OUTPUT_NORMAL";
        public const string KW_OUTPUT_HEIGHT = "OUTPUT_HEIGHT";
        public const float SEED_SIZE = 100f;

        public DebugModeEnum debugMode;
        public Material debugMat;
        public TextureEvent OnCreateTexture;
        public Camera targetCam;
        public int lod = 2;
        public float fieldSize = 1f;
        public float noiseFreq = 1f;
        public float timeScale = 1f;

        public System.Func<float, float, float, float> HeightFunc;

        Texture2D _noiseTex;
        float[] _heightValues;
        Vector3[] _normalValues;
        Color[] _noiseColors;
        Vector3 _seeds;
        int _width, _height;
        Vector2 _texelSize;

        void OnEnable() {
            if (targetCam == null)
                targetCam = Camera.main;
            _seeds = SEED_SIZE * new Vector3 (Random.value, Random.value, Random.value);
        }
    	void Update () {
            _width = targetCam.pixelWidth >> lod;
            _height = targetCam.pixelHeight >> lod;
            _texelSize.Set(1f / _width, 1f / _height);

            if (_noiseTex == null || _noiseTex.width != _width || _noiseTex.height != _height) {
                ReleaseTex();
                _noiseTex = new Texture2D (_width, _height, TextureFormat.ARGB32, false);
                _noiseTex.wrapMode = TextureWrapMode.Clamp;
                _noiseTex.filterMode = FilterMode.Bilinear;
                _noiseColors = _noiseTex.GetPixels ();
                _normalValues = new Vector3[_noiseColors.Length];
                _heightValues = new float[(_width + 1) * (_height + 1)];
                OnCreateTexture.Invoke (_noiseTex);
            }

            UpdateNoiseMap ();
    	}
        void OnRenderObject() {
            if (!this.IsVisibleLayer())
                return;

            if (debugMode != DebugModeEnum.None) {
                if (debugMat != null) {
                    debugMat.shaderKeywords = null;
                    debugMat.EnableKeyword (debugMode == DebugModeEnum.ShowHeight ? KW_OUTPUT_HEIGHT : KW_OUTPUT_NORMAL);
                }
                GL.PushMatrix ();
                GL.LoadOrtho ();
                Graphics.DrawTexture (new Rect (0f, 0f, 1f, 1f), _noiseTex, debugMat);
                GL.PopMatrix ();
            }
        }
        void OnDisable() {
            ReleaseTex ();
        }

		public void SetTargetCamera(Camera cam) {
			targetCam = cam;
		}
        public Vector3 GetZNormalFromUv(Vector2 uv) {
            var n = GetNormal (uv);
            return n.normalized;
        }
        public Vector3 GetYNormalFromUv(Vector2 uv) {
            var n = GetNormal (uv);
            n = new Vector3 (n.x, n.z, n.y);
            return n.normalized;
        }
        public Vector3 GetYNormalFromWorldPos(Vector3 worldPos) {
            var uv = targetCam.WorldToViewportPoint(worldPos);
            return GetYNormalFromUv (uv);
        }

        public Vector3 GetNormal(Vector2 uv) {
            var x = uv.x * _width + 0.5f;
            var y = uv.y * _height + 0.5f;
            var ix = (int)x;
            var iy = (int)y;
            var dx = x - ix;
            var dy = y - iy;

            ix = ix < 0 ? 0 : (ix >= _width ? _width - 1 : ix);
            iy = iy < 0 ? 0 : (iy >= _height ? _height - 1 : iy);
            var jx = ix + 1;
            var jy = iy + 1;
            if (jx >= _width)
                jx = _width - 1;
            if (jy >= _height)
                jy = _height - 1;

            return (1f - dx) * ((1f - dy) * GetNormal (ix, iy) + dy * GetNormal (ix, jy))
                + dx * ((1f - dy) * GetNormal (jx, iy) + dy * GetNormal (jx, jy));
        }

        public float GetHeight(int x, int y) { return _heightValues[x + y * (_width + 1)]; }
        public void SetHeight(int x, int y, float value) { _heightValues[x + y * (_width + 1)] = value; }

        public Vector3 GetNormal(int x, int y) { return _normalValues[x + y * _width]; }
        public void SetNormal(int x, int y, Vector3 value) { _normalValues[x + y * _width] = value; }

        public Color GetNoisePixel(int x, int y) { return _noiseColors[x + y * _width]; }
        public void SetNoisePixel(int x, int y, Color value) { _noiseColors[x + y * _width] = value; }

        void UpdateNoiseMap() {
            UpdateHeightMap ();
            UpdateNormalMap ();
            UpdateTexture ();

            _noiseTex.SetPixels (_noiseColors);
            _noiseTex.Apply (false);
        }

        void UpdateHeightMap () {
            var px = (float)noiseFreq / _height;
            var t = Time.timeSinceLevelLoad * timeScale + _seeds.z;
            var H = (HeightFunc == null ? DefaultHeightFunc : HeightFunc);
			var arg = new DataForUpdateHeightMap(px, t, H);
			Parallel.For (0, _height + 1, UpdateHeightMapEach, arg);
        }
		protected void UpdateHeightMapEach(int y, DataForUpdateHeightMap arg) {
			for (var x = 0; x <= _width; x++)
				SetHeight(x, y, arg.H(arg.px * (x - 0.5f + _seeds.x), arg.px * (y - 0.5f + _seeds.y), arg.t));
		}


		void UpdateNormalMap () {
            var idx = (float)_height / fieldSize;
            Parallel.For (0, _height, UpdateNormalMapEach, idx);
        }
		protected void UpdateNormalMapEach(int y, float idx) {
			for (var x = 0; x < _width; x++) {
				var h = GetHeight(x, y);
				var dhdx = (GetHeight(x + 1, y) - h) * idx;
				var dhdy = (GetHeight(x, y + 1) - h) * idx;
				SetNormal(x, y, new Vector3(-dhdx, -dhdy, 1f).normalized);
			}
		}

        void UpdateTexture() {
            Parallel.For (0, _height, UpdateTextureEach, -1);
        }
		protected void UpdateTextureEach(int y, int arg) {
			for (var x = 0; x < _width; x++) {
				var h = GetHeight(x, y);
				var n = GetNormal(x, y);
				SetNoisePixel(x, y, new Color(0.5f * (n.x + 1f), 0.5f * (n.y + 1f), 0.5f * (n.z + 1f), h));
			}
		}

        float DefaultHeightFunc(float x, float y, float z) {
            return (float)SimplexNoise.Noise (x, y, z);
        }
        void ReleaseTex () {
            DestroyImmediate(_noiseTex);
        }

		#region classes
		[System.Serializable]
		public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> { }
		public struct DataForUpdateHeightMap {
			public readonly float px;
			public readonly float t;
			public readonly System.Func<float, float, float, float> H;

			public DataForUpdateHeightMap(float px, float t, System.Func<float, float, float, float> H) {
				this.px = px;
				this.t = t;
				this.H = H;
			}
		}
		#endregion
	}
}