using nobnak.Gist.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.GPUBuffer {

	public class AsyncCPUTexture<T> : System.IDisposable, ITextureData<T> where T:struct {
		public event System.Action<NativeArray<T>, NativeArrayTextureData<T>> OnComplete;

		protected bool active = false;

		protected NativeArrayTextureData<T> output;
		protected AsyncGPUReadbackRequest req;
		protected Vector2Int size;
		protected T defaultValue;

		public event Action<ITextureData<T>> OnLoad;

		public AsyncCPUTexture(T defaultValue = default(T)) {
			this.defaultValue = defaultValue;
		}

		#region interface
		public Texture Source { get; set; }
		public virtual void Update() {
			if (req.hasError) {
				active = false;
			} else if (req.done) {
				Release();
				var data = req.GetData<T>();
				output = GenerateCPUTexture(data, size);
				OnComplete?.Invoke(data, output);
			}

			if (!active && Source != null) {
				active = true;
				req = AsyncGPUReadback.Request(Source);
				size = new Vector2Int(Source.width, Source.height);
			}
		}
		#endregion
		#region ITextureData
		public virtual Vector2Int Size => size;
		public Func<float, float, T> Interpolation { get; set; }
		public virtual T this[Vector2 uv] => (output != null ? output[uv] : defaultValue);
		public virtual T this[float nx, float ny] => (output != null ? output[nx, ny] : defaultValue);
		public virtual T this[int x, int y] => (output != null ? output[x, y] : defaultValue);
		#endregion
		#region IDisposable
		public virtual void Dispose() {
			Release();
		}
		#endregion
		#region private
		protected virtual NativeArrayTextureData<T> GenerateCPUTexture(NativeArray<T> data, Vector2Int size) {
			var tex = new NativeArrayTextureData<T>(data, size);
			tex.Interpolation = Interpolation;
			return tex;
		}
		protected virtual void Release() {
			if (output != null) {
				output.Dispose();
				output = null;
			}
			active = false;
		}
		#endregion
	}
}
