#pragma warning disable 0067

using nobnak.Gist.Extensions.GPUExt;
using nobnak.Gist.Extensions.NativeArrayExt;
using nobnak.Gist.ThreadSafe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.GPUBuffer {

	public abstract class AsyncCPUTexture {
		public enum StateEnum { Stopped = 0, Progress }

		protected bool autoreset = true;
		protected StateEnum state;
	}

	public class AsyncCPUTexture<T> 
		: AsyncCPUTexture, System.IDisposable, ITextureData<T>, IEnumerable
		where T:struct {

		public event System.Action<IList<T>, ListTextureData<T>, bool> OnComplete;
		public event Action<ITextureData<T>> OnLoad;

		protected T[] data;
		protected ListTextureData<T> output;
		protected AsyncGPUReadbackRequest req;
		protected Vector2Int size;
		protected T defaultValue;

		public AsyncCPUTexture(T defaultValue = default(T)) {
			this.defaultValue = defaultValue;
		}

		#region interface

		#region IEnumerable
		public IEnumerator GetEnumerator() {
			return StartCoroutine();
		}
		#endregion

		#region ITextureData
		public virtual Vector2Int Size { get { return size; } }
		public Func<float, float, T> Interpolation { get; set; }
		public virtual T this[Vector2 uv] {
			get { return (output != null ? output[uv] : defaultValue); }
		}
		public virtual T this[float nx, float ny] {
			get { return (output != null ? output[nx, ny] : defaultValue); }
		}
		public virtual T this[int x, int y] {
			get { return (output != null ? output[x, y] : defaultValue); }
		}
		#endregion

		#region IDisposable
		public virtual void Dispose() {
			Release();
		}
		#endregion

		public Texture Source { get; set; }
		public bool AutoReset {
			get { return autoreset; }
			set { autoreset = value; }
		}
		public virtual void Start() {
			if (state == StateEnum.Stopped && Source != null) {
				if (!Source.graphicsFormat.IsSupportedForReadPixels()) {
					Debug.LogWarning($"Format is not supported for readpixel : {Source.graphicsFormat}");
					return;
				}
				req = AsyncGPUReadback.Request(Source);
				size = new Vector2Int(req.width, req.height);
				state = StateEnum.Progress;
			}
		}
		public virtual void Stop() {
			state = StateEnum.Stopped;
		}

		public virtual void Update() {
			if (state == StateEnum.Progress)
				Progress();
			if (state == StateEnum.Stopped && autoreset)
				Start();
		}
		public virtual IEnumerator StartCoroutine() {
			for (Start(); state != StateEnum.Stopped; Progress())
				yield return null;
		}
		#endregion

		#region private
		private void Progress() {
			if (req.hasError) {
				Debug.LogFormat("Failed to read back from GPU async");
				Release();
				Notify(false);
				Stop();
			} else if (req.done) {
				Release();
				var nativeData = req.GetData<T>();
				System.Array.Resize(ref data, nativeData.Length);
				nativeData.UnsafeCopyTo(data);
				output = GenerateCPUTexture(data, size);
				Notify(true);
				Stop();
			}
		}
		private void Notify(bool result) {
			if (OnComplete != null)
				OnComplete.Invoke(data, output, result);
		}
		protected virtual ListTextureData<T> GenerateCPUTexture(IList<T> data, Vector2Int size) {
			var tex = new ListTextureData<T>(data, size);
			tex.Interpolation = Interpolation;
			return tex;
		}
		protected virtual void Release() {
			if (output != null) {
				output.Dispose();
				output = null;
			}
		}
		#endregion
	}
}
