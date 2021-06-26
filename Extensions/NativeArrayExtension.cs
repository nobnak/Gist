using nobnak.Gist.Syscall;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace nobnak.Gist.Extensions.NativeArrayExt {

	public static class NativeArrayExtension {

		public static void UnsafeCopyTo<T>(this NativeArray<T> src, T[] dst) where T:struct {
#if UNSAFE
            unsafe {
				var pSrc = (System.IntPtr)src.GetUnsafePtr();
				var hDst = GCHandle.Alloc(dst, GCHandleType.Pinned);
				try {
					var pDst = Marshal.UnsafeAddrOfPinnedArrayElement(dst, 0);
					Kern32.CopyMemory(pDst, pSrc,
						(uint)(dst.Length * Marshal.SizeOf(typeof(T)))
						);
				} finally {
					hDst.Free();
				}
			}
#else
			//Debug.LogWarning("Unsafe copy is not enabled");
			src.CopyTo(dst);
#endif
		}
	}
}
