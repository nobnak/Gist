using Gist.Extensions.Int;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Gist.GPUBuffer {

    public class GPUList<T> : System.IDisposable, IList<T> where T : struct {
        public const int MIN_CAPACITY = 1;
        public const int DEFAULT_CAPACITY = 16;

        protected int capacity;
        protected int count;
        protected bool disposed;
        protected bool dataChanged;
        protected ComputeBufferType cbtype;

        protected T[] data;
        protected ComputeBuffer buffer;

        public GPUList(int capacity, ComputeBufferType cbtype) {
            this.count = 0;
            this.capacity = 0;
            this.disposed = false;
            this.dataChanged = false;
            this.cbtype = cbtype;
            Resize(capacity);
        }
        public GPUList(int capacity) : this(capacity, ComputeBufferType.Default) { }
        public GPUList() : this(DEFAULT_CAPACITY) { }

        public int Capacity {
            get { return capacity; }
            set { Resize(value); }
        }

        public void Resize(int preferedSize) {
            preferedSize = Mathf.Max(preferedSize, MIN_CAPACITY);

            if (preferedSize != capacity) {
                System.Array.Resize(ref data, preferedSize);
                ResizeComputeBuffer(preferedSize);

                capacity = preferedSize;
                count = Mathf.Min(count, capacity);
            }
        }
        public bool Upload() {
            if (dataChanged) {
                dataChanged = false;
                buffer.SetData(data);
                buffer.SetCounterValue((uint)count);
                return true;
            }
            return false;
        }
        public void Download() {
            dataChanged = false;
            buffer.GetData(data);
        }

        protected void ResizeComputeBuffer(int nextSize) {
            if (buffer != null)
                buffer.Dispose();
            buffer = new ComputeBuffer(nextSize, Marshal.SizeOf(typeof(T)), cbtype);
        }
        protected void EnsureCapacity(int minCapacity) {
            if (minCapacity > capacity)
                Resize(minCapacity.SmallestPowerOfTwoGreaterThan());
        }

        #region IDisposable
        public void Dispose() {
            if (!disposed) {
                disposed = true;
                data = null;
                buffer.Dispose();
            }
        }
        #endregion

        #region IList
        public int Count {
            get { return count; }
        }
        public bool IsReadOnly { get { return false; } }
        public T this[int index] {
            get { return data[index]; }
            set {
                dataChanged = true;
                data[index] = value;
            }
        }
        public int IndexOf(T item) {
            for (var i = 0; i < count; i++)
                if (data[i].Equals(item))
                    return i;
            return -1;
        }
        public void Insert(int index, T item) {
            dataChanged = true;
            EnsureCapacity(count + 1);
            System.Array.Copy(data, index, data, index + 1, count - index);
            data[index] = item;
            count++;
        }
        public void RemoveAt(int index) {
            dataChanged = true;
            System.Array.Copy(data, index + 1, data, index, count - (index + 1));
            count--;
        }
        public void Add(T item) {
            dataChanged = true;
            EnsureCapacity(count + 1);
            data[count++] = item;
        }
        public void Clear() {
            dataChanged = true;
            count = 0;
        }
        public bool Contains(T item) {
            return IndexOf(item) >= 0;
        }
        public void CopyTo(T[] array, int arrayIndex) {
            System.Array.Copy(data, 0, array, arrayIndex, count);
        }
        public bool Remove(T item) {
            var index = IndexOf(item);
            if (index >= 0) {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        public IEnumerator<T> GetEnumerator() {
            for (var i = 0; i < count; i++)
                yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
