using nobnak.Gist.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public class Categorizer<S, T> where T : System.IComparable<T> {
        protected System.Func<S, T> categorize;
        protected Dictionary<T, List<S>> dataInCategory;
        protected MemoryPool<List<S>> cagePool;

        public Categorizer() : this(null) {}
        public Categorizer(System.Func<S, T> categorize) {
            Set(categorize);
            this.dataInCategory = new Dictionary<T, List<S>> ();
            this.cagePool = new MemoryPool<List<S>>(() => new List<S>(), (l) => l.Clear(), (l) => { });
        }

        #region Add
        public void Add(S d) {
            var category = categorize (d);
            List<S> cage;
            if (!dataInCategory.TryGetValue (category, out cage))
                dataInCategory[category] = cage = cagePool.New ();
            cage.Add (d);
        }
        public void Add(IEnumerable<S> dd) {
            foreach (var d in dd)
                Add (d);
        }
        #endregion

        #region Remove / Pop
        public void Remove(S d) {
            var category = categorize (d);
            List<S> cage;
            if (dataInCategory.TryGetValue (category, out cage)
                && cage.Remove (d)
                && cage.Count == 0
                && dataInCategory.Remove (category))
                cagePool.Free (cage);            
        }
        public void Remove(IEnumerable<S> dd) {
            foreach (var d in dd)
                Remove(d);
        }
        public S Pop(T category) {
            var cage = dataInCategory [category];
            var i = cage.Count - 1;
            var d = cage [i];
            cage.RemoveAt (i);
            return d;
        }
        #endregion

        #region Clear
        public void Clear() {
            foreach (var cage in dataInCategory.Values) {
                cage.Clear ();
                cagePool.Free (cage);
            }
            dataInCategory.Clear ();
        }
        #endregion

        public void Set(System.Func<S, T> categorize) {
            this.categorize = categorize;
        }
        public int Count(T category) {
            List<S> cage;
            return dataInCategory.TryGetValue (category, out cage) ? cage.Count : 0;
        }
        public IEnumerable<S> EnumerateData(T category) {
            List<S> cage;
            if (!dataInCategory.TryGetValue (category, out cage))
                yield break;
            foreach (var d in cage)
                yield return d;
        }
    }
}
