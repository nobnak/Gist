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

    public class MemoryPool<T> : IMemoryPool<T> where T : new () {
        Stack<T> _pool = new Stack<T>();

        public T New() {
            lock (this) {
                if (_pool.Count > 0)
                    return _pool.Pop ();
                return new T ();
            }
        }
        public IMemoryPool<T> Free(T used) {
            lock (this) {
                _pool.Push (used);
                return this;
            }
        }
    }
}
