using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gist.HashGridSystem.Storage;

namespace Gist.HashGridSystem {

    [ExecuteInEditMode]
    public class HashGrid2D : MonoBehaviour {
        public enum UpdateModeEnum { Update = 0, Rebuild }

        public UpdateModeEnum updateMode;
        public float cellSize = 1f;
        public int gridWidth = 20;
        public Color gizmoColor = Color.white;

        public Storage2D<MonoBehaviour> World { get; private set; }

        #region Unity
        void OnEnable() {
            World = new Storage2D<MonoBehaviour> (GetPosition, cellSize, gridWidth, gridWidth);
        }
        void LateUpdate() {
            switch (updateMode) {
            default:
                World.Update ();
                break;
            case UpdateModeEnum.Rebuild:
                World.Rebuild (cellSize, gridWidth, gridWidth);
                break;
            }
        }
        void OnDrawGizmos() {
            if (!(World != null && isActiveAndEnabled))
                return;
            
            var size = gridWidth * cellSize * new Vector3 (1f, 1f, 0f);
			var center = transform.position;
            var offset = cellSize * new Vector3 (
                             Mathf.FloorToInt (center.x / cellSize) + 0.5f, 
                             Mathf.FloorToInt (center.y / cellSize) + 0.5f, 
                             0f);
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube (center + 0.5f * size, size);

            var cubeSize = 0.5f * cellSize * new Vector3 (1f, 1f, 0f);
			var hash = World.GridInfo;
			for (var y = 0; y < hash.ny; y++) {
				for (var x = 0; x < hash.nx; x++) {
                    var count = World.Stat (x, y);
                    var pos = cellSize * new Vector3 (x, y, 0f) + offset;

                    if (count > 0) {
                        var h = Mathf.Clamp01 ((float)count / 100);
                        Gizmos.color = Jet (h, 0.5f * Mathf.Clamp01 (count / 10f));
                        Gizmos.DrawCube (pos, cubeSize);
                    } else {
                        Gizmos.color = 0.5f * gizmoColor;
                        Gizmos.DrawWireCube (pos, cubeSize);
                    }
                }
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
        void OnDisable() {
            World.Dispose ();
            World = null;
        }
        #endregion

        Vector2 GetPosition(MonoBehaviour m) {
            return (Vector2)transform.InverseTransformPoint (m.transform.position);
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