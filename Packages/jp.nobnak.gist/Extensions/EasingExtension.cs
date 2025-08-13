using UnityEngine;
using System.Collections;

namespace nobnak.Gist.Extensions.Easing {

	public static class EasingExtension {

        public static Easer Interpolate(this MonoBehaviour bhv, Easings.Functions mode, float duration) {
            return new Easer(bhv).Interpolate (mode, duration);
		}
	}

	public class Easer {
        public const float EPSILON = 1e-3f;

        public event System.Action<float> OnBegin;
        public event System.Action<float> OnEnd;        

        Invoker _invoker;
		MonoBehaviour _bhv;
        Coroutine _coroutine;

		public Easer(MonoBehaviour bhv) {
            this._bhv = bhv;
		}

        public Easer Interpolate(Easings.Functions mode, float duration) {
            Stop ();
            if (_bhv.isActiveAndEnabled)
                _coroutine = _bhv.StartCoroutine (Interpolation (mode, duration));
            return this;
        }
        public Easer ForEach(System.Action<float> func, float start, float end) {
            _invoker = new Invoker (func, start, end);
            return this;
        }
        public Easer ForEach(System.Action<float> func, float end) {
            var curr = _invoker.current;
            _invoker = new Invoker (func, curr, end);
            return this;
        }
        public void Stop () {
            if (_coroutine != null)
                _bhv.StopCoroutine (_coroutine);
            _coroutine = null;
        }

        Easer Func(float t) {
            _invoker.Invoke (t);
            return this;
        }
        IEnumerator Interpolation(Easings.Functions mode, float duration) {
            if (-EPSILON < duration && duration < EPSILON)
                yield break;
            yield return null;

            var t = (duration > 0f ? 0f : 1f);
            var ds = 1f / duration;

            NotifyOnBegin (t);
            while (true) {
                t += Time.deltaTime * ds;
                if (t < 0f || 1f < t)
                    break;

                Func (Easings.Interpolate (t, mode));
                yield return null;
            }

            Func (Easings.Interpolate (Mathf.Clamp01 (t), mode));
            NotifyOnEnd (t);
            _coroutine = null;
        }

        void NotifyOnBegin(float t) {
            if (OnBegin != null)
                OnBegin (t);
        }
        void NotifyOnEnd(float t) {
            if (OnEnd != null)
                OnEnd(t);
        }

        public struct Invoker {
            public readonly System.Action<float> func;
            public readonly float start;
            public readonly float end;
            public readonly float duration;

            public float current;

            public Invoker(System.Action<float> func, float start, float end) {
                this.func = func;
                this.start = start;
                this.end = end;
                this.duration = end - start;
                this.current = start;
            }
            public float Invoke(float t) {
                current = duration * t + start;
                if (func != null)
                    func(current);
                return current;
            }
        }
	}
}
