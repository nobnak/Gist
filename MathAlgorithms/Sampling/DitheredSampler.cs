using nobnak.Gist.ThreadSafe;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Sampler {

    public class DitheredSampler {

        public UnityEngine.Events.UnityEvent Changed = new UnityEngine.Events.UnityEvent();

        public const float THRESHOLD = 0.5f;
        public static readonly Pattern[] DITHER_JJN = new Pattern[] {
            new Pattern(1, 0, 7), new Pattern(2, 0, 5),
            new Pattern(-2, 1, 3), new Pattern(-1, 1, 5), new Pattern(0, 1, 7), new Pattern(1, 1, 5), new Pattern(2, 1, 3),
            new Pattern(-2, 2, 1), new Pattern(-1, 2, 3), new Pattern(0, 2, 5), new Pattern(1, 2, 3), new Pattern(2, 2, 1)
        };
        public static readonly float DITHER_SUM_INV = 1f / DITHER_JJN.Select(v => v.amount).Sum();

        protected Vector2Int size;
        protected ITextureData<float> inputImage;
        protected int limit = 1000;

        protected Validator validator = new Validator();
        protected List<Vector2> samples = new List<Vector2>();

        protected float[] diffusion;
        protected Vector2 dxdy;

        public DitheredSampler() {
            validator.Validation += () => {
                samples.Clear();

                var w = size.x;
                var h = size.y;

                System.Array.Resize(ref diffusion, w * h);
                for (var y = 0; y < h; y++) {
                    for (var x = 0; x < w; x++) {
                        var uv = GetUv(y, x);
                        var c = inputImage[uv.x, uv.y];
                        diffusion[x + y * w] = c;
                    }
                }

                var sumOfDiffusion = diffusion.Sum();
                var density = (sumOfDiffusion > 0f ? limit / sumOfDiffusion : 0f);
                for (var i = 0; i < diffusion.Length; i++)
                    diffusion[i] *= density;

                for (var y = 0; y < h; y++) {
                    for (var x = 0; x < w; x++) {
                        var e = diffusion[x + y * w];
                        if (e == 0f)
                            continue;

                        if (e > THRESHOLD) {
                            e -= 1f;
                            var uv = GetUv(y, x);
                            samples.Add(uv);
                        }
                        for (var i = 0; i < DITHER_JJN.Length; i++) {
                            var p = DITHER_JJN[i];
                            var offset = p.offset;
                            var x1 = x + offset.x;
                            var y1 = y + offset.y;
                            if (x1 < 0 || w <= x1 || y1 < 0 || h <= y1)
                                continue;
                            diffusion[x1 + y1 * w] += (e * p.amount) * DITHER_SUM_INV;
                        }
                    }
                }

                if (Changed != null)
                    Changed.Invoke();
            };
        }

        #region interface
        public int Count {
            get {
                validator.Validate();
                return samples.Count;
            }
        }
        public List<Vector2> Samples {
            get {
                validator.Validate();
                return samples;
            }
        }
        public Vector2Int Size {
            set {
                this.size = value;
                dxdy = new Vector2(1f / (size.x + 1), 1f / (size.y + 1));
                validator.Invalidate();
            }
        }
        public ITextureData<float> Input {
            set {
                this.inputImage = value;
                validator.Invalidate();
            }
        }
        public int Limit {
            set {
                if (limit != value) {
                    this.limit = value;
                    validator.Invalidate();
                }
            }
        }
        #endregion

        #region member
        private Vector2 GetUv(int y, int x) {
            return new Vector2((x + 0.5f) * dxdy.x, (y + 0.5f) * dxdy.y);
        }
        #endregion

        #region definition
        public class Pattern {
            public Vector2Int offset;
            public float amount;

            public Pattern(Vector2Int offset, float amount) {
                this.offset = offset;
                this.amount = amount;
            }
            public Pattern(int xoffset, int yoffset, float amount)
                : this(new Vector2Int(xoffset, yoffset), amount) { }
        }
        #endregion
    }
}
