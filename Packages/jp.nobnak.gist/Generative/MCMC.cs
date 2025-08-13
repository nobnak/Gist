using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.MathAlgorithms.Distribution;

namespace nobnak.Gist {

    public class MCMC {
        public const float EPSILON = 1e-6f;

        protected System.Func<Vector2, float> heightFunc;
        protected Vector2 distunit;
        protected float sigma;
        protected float cutoff;
		protected bool noisy;

        protected Vector2 currUV;
        protected float currValue;

        public MCMC(
			System.Func<Vector2, float> HeightFunc, 
			Vector2 distunit, 
			float sigma, 
			float cutoff,
			bool noisy = false) {

            this.heightFunc = HeightFunc;
            this.distunit = distunit;
            this.sigma = sigma;
            this.cutoff = Mathf.Max (cutoff, EPSILON);
			this.noisy = noisy;
		}

        public IEnumerable<Vector2> Sequence(int nInitialize, int limit, int skip = 0) {
            currUV = new Vector2(Random.value, Random.value);
            currValue = Mathf.Max (heightFunc (currUV), cutoff);

            for (var i = 0; i < nInitialize; i++)
                Next();

            for (var i = 0; i < limit; i++) {
                for (var j = 0; j < skip; j++)
                    Next ();
				if (cutoff < currValue) {
					yield return noisy ? (currUV + 0.1f * RandomStep()) : currUV;
				}
                Next ();
            }
        }

		public Vector2 RandomStep() {
#if RANDOM_UNIFORM
			return new Vector2 (
                sigma * dist.x * Random.Range (-1f, 1f),
                sigma * dist.y * Random.Range (-1f, 1f));
#else
			var g = Gaussian.BoxMuller();
			return new Vector2(sigma * distunit.x * g.x, sigma * distunit.y * g.y);
#endif
		}
		public void Next() {
            var next = RandomStep() + currUV;
            next = Repeat(next);

            var nextValue = Mathf.Max (heightFunc (next), cutoff);
            if (Mathf.Min(1f, nextValue / currValue) >= Random.value) {
                currUV = next;
                currValue = nextValue;
            }
        }
		public Vector2 Repeat(Vector2 uv) {
            var ix = Mathf.FloorToInt (uv.x);
            var iy = Mathf.FloorToInt (uv.y);
            return new Vector2 (uv.x - ix, uv.y - iy);
        }
		public Vector2 Reflect(Vector2 uv) {
            if (uv.x < 0f)
                uv.x = -uv.x;
            else if (uv.x > 1f)
                uv.x = 1f - uv.x;
            
            if (uv.y < 0f)
                uv.y = -uv.y;
            else if (uv.y > 1f)
                uv.y = 1f - uv.y;
            
            return uv;
        }
    }
}
