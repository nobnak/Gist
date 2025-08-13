using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace nobnak.Gist.Syscall {

	public static class Kern32 {

		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		public static extern void CopyMemory(System.IntPtr dest, System.IntPtr src, uint count);
	}
}
