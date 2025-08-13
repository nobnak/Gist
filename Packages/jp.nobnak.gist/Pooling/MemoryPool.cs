using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.Pooling {

    public class MemoryPool<T> : IMemoryPool<T>, System.IDisposable {
        public event System.Action<T> OnNew;
        public event System.Action<T> OnFree;
        
        protected System.Func<T> create;
        protected System.Action<T> reset;
        protected System.Action<T> delete;

        protected Stack<T> _pool = new Stack<T>();

        public MemoryPool(System.Func<T> create, System.Action<T> reset, System.Action<T> delete) {
            this.create = create;
            this.reset = reset;
            this.delete = delete;
        }

        #region IMemoryPool
        public T New() {
            lock (this) {
                T o;
                if (!TryPop(out o))
                    o =create ();

                NotifyOnNew (o);
                return o;
            }
        }
        public IMemoryPool<T> Free(T used) {
            lock (this) {
                reset(used);
                Push (used);
                NotifyOnFree (used);
                return this;
            }
        }
		public int Count {
			get { return _pool.Count; }
		}
        #endregion

        #region IDisposable implementation
        public virtual void Dispose () {
            if (_pool != null) {
                while (_pool.Count > 0)
                    delete(_pool.Pop());
                _pool = null;
            }
        }
        #endregion

        protected virtual bool TryPop (out T fresh) {
            if (_pool.Count > 0) {
                fresh = _pool.Pop ();
                return true;
            }
            fresh = default(T);
            return false;
        }
        protected virtual void Push(T used) {
            _pool.Push (used);
        }

        protected virtual void NotifyOnNew(T o) {
            if (OnNew != null)
                OnNew (o);
        }
        protected virtual void NotifyOnFree(T o) {
            if (OnFree != null)
                OnFree (o);
        }
    }

    public static class MemoryPoolUtil {

        public static void Free<T>(IList<T> data, IMemoryPool<T> pool) {
            foreach (var d in data)
                pool.Free(d);
			if (!data.IsReadOnly)
				data.Clear();
        }
    }
}
