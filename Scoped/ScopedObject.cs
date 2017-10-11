using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist.Scoped {

    public class ScopedObject<T> : ScopedObject where T : Object {
        public T Data { get; protected set; }

        public ScopedObject(T data) {
            this.Data = data;
            this.Disposed = false;
        }

        public override void Dispose() {
            if (!Disposed) {
                Disposed = true;
                Release(Data);
            }
        }

        public static implicit operator ScopedObject<T>(T data) {
            return new ScopedObject<T>(data);
        }
        public static implicit operator T(ScopedObject<T> scoped) {
            return scoped.Data;
        }
    }

    public abstract class ScopedObject : System.IDisposable {
        public bool Disposed { get; protected set; }

        public static void Release(Object obj) {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }

        public abstract void Dispose();
    }
}
