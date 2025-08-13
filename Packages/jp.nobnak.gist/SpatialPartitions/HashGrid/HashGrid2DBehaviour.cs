using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nobnak.Gist.HashGridSystem.Storage;

namespace nobnak.Gist.HashGridSystem {

    public class HashGrid2DBehaviour : AbstractHashGrid {
        public enum UpdateModeEnum { Update = 0, Rebuild }

        public UpdateModeEnum updateMode;
        public float cellSize = 1f;
        public int gridWidth = 20;
        public Color gizmoColor = Color.white;

        public HashGrid2D<Component> World { get; private set; }

        #region Unity
        void OnEnable() {
            World = new HashGrid2D<Component> (GetPosition, cellSize, gridWidth, gridWidth);
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
            var offset = cellSize * new Vector3 (0.5f, 0.5f, 0f);
            Gizmos.color = gizmoColor;
            Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube (0.5f * size, size);

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

        #region implemented abstract members of AbstractHashGrid
        public override void Add (Component point) {
            World.Add (point);
        }
        public override void Remove (Component point) {
            World.Remove (point);
        }
        public override Component Find (System.Predicate<Component> Predicate) {
            return World.Find (Predicate);
        }
        public override IEnumerable<S> Neighbors<S> (Vector3 center, float distance) {
            return World.Neighbors<S> (GetPosition(center), distance);
        }
        public override IEnumerable<Component> Points {
            get { return World; }
        }

        public override int Count { get { return World.Count; } }
        public override Component IndexOf (int index) {
            return World.IndexOf (index);
        }
        #endregion


        Vector2 GetPosition(Component m) {
            return GetPosition(m.transform.position);
        }
        Vector2 GetPosition(Vector3 worldPos) {
            return (Vector2)transform.InverseTransformPoint (worldPos);
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