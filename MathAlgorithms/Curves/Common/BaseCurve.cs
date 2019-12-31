using nobnak.Gist;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.MathAlgorithms.Curves.Common {

    public abstract class BaseCurve : IParametricCurve, IList<Vector3> {
        public event System.Action Changed;

        protected Validator validator = new Validator();
        protected List<Vector3> points = new List<Vector3>();

        public BaseCurve() {
            validator.Validated += () => {
                if (Changed != null)
                    Changed();
            };
        }

        #region interface
        #region IParametricCurve
        public abstract bool Valid { get; }
        public abstract float ParameterLength { get; }
        public abstract ILineStrip GetLineStrip();
        public abstract Vector3 PositionAt(float t);
        public abstract Vector3 DirectionAt(float t);
        public abstract float CurvatureAt(float t);

        public ParametricPoint PointAt(float t) {
            return new ParametricPoint(this, t);
        }
        #endregion

        #region IList
        public virtual bool IsReadOnly {
            get { return false; }
        }
        public virtual int Count {
            get {
                validator.Validate();
                return points.Count;
            }
        }
        public Vector3 this[int index] {
            get {
                validator.Validate();
                return points[index];
            }
            set {
                if (points[index] != value) {
                    validator.Invalidate();
                    points[index] = value;
                }
            }
        }
        public void Insert(int index, Vector3 item) {
            validator.Invalidate();
            points.Insert(index, item);
        }
        public void RemoveAt(int index) {
            validator.Invalidate();
            points.RemoveAt(index);
        }
        public virtual void Add(Vector3 item) {
            validator.Invalidate();
            points.Add(item);
        }
        public virtual void Clear() {
            validator.Invalidate();
            points.Clear();
        }
        public virtual bool Remove(Vector3 item) {
            validator.Invalidate();
            return points.Remove(item);
        }

        public int IndexOf(Vector3 item) {
            validator.Validate();
            return points.IndexOf(item);
        }
        public virtual bool Contains(Vector3 item) {
            validator.Validate();
            return points.Contains(item);
        }
        public virtual void CopyTo(Vector3[] array, int arrayIndex) {
            validator.Validate();
            points.CopyTo(array, arrayIndex);
        }
        public virtual IEnumerator<Vector3> GetEnumerator() {
            validator.Validate();
            return points.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
        #endregion
    }
}
