using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist {

    public class ProgressTimer : Timer {
        protected float progress;
        protected float dprogress;

        public ProgressTimer(float interval) : base(interval) {
        }

        public float Progress { get { return progress; } }

        public override void Start(float interval) {
            base.Start (interval);
            dprogress = 1f / interval;
            progress = 0f;
        }

        protected override void Update(float elapsedTime) {
            progress = Mathf.Clamp01 (elapsedTime * dprogress);
            base.Update (elapsedTime);
        }
    }
}
