using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Extensions.GPUExt {

	public static class GPUExtension {

		public static int DispatchSize(this int threadCount, int groupSize) {
			return (threadCount - 1) / groupSize + 1;
		}
	}
}
