using nobnak.Gist.Extensions.Int;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.GPUBuffer {

    public class GPUList<T> : System.IDisposable, IList<T> where T : struct {
        public const int MIN_CAPACITY = 1;
        public const int DEFAULT_CAPACITY = 16;

        public enum DirtyFlag { Clean = 0, Data, Buffer }

        protected int capacity;
        protected int count;
        protected DirtyFlag dirty;
        protected ComputeBufferType cbtype;

        protected T[] data;
        protected ComputeBuffer buffer;

        public GPUList(
                int capacity = DEFAULT_CAPACITY,
                ComputeBufferType cbtype = ComputeBufferType.Default) {
            this.count = 0;
            this.capacity = 0;
            this.dirty = DirtyFlag.Data;
            this.cbtype = cbtype;
            Resize(capacity);
        }
		public GPUList(
			IEnumerable<T> iter,
			int capacity = DEFAULT_CAPACITY,
			ComputeBufferType cbtype = ComputeBufferType.Default)
			: this(capacity, cbtype) {
			foreach (var v in iter)
				Add(v);
		}

		#region interface
		public ComputeBuffer Buffer {
            get {
                Upload();
                dirty = DirtyFlag.Buffer;
                return buffer;
            }
        }
        public T[] Data {
            get {
                Download();
                dirty = DirtyFlag.Data;
                return data;
            }
        }
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
            if (dirty == DirtyFlag.Data) {
                dirty = DirtyFlag.Clean;
                buffer.SetData(data);
                //buffer.SetCounterValue((uint)count);
                return true;
            }
            return false;
        }
        public void Download() {
            if (dirty == DirtyFlag.Buffer) {
                dirty = DirtyFlag.Clean;
                buffer.GetData(data);
            }
        }
		public static implicit operator ComputeBuffer (GPUList<T> gl) {
			return gl.Buffer;
		}
		#endregion

		#region private
		protected void ResizeComputeBuffer(int nextSize) {
            dirty = DirtyFlag.Data;
            DisposeComputeBuffer();
            buffer = new ComputeBuffer(nextSize, Marshal.SizeOf(typeof(T)), cbtype);
        }
        protected void EnsureCapacity(int minCapacity) {
            if (minCapacity > capacity)
                Resize(minCapacity.Po2());
        }
        protected void DisposeComputeBuffer() {
            if (buffer != null) {
                buffer.Dispose();
                buffer = null;
            }
        }
		#endregion

		#region IDisposable
		public void Dispose() {
            DisposeComputeBuffer();
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
                dirty = DirtyFlag.Data;
                count = (index >= count ? (index + 1) : count);
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
            dirty = DirtyFlag.Data;
            EnsureCapacity(count + 1);
            System.Array.Copy(data, index, data, index + 1, count - index);
            data[index] = item;
            count++;
        }
        public void RemoveAt(int index) {
            dirty = DirtyFlag.Data;
            System.Array.Copy(data, index + 1, data, index, count - (index + 1));
            count--;
        }
        public void Add(T item) {
            dirty = DirtyFlag.Data;
            EnsureCapacity(count + 1);
            data[count++] = item;
        }
        public void Clear() {
            dirty = DirtyFlag.Data;
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
