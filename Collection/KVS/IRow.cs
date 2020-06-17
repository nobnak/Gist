using System.Collections.Generic;
using System.Reflection;

namespace nobnak.Gist.Collection.KVS {



	public interface IRow<out K> {
		K Key { get; }
	}
}