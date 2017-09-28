using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NonInstancedMaterialProperty {

    public class Retainer<T> : System.IDisposable {
        protected T value;
        protected System.Action<T> disposer;
        protected int referenceCount = 0;
        protected bool disposed = false;

        public Retainer(T value, System.Action<T> disposer) {
            this.value = value;
            this.disposer = disposer;
        }

        public void Dispose() {
            if (!disposed) {
                disposed = true;
                disposer(value);
            }
        }

        public Token Retain() {
            var t = new Token(this);
            ++referenceCount;
            return t;
        }

        protected void Release(Token t) {
            if (--referenceCount == 0)
                Dispose();
        }

        public class Token : System.IDisposable {
            protected Retainer<T> retainer;
            public bool Disposed { get; private set; }

            public Token(Retainer<T> retainer) {
                this.retainer = retainer;
                this.Disposed = false;
            }

            public T Value {  get { return retainer.value;  } }
            public void Dispose() {
                if (!Disposed) {
                    Disposed = true;
                    retainer.Release(this);
                }
            }
        }
    }
}
