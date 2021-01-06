using UnityEngine;
using System.Collections.Generic;

namespace nobnak.Gist {
	#region Interfaces
#if NET35
	public interface IReadOnlyCollection<Value> : IEnumerable<Value> {
        int Count { get; }
    }
    public interface IReadOnlyList<Value> : IReadOnlyCollection<Value> {
        Value this[int i] { get; }
    }
#endif
	#endregion

    public class IndexedList<Value> : IReadOnlyList<Value> {
        public readonly IList<int> Indices;
        public readonly IList<Value> Values;

        public IndexedList(IList<int> indices, IList<Value> values) {
            this.Indices = indices;
            this.Values = values;
        }

#region IEnumerable implementation
        public IEnumerator<Value> GetEnumerator () {
            foreach (var i in Indices)
                yield return Values [i];
        }
#endregion
#region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () {
            return ((IEnumerable<Value>)this).GetEnumerator ();
        }
#endregion
#region IReadOnlyList implementation
        public Value this [int i] {
            get {
                return Values [Indices [i]];
            }
        }
#endregion
#region IReadOnlyCollection implementation
        public int Count {
            get {
                return Values.Count;
            }
        }
#endregion

    }
}
