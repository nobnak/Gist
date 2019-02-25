#if NET_2_0 || NET_2_0_SUBSET

using System.Collections.Generic;

public interface IReadOnlyCollection<T> : IEnumerable<T> {
	int Count { get; }
}

public interface IReadOnlyList<T> : IReadOnlyCollection<T> {
	T this[int index] {get;}
}

#endif