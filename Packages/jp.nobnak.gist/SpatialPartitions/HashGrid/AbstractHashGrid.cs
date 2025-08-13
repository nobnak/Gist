using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.HashGridSystem.Storage;

namespace nobnak.Gist.HashGridSystem {

    public abstract class AbstractHashGrid : AbstractHashGrid<Component> {}

    public abstract class AbstractHashGrid<T> : MonoBehaviour {
        public abstract void Add (T m);
        public abstract void Remove (T point);
        public abstract T Find (System.Predicate<T> Predicate);
        public abstract IEnumerable<S> Neighbors<S> (Vector3 center, float distance) where S : class, T;
        public abstract IEnumerable<T> Points { get; }


        public abstract int Count { get; }
        public abstract T IndexOf (int index);
    }
}
