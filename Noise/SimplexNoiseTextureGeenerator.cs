using UnityEngine;
using System.Collections;
using Gist.Layers;

namespace Gist {
    [ExecuteInEditMode]
    public class SimplexNoiseTextureGeenerator : MonoBehaviour {
        public const float SEED_SIZE = 100f;

        public TextureEvent OnCreateTexture;

        [SerializeField]
        AbstractLayer layer;
        [SerializeField]
        int width = 256;
        [SerializeField]
        float aspect = 1f;

        [SerializeField]
        float fieldSize = 1f;
        [SerializeField]
        float noiseFreq = 1f;
        [SerializeField]
        float timeScale = 1f;

        public System.Func<float, float, float, float> HeightFunc;

        Texture2D _noiseTex;
        float[] _heightValues;
        Vector3[] _normalValues;
        Color[] _noiseColors;
        Vector3 _seeds;
        Vector2 _texelSize;

        #region Unity
        void OnEnable() {
            _seeds = SEED_SIZE * new Vector3 (Random.value, Random.value, Random.value);
        }
    	void Update () {
            _texelSize.Set(1f / width, 1f / width);

            if (_noiseTex == null || _noiseTex.width != width) {
                ReleaseTex();
                _noiseTex = new Texture2D (width, width, TextureFormat.ARGB32, false, true);
                _noiseTex.wrapMode = TextureWrapMode.Clamp;
                _noiseTex.filterMode = FilterMode.Bilinear;
                _noiseColors = _noiseTex.GetPixels ();
                _normalValues = new Vector3[_noiseColors.Length];
                _heightValues = new float[(width + 1) * (width + 1)];
                OnCreateTexture.Invoke (_noiseTex);
            }

            UpdateNoiseMap ();
    	}
        void OnDisable() {
            ReleaseTex ();
        }
        #endregion

        public void SetAspect(float aspect) {
            this.aspect = aspect;
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
            var uv = layer.ProjectOnNormalized (worldPos);
            return GetYNormalFromUv (uv);
        }

        public Vector3 GetNormal(Vector2 uv) {
            var x = uv.x * width + 0.5f;
            var y = uv.y * width + 0.5f;
            var ix = (int)x;
            var iy = (int)y;
            var dx = x - ix;
            var dy = y - iy;

            ix = ix < 0 ? 0 : (ix >= width ? width - 1 : ix);
            iy = iy < 0 ? 0 : (iy >= width ? width - 1 : iy);
            var jx = ix + 1;
            var jy = iy + 1;
            if (jx >= width)
                jx = width - 1;
            if (jy >= width)
                jy = width - 1;
            
            return (1f - dx) * ((1f - dy) * GetNormal (ix, iy) + dy * GetNormal (ix, jy))
                + dx * ((1f - dy) * GetNormal (jx, iy) + dy * GetNormal (jx, jy));
        }

        public float GetHeight(int x, int y) { return _heightValues[x + y * (width + 1)]; }
        public void SetHeight(int x, int y, float value) { _heightValues[x + y * (width + 1)] = value; }

        public Vector3 GetNormal(int x, int y) { return _normalValues[x + y * width]; }
        public void SetNormal(int x, int y, Vector3 value) { _normalValues[x + y * width] = value; }

        public Color GetNoisePixel(int x, int y) { return _noiseColors[x + y * width]; }
        public void SetNoisePixel(int x, int y, Color value) { _noiseColors[x + y * width] = value; }

        void UpdateNoiseMap() {
            UpdateHeightMap ();
            UpdateNormalMap ();
            UpdateTexture ();

            _noiseTex.SetPixels (_noiseColors);
            _noiseTex.Apply (false);
        }

        void UpdateHeightMap () {
            var px = new Vector2 (noiseFreq * aspect / width, noiseFreq / width);
            var t = Time.timeSinceLevelLoad * timeScale + _seeds.z;
            var H = (HeightFunc == null ? DefaultHeightFunc : HeightFunc);                
            Parallel.For (0, width + 1, y =>  {
                for (var x = 0; x <= width; x++)
                    SetHeight(x, y, H(px.x * (x - 0.5f + _seeds.x), px.y * (y - 0.5f + _seeds.y), t));
            });
        }
        void UpdateNormalMap () {
            var invDx = new Vector2(width / (fieldSize * aspect), width / fieldSize);
            Parallel.For (0, width, y =>  {
                for (var x = 0; x < width; x++) {
                    var h = GetHeight(x, y);
                    var dhdx = (GetHeight(x + 1, y) - h) * invDx.x;
                    var dhdy = (GetHeight(x, y + 1) - h) * invDx.y;
                    SetNormal(x, y, new Vector3 (-dhdx, -dhdy, 1f).normalized);
                }
            });
        }
        void UpdateTexture() {
            Parallel.For (0, width, y =>  {
                for (var x = 0; x < width; x++) {
                    var h = GetHeight(x, y);
                    var n = GetNormal(x, y);
                    SetNoisePixel(x, y, new Color(0.5f * (n.x + 1f), 0.5f * (n.y + 1f), 0.5f * (n.z + 1f), h));
                }
            });
        }

        float DefaultHeightFunc(float x, float y, float z) {
            return (float)SimplexNoise.Noise (x, y, z);
        }
        void ReleaseTex () {
            DestroyImmediate(_noiseTex);
        }

        [System.Serializable]
        public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
    }
}