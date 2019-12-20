using nobnak.Gist.Extensions.IListExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.Common {

    public class DatabaseOnCurve<T> : IEnumerable<DatabaseOnCurve<T>.Row> {

        protected SortedList<float, Row> database = new SortedList<float, Row>();

        public DatabaseOnCurve(BaseCurve curve) {
            this.Curve = curve;
        }

        #region interface
        #region IEnumerable
        public IEnumerator<Row> GetEnumerator() {
            return database.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        public BaseCurve Curve { get; protected set; }

        public int IndexOf(float t) {
            var i = database.Keys.Bisect(t);
            if (i < 0)
                i = ~i;
            return i;
        }
        public IEnumerable<Row> GetEnumeratorFrom(float t) {
            for (var i = IndexOf(t); i < database.Count; i++)
                yield return database[i];
        }
        public Row AddAt(float t, T value) {
            t = Mathf.Clamp(t, 0f, Curve.ParameterLength);
            var r = new Row(Curve, t, value);
            database.Add(t, r);
            return r;
        }
        public bool Remove(Row r) {
            var i = database.IndexOfValue(r);
            var found = (i >= 0);
            if (found)
                database.RemoveAt(i);
            return found;
        }
        #endregion

        #region static
        public static implicit operator BaseCurve(DatabaseOnCurve<T> me) {
            return me.Curve;
        }
        #endregion

        #region classes
        public struct Row {
            public readonly BaseCurve curve;
            public readonly float t;
            public readonly T value;

            public Row(BaseCurve curve, float t, T value) {
                this.curve = curve;
                this.t = t;
                this.value = value;
            }
        }
        #endregion
    }
}
