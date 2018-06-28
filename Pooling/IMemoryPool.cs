using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nobnak.Gist.Pooling {

    #region Interface
    public interface IMemoryPool<T> {
        T New();
        IMemoryPool<T> Free(T used);
		int Count { get; }
    }
    #endregion
	
}
