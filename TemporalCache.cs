using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {

    public class TemporalCache<V> {

        protected float cacheDuration;
        protected float valueGeneratedTime;
        protected V value;

        public TemporalCache(float cacheDuration) {
            this.valueGeneratedTime = float.MinValue;
            this.cacheDuration = cacheDuration;
        }

        public V Wrap(V nextValue) {
            if (!ValueIsEffective)
                SetValue(nextValue);
            return value;
        }

        protected bool ValueIsEffective {
            get { return (valueGeneratedTime + cacheDuration) >= Time.time; }
        }
        protected void SetValue(V value) {
            this.value = value;
            this.valueGeneratedTime = Time.time;
        }
    }
}