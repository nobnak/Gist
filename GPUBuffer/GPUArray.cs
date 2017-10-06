using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Gist {
		
	public class GPUArray<T> : System.IDisposable, IEnumerable<T> where T : struct {
		protected bool bufferIsDirty;
		protected int count;
		protected int capacity;
		protected T[] cpuBuffer;
		protected ComputeBufferType gpuBufferType;
		protected ComputeBuffer gpuBuffer;

		public GPUArray(int capacity) : this(0, capacity, ComputeBufferType.Default) {}
		public GPUArray(int capacity, ComputeBufferType bufferType) : this(0, capacity, bufferType) {}
		public GPUArray(int count, int capacity, ComputeBufferType bufferType) {
			this.count = count;
			this.capacity = capacity;
			this.gpuBufferType = bufferType;
			this.gpuBuffer = CreateGPUBuffer (capacity, bufferType);
			this.cpuBuffer = new T[capacity];
			Clear ();
		}
		
		#region Create
		protected virtual void Release<S>(ref S buf) where S : class, System.IDisposable  {
			if (buf != null) {
				buf.Dispose ();
				buf = null;
			}
		}
		protected virtual ComputeBuffer CreateGPUBuffer(int capacity) {
			return CreateGPUBuffer (capacity, gpuBufferType);
		}
		protected virtual ComputeBuffer CreateGPUBuffer (int capacity, ComputeBufferType bufferType) {
			return new ComputeBuffer (capacity, Marshal.SizeOf (typeof(T)), bufferType);
		}
		#endregion


		#region List
		public virtual int Count { get { return count; } }
		public virtual int Capacity { get { return capacity; } }
		public virtual T[] CPUBuffer { get { return cpuBuffer; } }
		public virtual ComputeBuffer GPUBuffer { 
			get {
				UploadIfBufferIsDirty ();
				return gpuBuffer; 
			}
		}
		public virtual void Clear() {
			System.Array.Clear (cpuBuffer, 0, cpuBuffer.Length);
			Upload ();
		}

		protected virtual bool TryChangeCount(int nextCount) {
			if (0 <= nextCount && nextCount < capacity) {
				count = nextCount;
				return true;
			}
			return false;
		}
		#endregion

		#region Elements 
		public virtual T this[int i] {
			get { return cpuBuffer [i]; }
			set { 
				bufferIsDirty = true;
				cpuBuffer [i] = value; 
			}
		}
		public virtual void Push(T e) {
			var i = count;
			if (TryChangeCount (count + 1)) {
				bufferIsDirty = true;
				cpuBuffer [i] = e;
			}
		}
		public virtual T Pop() {
			var i = count - 1;
			var e = cpuBuffer [i];
			if (TryChangeCount (i))
				bufferIsDirty = true;
			return e;
		}
		public virtual T RemoveAt(int indexOf) {
			bufferIsDirty = true;
			var e = cpuBuffer [indexOf];
			System.Array.Copy (cpuBuffer, indexOf + 1, cpuBuffer, indexOf, count - 1 - indexOf);
			return e;
		}
		#endregion

		#region CPU-GPU
		public virtual void SetBuffer(ComputeShader cs, int kernel, string name) {
			SetBuffer (cs, kernel, Shader.PropertyToID (name));
		}
		public virtual void SetBuffer(ComputeShader cs, int kernel, int name) {
			UploadIfBufferIsDirty ();
			cs.SetBuffer (kernel, name, GPUBuffer);
		}
        public virtual void SetBuffer(Material mat, string name) {
            SetBuffer (mat, Shader.PropertyToID (name));
        }
        public virtual void SetBuffer(Material mat, int name) {
            UploadIfBufferIsDirty ();
            mat.SetBuffer (name, GPUBuffer);
        }
		public virtual void Download() {
			GPUBuffer.GetData (cpuBuffer);
		}
		public virtual void Upload () {
			GPUBuffer.SetData (cpuBuffer);
		}
		public virtual void UploadIfBufferIsDirty() {
			if (bufferIsDirty) {
				bufferIsDirty = false;
				Upload();
			}
		}
		#endregion

		#region IDisposable implementation
		public virtual void Dispose () {
			Release (ref gpuBuffer);
		}
		#endregion

		#region IEnumerable implementation
		public virtual IEnumerator<T> GetEnumerator () {
			foreach (var e in cpuBuffer)
				yield return e;
		}
		#endregion
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator ();
		}
		#endregion
	}
}
