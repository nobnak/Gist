using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Gist {

	public class GPUList<T> : GPUArray<T> where T : struct {
	    public const int MINIMUM_CAPACITY = 16;

		public GPUList(int initialCapacity = MINIMUM_CAPACITY) : base(initialCapacity) {
	        Resize (OptimalCapacity (initialCapacity));
	    }

		protected override bool TryChangeCount (int nextCount) {
			Resize (OptimalCapacity (nextCount));
			return base.TryChangeCount (nextCount);
		}
	    protected virtual int OptimalCapacity(int count) {
	        return SmallestPowerOfTwoGreaterThan(Mathf.Max(MINIMUM_CAPACITY, count));
	    }
	    protected virtual void Resize (int targetCapacity) {
			if (cpuBuffer == null || cpuBuffer.Length != targetCapacity) {
				System.Array.Resize (ref cpuBuffer, targetCapacity);
				Release (ref gpuBuffer);
				gpuBuffer = CreateGPUBuffer (targetCapacity);
				Upload ();
				bufferIsDirty = false;
	        }
	    }
	    public static int SmallestPowerOfTwoGreaterThan (int n) {
	        --n;
	        n |= n >> 1;
	        n |= n >> 2;
	        n |= n >> 4;
	        n |= n >> 8;
	        n |= n >> 16;
	        return ++n;
	    }
	}
}