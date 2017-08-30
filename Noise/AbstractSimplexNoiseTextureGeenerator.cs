using UnityEngine;
using System.Collections;
using Gist.Layers;
using Gist.Events;

namespace Gist {
    [ExecuteInEditMode]
    public abstract class AbstractSimplexNoiseTextureGeenerator : MonoBehaviour {
        public const float SEED_SIZE = 100f;

        public TextureEvent OnCreateTexture;

        [SerializeField]
        protected int width = 256;
        [SerializeField]
        protected float aspect = 1f;

        [SerializeField]
        protected float fieldSize = 1f;
        [SerializeField]
        protected float noiseFreq = 1f;
        [SerializeField]
        protected float timeScale = 1f;

        public System.Func<float, float, float, float> HeightFunc;

        protected Texture2D _noiseTex;
        protected float[] _heightValues;
        protected Vector3[] _normalValues;
        protected Color[] _noiseColors;
        protected Vector3 _seeds;
        protected Vector2 _texelSize;

        #region Unity
        protected virtual void OnEnable() {
            _seeds = SEED_SIZE * new Vector3 (Random.value, Random.value, Random.value);
        }
        protected virtual void Update () {
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
        protected virtual void OnDisable() {
            ReleaseTex ();
        }
        #endregion

        protected abstract Vector2 WorldToViewportPoint (Vector3 worldPos);
        protected abstract Vector3 TransformDirection(Vector3 localDir);

        public virtual void SetAspect(float aspect) {
            this.aspect = aspect;
        }

        public virtual Vector3 GetLocalNormalFromWorldPos(Vector3 worldPos) {
            var uv = WorldToViewportPoint (worldPos);
            return GetLocalNormal (uv);
        }
        public virtual Vector3 GetNormalFromWorldPos(Vector3 worldPos) {
            return TransformDirection(GetLocalNormalFromWorldPos(worldPos));
        }

        public virtual Vector3 GetWorldNormal(Vector2 uv) {
            var n = GetLocalNormal (uv);
            return TransformDirection(n);
        }
        public virtual Vector3 GetLocalNormal(Vector2 uv) {
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
            
            return (1f - dx) * ((1f - dy) * GetLocalNormal (ix, iy) + dy * GetLocalNormal (ix, jy))
                + dx * ((1f - dy) * GetLocalNormal (jx, iy) + dy * GetLocalNormal (jx, jy));
        }

        public virtual float GetHeight(int x, int y) { return _heightValues[x + y * (width + 1)]; }
        public virtual void SetHeight(int x, int y, float value) { _heightValues[x + y * (width + 1)] = value; }

        public virtual Vector3 GetLocalNormal(int x, int y) { return _normalValues[x + y * width]; }
        public virtual void SetNormal(int x, int y, Vector3 value) { _normalValues[x + y * width] = value; }

        public virtual Color GetNoisePixel(int x, int y) { return _noiseColors[x + y * width]; }
        public virtual void SetNoisePixel(int x, int y, Color value) { _noiseColors[x + y * width] = value; }

        protected virtual void UpdateNoiseMap() {
            UpdateHeightMap ();
            UpdateNormalMap ();
            UpdateTexture ();

            _noiseTex.SetPixels (_noiseColors);
            _noiseTex.Apply (false);
        }

        protected virtual void UpdateHeightMap () {
            var px = new Vector2 (noiseFreq * aspect / width, noiseFreq / width);
            var t = Time.timeSinceLevelLoad * timeScale + _seeds.z;
            var H = (HeightFunc == null ? DefaultHeightFunc : HeightFunc);                
            Parallel.For (0, width + 1, y =>  {
                for (var x = 0; x <= width; x++)
                    SetHeight(x, y, H(px.x * (x - 0.5f + _seeds.x), px.y * (y - 0.5f + _seeds.y), t));
            });
        }
        protected virtual void UpdateNormalMap () {
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
                    var n = GetLocalNormal(x, y);
                    SetNoisePixel(x, y, new Color(0.5f * (n.x + 1f), 0.5f * (n.y + 1f), 0.5f * (n.z + 1f), h));
                }
            });
        }

        protected virtual float DefaultHeightFunc(float x, float y, float z) {
            return (float)SimplexNoise.Noise (x, y, z);
        }
        protected virtual void ReleaseTex () {
            DestroyImmediate(_noiseTex);
        }
    }
}