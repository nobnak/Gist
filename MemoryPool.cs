using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist {
    #region Interface
    public interface IMemoryPool<T> {
        T New();
        IMemoryPool<T> Free(T used);
    }
    #endregion

    public abstract class AbstractMemoryPool<T> : IMemoryPool<T> {
        public event System.Action<T> OnNew;
        public event System.Action<T> OnFree;

        protected Stack<T> _pool = new Stack<T>();

        #region IMemoryPool
        public T New() {
            lock (this) {
                T o;
                if (!TryPop (out o))
                    o =Create ();
                NotifyOnNew (o);
                return o;
            }
        }
        public IMemoryPool<T> Free(T used) {
            lock (this) {
                Push (used);
                NotifyOnFree (used);
                return this;
            }
        }
        #endregion

        protected abstract T Create ();

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

    public class MemoryPool<T> : AbstractMemoryPool<T> where T : new () {
        protected override T Create () {
            return new T ();
        }
    }

    public class OutsourceMemoryPool<T> : AbstractMemoryPool<T> {
        protected System.Func<T> create;

        public OutsourceMemoryPool(System.Func<T> create) {
            this.create = create;
        }

        protected override T Create () {
            return create ();
        }
    }
}
