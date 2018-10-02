using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    #region Timer class
    public class Timer {
        public enum StateEnum { Init = 0, Active, Completed }

        public event System.Action<Timer> Elapsed;

        protected bool active;
        protected bool completed;

        protected float interval;
        protected float timeOfStart;
        protected float elapsedTime;

		#region interface
		public Timer(float interval, StateEnum initState) {
			Goto(initState);
			Interval = interval;
		}
		public Timer(float interval) : this(interval, StateEnum.Init) {}

		public virtual float Interval {
            get { return interval; }
            set {
                interval = value;
                if (interval < 0f)
                    Goto (StateEnum.Init);
            }
        }
        public virtual bool Active { get { return active; } }
        public virtual bool Completed { get { return completed; } }
        public virtual float ElapsedTime { get { return elapsedTime; } }

        public virtual void Start() {
            Start (interval);
        }
        public virtual void Start(float interval) {
            Goto (StateEnum.Active);
            Interval = interval;
            timeOfStart = Time.timeSinceLevelLoad;
            elapsedTime = 0f;
        }
        public virtual bool Update() {
            var currActivity = active;
            if (active) {
                elapsedTime = CalculateElapsedTime ();
				if (interval <= elapsedTime) {
					Goto(StateEnum.Completed);
					NotifyElapsed();
				}
			}
            return currActivity;
        }
        public virtual void Stop() {
            Goto (StateEnum.Init);
        }
		#endregion

		#region private
		protected virtual void Goto(StateEnum state) {
            switch (state) {
            case StateEnum.Init:
                Goto_Init ();
                break;
            case StateEnum.Active:
                Goto_Active ();
                break;
            case StateEnum.Completed:
                Goto_Completed ();
                break;
            }
        }
        protected virtual void Goto_Init() {
            active = false;
            completed = false;
        }
        protected virtual void Goto_Active() {
            active = true;
            completed = false;
        }
        protected virtual void Goto_Completed() {
            active = false;
            completed = true;
        }

        protected virtual void NotifyElapsed () {
            if (Elapsed != null)
                Elapsed (this);
        }

        protected virtual float CalculateElapsedTime () {
            return Time.timeSinceLevelLoad - timeOfStart;
        }
		#endregion

	}
	#endregion

	#region Progress Timer class
	public class ProgressTimer : Timer {
        public event System.Action<ProgressTimer, float> ProgressChanged;

        protected float progress;
        protected float dProgressDTime;

		#region interface
		public ProgressTimer(float interval) : base(interval) {
        }

        public float Progress { get { return progress; } }

        public override void Start(float interval) {
            base.Start (interval);
            dProgressDTime = 1f / interval;
            progress = 0f;
        }

        public override bool Update() {
			var prevProgress = progress;
			var result = base.Update();
			progress = dProgressDTime * elapsedTime;
			var dprogress = progress - prevProgress;
            NotifyProgressChanged (dprogress);
			return result;
        }
		#endregion

		#region private
		void NotifyProgressChanged(float delta) {
            if (ProgressChanged != null)
                ProgressChanged (this, delta);
        }

        float CalculateProgress (float dtime, out float dprogress) {
            dprogress = dtime * dProgressDTime;
            return Mathf.Clamp01 (elapsedTime * dProgressDTime);
        }
		#endregion
	}
	#endregion
}
