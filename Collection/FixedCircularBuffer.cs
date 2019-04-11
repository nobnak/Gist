using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Collection {

    public class FixedCircularBuffer<T> : IEnumerable<T> {

        public readonly int size;

        protected readonly T[] array;

        protected int head = 0;
        protected int tail = 0;
        protected int count = 0;

        public FixedCircularBuffer(int size = 16) {
            this.size = size;
            this.array = new T[size];
        }

        #region interface
        #region IEnumerable
        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        public void Enqueue(T v) {
            array[tail] = v;
            tail = ++tail % size;
            count = ++count <= size ? count : size;
            head = (tail + size - count) % size;
        }
        public void Dequeue() {
            if (count <= 0)
                throw new System.InvalidOperationException("Empty");
            head = ++head % size;
            --count;
        }
        public T this[int index] {
            get { return array[(head + index) % size]; }
            set { array[(head + index) % size] = value; }
        }
        #endregion
    }
}
