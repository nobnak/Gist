using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.SpatialPartition {

	public interface ISpatialPartition<Entity, Position> {

		void Add(Entity entity);
		void Remove(Entity entity);
		void UpdatePosition(System.Func<Entity, Position> getPosition);
		IEnumerable<Entity> RadialSearch(Position center, float radius);
		Entity Neareset(Position center);
	}
}
