using UnityEngine;

namespace Blending.Gist.Scoped {

    public class ScopedObject<T> : Scoped<T> where T : Object {

        public ScopedObject(T data) : base(data) {  }

        public override void Disposer(T data) {
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

        public override void Disposer(T data) {
            disposer(data);
        }
    }

    public abstract class Scoped<T> : System.IDisposable {
        public T Data { get; protected set; }
        public bool Disposed { get; protected set; }

        public Scoped(T data) {
            this.Data = data;
            this.Disposed = false;
        }

        public void Dispose() {
            if (!Disposed) {
                Disposed = true;
                Disposer(Data);
            }
        }

        public abstract void Disposer(T data);
    }
}
