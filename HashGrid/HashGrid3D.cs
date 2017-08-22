using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gist.HashGridSystem.Storage;

namespace Gist.HashGridSystem {

    public class HashGrid3D : MonoBehaviour {
        public enum UpdateModeEnum { Update = 0, Rebuild }

        public UpdateModeEnum updateMode;
        public float cellSize = 1f;
        public int gridWidth = 20;
        public Color gizmoColor = Color.white;

        public Storage3D<MonoBehaviour> World { get; private set; }

        void Awake() {
            World = new Storage3D<MonoBehaviour> (GetPosition, cellSize, gridWidth, gridWidth, gridWidth);
        }
        void LateUpdate() {
            switch (updateMode) {
            default:
                World.Update ();
                break;
            case UpdateModeEnum.Rebuild:
                World.Rebuild (cellSize, gridWidth, gridWidth, gridWidth);
                break;
            }
        }
        void OnDrawGizmosSelected() {
            if (World == null)
                return;
            
			var size = gridWidth * cellSize * Vector3.one;
			var offset = transform.position;
            Gizmos.color = gizmoColor;
			Gizmos.DrawWireCube (offset + 0.5f * size, size);

			var cubeSize = 0.5f * cellSize * Vector3.one;
			var hash = World.GridInfo;
			for (var z = 0; z < hash.nz; z++) {
				for (var y = 0; y < hash.ny; y++) {
					for (var x = 0; x < hash.nx; x++) {
						var pos = cellSize * new Vector3 (
							x + Mathf.FloorToInt(offset.x / cellSize) + 0.5f, 
							y + Mathf.FloorToInt(offset.y / cellSize) + 0.5f,
							z + Mathf.FloorToInt(offset.z / cellSize) + 0.5f);
						var count = World.Stat (pos);
						if (count > 0) {
							var h = Mathf.Clamp01((float)count / 100);
							Gizmos.color = Jet (h, 0.5f * Mathf.Clamp01 (count / 10f));
                            Gizmos.DrawCube (pos, cubeSize);
                        }

                    }
                }
            }                        
        }

        Vector3 GetPosition(MonoBehaviour m) {
            return m.transform.position;
        }
		Color Jet(float x, float a) {
			return new Color(
				Mathf.Clamp01(Mathf.Min(4f * x - 1.5f, -4f * x + 4.5f)),
				Mathf.Clamp01(Mathf.Min(4f * x - 0.5f, -4f * x + 3.5f)),
				Mathf.Clamp01(Mathf.Min(4f * x + 0.5f, -4f * x + 2.5f)),
				a);
		}
    }
}
