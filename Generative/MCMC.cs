using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist {

    public class MCMC {
        public const float EPSILON = 1e-6f;

        protected System.Func<Vector2, float> Func;
        protected Vector2 dist;
        protected float sigma;
        protected float cutoff;

        protected Vector2 currUV;
        protected float currValue;

        public MCMC(System.Func<Vector2, float> Func, Vector2 dist, float sigma, float cutoff) {
            this.Func = Func;
            this.dist = dist;
            this.sigma = sigma;
            this.cutoff = Mathf.Max (cutoff, EPSILON);
        }

        public IEnumerable<Vector2> Sequence(int nInitialize, int limit, int skip = 0) {
            currUV = new Vector2(Random.value, Random.value);
            currValue = Mathf.Max (Func (currUV), cutoff);

            for (var i = 0; i < nInitialize; i++)
                Next();

            for (var i = 0; i < limit; i++) {
                for (var j = 0; j < skip; j++)
                    Next ();
                if (cutoff < currValue)
                    yield return currUV;
                Next ();
            }
        }

        Vector2 RandomStep() {
            return new Vector2 (
                sigma * dist.x * Random.Range (-1f, 1f),
                sigma * dist.y * Random.Range (-1f, 1f));                
        }
        void Next() {
            var next = RandomStep() + currUV;
            next = Repeat(next);

            var nextValue = Mathf.Max (Func (next), cutoff);
            if (Mathf.Min(1f, nextValue / currValue) >= Random.value) {
                currUV = next;
                currValue = nextValue;
            }
        }
        Vector2 Repeat(Vector2 uv) {
            var ix = Mathf.FloorToInt (uv.x);
            var iy = Mathf.FloorToInt (uv.y);
            return new Vector2 (uv.x - ix, uv.y - iy);
        }
        Vector2 Reflect(Vector2 uv) {
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
