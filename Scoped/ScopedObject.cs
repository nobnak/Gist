using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Scoped {

    public class ScopedObject<T> : Scoped<T> where T : Object {

        public ScopedObject(T data) : base(data) {  }

        protected override void Disposer(T data) {
            ObjectDestructor.Destroy(data);
        }

        public static implicit operator ScopedObject<T>(T data) {
            return new ScopedObject<T>(data);
        }
        public static implicit operator T(ScopedObject<T> scoped) {
            return scoped.Data;
        }
    }

    public class ScopedPlug<T> : Scoped<T> {
        protected System.Action<T> disposer;

        public ScopedPlug(T data, System.Action<T> disposer) : base(data) {
            this.disposer = disposer;
        }

        protected override void Disposer(T data) {
            disposer(data);
        }
    }

    public abstract class Scoped<T> : System.IDisposable {
        protected T data;

        public Scoped(T data) {
            Create(data);
        }

        public virtual T Data {
            get { return data; }
            set {
                Dispose();
                Create(value);
            }
        }
        public virtual bool Disposed { get; protected set; }

        public void Dispose() {
            if (!Disposed) {
                Disposed = true;
                Disposer(Data);
            }
        }

        protected virtual void Create(T data) {
            this.data = data;
            this.Disposed = false;
        }
        protected abstract void Disposer(T data);
    }
}
