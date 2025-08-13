using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2.Extensions {
	public static class LayerExtension {

		public static Vector2 LocalToUvPos(this Layer l, Vector3 localPos) {
			return new Vector2(localPos.x + 0.5f, localPos.y + 0.5f);
		}
		public static Vector3 UvToLocalPos(this Layer l, Vector2 uv, float z = 0f) {
			return new Vector3(uv.x - 0.5f, uv.y - 0.5f, z);
		}

		public static Vector3 LocalToLayerPos(this Layer l, Vector3 localPos) {
			return l.LocalToLayer.TransformPoint(localPos);
		}
		public static Vector3 LayerToLocalPos(this Layer l, Vector3 layerPos) {
			return l.LocalToLayer.InverseTransformPoint(layerPos);
		}

		public static Vector3 UvToLayerPos(this Layer l, Vector2 uv) {
			return l.LocalToLayerPos(l.UvToLocalPos(uv));
		}
		public static Vector3 LayerToUvPos(this Layer l, Vector3 layerPos) {
			return l.LocalToUvPos(l.LayerToLocalPos(layerPos));
		}

		public static Vector3 WorldToLayerPos(this Layer l, Vector3 worldPos) {
			return l.LayerToWorld.InverseTransformPoint(worldPos);
		}
		public static Vector2 WorldToLocalPos(this Layer l, Vector3 worldPos) {
			return l.LocalToWorld.InverseTransformPoint(worldPos);
		}
		public static Vector2 WorldToUvPos(this Layer l, Vector3 worldPos) {
			return l.LocalToUvPos(l.WorldToLocalPos(worldPos));
		}
		public static Vector3 UvToWorldPos(this Layer l, Vector2 uv, float z = 0f) {
			var layerpos = l.UvToLayerPos(uv);
			layerpos.z = z;
			return l.LayerToWorld.TransformPoint(layerpos);
		}

        public static Vector3 ProjectOn(this Layer l, Vector3 worldpos, float depth = 0f) {
            var layerpos = l.LayerToWorld.InverseTransformPoint(worldpos);
            layerpos.z = depth;
            worldpos = l.LayerToWorld.TransformPoint(layerpos);
            return worldpos;
        }
	}
}
