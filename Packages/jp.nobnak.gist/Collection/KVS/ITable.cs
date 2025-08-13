using System.Collections.Generic;

namespace nobnak.Gist.Collection.KVS {

	public interface IReadonlyTable<K, out R>
		: IEnumerable<R> {

		R this[K key] { get; }
		int Count { get; }

		R Get(K key);
		int IndexAt(K key);
		K KeyAt(int index);
		bool ContainsKey(K key);

		IEnumerable<K> Keys { get; }
		IEnumerable<R> Values { get; }

	}

	public interface ITable { }

	public interface ITable<K, R>
		: IReadonlyTable<K, R>, ITable
		where R : IRow<K> 
	{
		bool Remove(K key);
		bool Remove(R row);
		void Set(R row);
	}
}
