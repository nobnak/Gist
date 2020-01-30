using nobnak.FieldLayout;
using nobnak.FieldLayout.Extensions;
using nobnak.Gist.Extensions.RandomExt;
using nobnak.Gist.MathAlgorithms.Distribution.Uniform;
using nobnak.Gist.ObjectExt;
using nobnak.Gist.Scoped;
using nobnak.Gist.ThreadSafe;
using nobnak.Gist.Wrapper;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Sampler.Example {

    [ExecuteAlways]
    public class ImageBasedSampler : MonoBehaviour {
        public enum RGBEnum { R = 0, G, B, A }

        public static readonly int ID_RANDOM = Shader.PropertyToID("_Random");

        [SerializeField]
        protected Field field;
        [SerializeField]
        protected GameObject[] fabs = new GameObject[0];

        [SerializeField]
        protected bool debugMode = false;
        [SerializeField]
        protected int seed = 0;
        [SerializeField]
        protected float size = 1f;
        [SerializeField]
        [Range(0f, 4f)]
        protected float sizePerturb = 0f;
        [SerializeField]
        [Range(0, 10000)]
        protected int count = 100;
        [SerializeField]
        protected float resolutionScale = 1f;
        [SerializeField]
        [Range(0f, 1f)]
        protected float cohesive = 0.5f;
        //[SerializeField]
        //protected Vector3 upward = Vector3.back;
        [SerializeField]
        protected Vector3 eularRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField]
        protected Texture2D mask;
        [SerializeField]
        protected RGBEnum maskSelector;

        [SerializeField]
        protected Color debugPosColor = Color.green;

        protected GLFigure fig;
        protected Validator validator = new Validator();
        protected DitheredSampler sampler = new DitheredSampler();
        protected List<Vector3> fpositions = new List<Vector3>();
        protected List<GameObject> flowers = new List<GameObject>();
        protected List<Block> materialProperties = new List<Block>();
        protected List<Vector3> flowerOriginalSizes = new List<Vector3>();

        #region interface
        public void SetMask(Texture2D mask) {
            validator.Invalidate();
            this.mask = mask;
        }

        public int Count
        {
            get { return count; }
            set {
                count = value;
                validator.Invalidate();
            }
        }

        public float ResolutionScale
        {
            get { return resolutionScale; }
            set
            {
                resolutionScale = value;
                validator.Invalidate();
            }
        }
        #endregion

        #region unity
        private void OnEnable() {
            fig = new GLFigure();

            validator.Reset();
            validator.Validation += () => {
                count = Mathf.Max(count, 0);

                if (field != null) {
                    field.eventHolder.EventValidated -= ListenOnFieldValidated;
                    field.eventHolder.EventValidated += ListenOnFieldValidated;
                }

                GeneratePositions();
                ResizeFlowers(fpositions.Count);
            };
        }
        private void OnValidate() {
            validator.Invalidate();
        }
        private void Update() {
            validator.Validate();
        }
        private void OnDisable() {
            if (fig != null) {
                fig.Dispose();
                fig = null;
            }
            fpositions.Clear();
            ResizeFlowers(0);
        }
        private void OnRenderObject() {
            if (!debugMode)
                return;

            if (fig == null)
                return;

            validator.Validate();
            var q = field.transform.rotation;
            var s = 0.1f * Vector2.one;
            var c = debugPosColor;

            for (var i = 0; i < fpositions.Count; i++) {
                var pos = fpositions[i];
                fig.DrawQuad(pos, q, s, c);
            }
        }
        #endregion

        #region member
        void ListenOnFieldValidated(Field field) {
            validator.Invalidate();
        }
        protected virtual void ResizeFlowers(int count) {
            for (var i = flowers.Count - 1; i >= count; i--) {
                var f = flowers[i];
                f.DestroyGo();
                flowers.RemoveAt(i);
                flowerOriginalSizes.RemoveAt(i);
            }
            while (flowers.Count < count) {
                var f = Instantiate(fabs[Random.Range(0, fabs.Length)]);
                f.hideFlags = HideFlags.DontSave;
                f.transform.SetParent(transform, false);
                flowers.Add(f);
                flowerOriginalSizes.Add(f.transform.localScale);
                materialProperties.Add(new Block(f.GetComponents<Renderer>()));
            }
            using (new ScopedRandom(seed + 1)) {
                var rotToLayer = field.Layer.transform.rotation;
                for (var i = 0; i < fpositions.Count; i++) {
                    var pos = fpositions[i];
                    var scale = flowerOriginalSizes[i];
                    var f = flowers[i];
                    var mp = materialProperties[i];
                    var rot = rotToLayer
                        * Quaternion.Euler(eularRotation)
                        * Quaternion.Euler(0f, 360f * Random.value, 0);

                    f.transform.position = pos;
                    f.transform.rotation = rot;
                    f.transform.localScale = scale * (size * (1f + sizePerturb.UUniform()));

                    mp.SetFloat(ID_RANDOM, Random.value).Apply();
                }
            }
        }
        private void GeneratePositions() {
            var unit = field.LocalToLayer.TransformVector(new Vector3(1f, 1f, 0f));
            var aspect = unit / unit.y;
            var res = Mathf.RoundToInt(2f * Mathf.Sqrt(count * resolutionScale));
            var size = new Vector2Int(Mathf.RoundToInt(res * aspect.x), res);
            var duv = 0.2f * new Vector2(1f / size.x, 1f / size.y);

            var colorTex = (ColorTextureData)(mask != null ? mask : Texture2D.blackTexture);
            sampler.Input = new FloatFilter(colorTex, (int)maskSelector);
            sampler.Size = size;
            sampler.Limit = count;

            fpositions.Clear();
            foreach (var s in sampler.Samples) {
                var perturb = new Vector2(duv.x.Perturb(), duv.y.Perturb());
                var pos = field.UvToWorldPos(s + perturb);
                fpositions.Add(pos);
            }

            Debug.Log($"Positions : limit={count} n={fpositions.Count} size={size}");
        }
        #endregion
    }
}
