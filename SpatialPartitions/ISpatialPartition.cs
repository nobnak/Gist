using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.SpatialPartition { 

	public interface ISpatialPartition<ID, Position> {

		void Build(IEnumerable<ID> ids, IEnumerable<Position> positions);
		void Add(ID id, Position pos);
		void Remove(ID id);
		IEnumerable<ID> RadialSearch(Position center, float radius);
		ID Neareset(Position center);
	}

	public interface ISpatialPartition3D : ISpatialPartition<int, Vector3> { }
}
