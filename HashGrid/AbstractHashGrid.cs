using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gist.HashGridSystem.Storage;

namespace Gist.HashGridSystem {

    public abstract class AbstractHashGrid : AbstractHashGrid<MonoBehaviour> {}

    public abstract class AbstractHashGrid<T> : MonoBehaviour {
        public abstract void Add (MonoBehaviour m);
        public abstract void Remove (T point);
        public abstract T Find (System.Predicate<T> Predicate);
        public abstract IEnumerable<S> Neighbors<S> (Vector3 center, float distance) where S : class, T;
        public abstract IEnumerable<T> Points { get; }
    }
}
