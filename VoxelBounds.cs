using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public abstract class AbstractVoxelBounds {
    	public event System.Action<AbstractVoxelBounds> Changed;

        protected bool boundsChanged;
    	protected Bounds bounds;

    	protected Vector3 worldMinPosition;
    	protected Vector3 worldMaxPosition;

        #region Bounds Interface
        public virtual Bounds LocalBounds {
    		get { return bounds; }
    		set { 
                SetBounds (value);
                Update (); 
            }
    	}
        public virtual bool IsBoundsChanged() {
            return boundsChanged;
        }
        #endregion

        #region World Space Transform
    	public Vector3 NormalizedToLocalPosition(float u, float v, float w) {
    		var min = bounds.min;
    		var max = bounds.max;
    		return new Vector3 (Mathf.Lerp (min.x, max.x, u), Mathf.Lerp (min.y, max.y, v), Mathf.Lerp (min.z, max.z, w));
    	}
    	public Vector3 NormalizedToLocalPosition(Vector3 uvw) { return NormalizedToLocalPosition (uvw.x, uvw.y, uvw.z); }
    	public Vector3 NormalizedToWorldPosition(float u, float v, float w) {
    		return new Vector3 (
    			Mathf.Lerp (worldMinPosition.x, worldMaxPosition.x, u), 
    			Mathf.Lerp (worldMinPosition.y, worldMaxPosition.y, v), 
    			Mathf.Lerp (worldMinPosition.z, worldMaxPosition.z, w));
    	}
    	public Vector3 NormalizedToWorldPosition(Vector3 uvw) {
    		return NormalizedToWorldPosition (uvw.x, uvw.y, uvw.z);
    	}
        #endregion

        #region Matrix
        public virtual Matrix4x4 VoxelUvToLocalMatrix() {
            var min = bounds.min;
            var size = bounds.size;
            var m = Matrix4x4.zero;
            m [0] = size.x; m [12] = min.x;
            m [5] = size.y; m [13] = min.y;
            m [10] = size.z; m [14] = min.z;
            m [15] = 1f;
            return m;
        }
        #endregion

        #region Debug
        public void DrawGizmos() {
            Gizmos.matrix = LocalToWorldMatrix ();
            DrawGizmosLocal ();
            Gizmos.matrix = Matrix4x4.identity;
        }
        public void DrawGizmosLocal() {
            Gizmos.DrawWireCube (bounds.center, bounds.size);
        }
        #endregion

        public virtual void Update() {
            if (IsBoundsChanged())
                Rebuild ();
        }

    	protected abstract Vector3 TransformPoint (Vector3 pos);
        protected abstract Matrix4x4 LocalToWorldMatrix ();

        protected virtual void SetBounds(Bounds localBounds) {
            this.boundsChanged = true;
            this.bounds = localBounds;
        }
    	protected virtual void Rebuild() {
            boundsChanged = false;
    		Precompute ();
    		NotifyChanged ();
    	}
    	protected virtual void Precompute() {
    		worldMinPosition = TransformPoint (NormalizedToLocalPosition (0f, 0f, 0f));
    		worldMaxPosition = TransformPoint (NormalizedToLocalPosition (1f, 1f, 1f));
    	}
    	protected virtual void NotifyChanged() {
    		if (Changed != null)
    			Changed(this);
    	}
    }


    public class TransformVoxelBounds : AbstractVoxelBounds {
    	protected Transform tr;

    	public TransformVoxelBounds(Transform tr) {
    		this.tr = tr;
    		Rebuild ();
    	}

    	#region implemented abstract members of AbstractVoxelBounds
    	protected override Vector3 TransformPoint (Vector3 pos) {
    		return tr.TransformPoint (pos);
    	}
        protected override Matrix4x4 LocalToWorldMatrix () {
            return tr.localToWorldMatrix;
        }
    	#endregion

        public override bool IsBoundsChanged () {
            return tr.hasChanged || base.IsBoundsChanged ();
        }
    	protected override void Rebuild() {
    		tr.localScale = Vector3.one;
    		tr.hasChanged = false;
    		base.Rebuild ();
    	}
    }
}
