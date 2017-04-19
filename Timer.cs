using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist { 

    #region Timer class
    public class Timer {
        public enum StateEnum { Init = 0, Active, Completed }

        public event System.Action<Timer> Elapsed;
        public event System.Action<Timer, float> TimeChanged;

        protected bool active;
        protected bool completed;

        protected float interval;
        protected float timeOfStart;
        protected float elapsedTime;

        public Timer(float interval) {
            Goto (StateEnum.Init);
            Interval = interval;
        }

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
        public virtual void Update() {
            if (active) {
                float dtime;
                elapsedTime = CalculateElapsedTime (out dtime);
                Update (dtime);
            }
        }
        public virtual void Stop() {
            Goto (StateEnum.Init);
        }

        protected virtual void Update(float dtime) {
            NotifyTimeChanged (dtime);
            if (interval <= elapsedTime) {
                Goto (StateEnum.Completed);
                NotifyElapsed ();
            }
        }
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
        protected virtual void NotifyTimeChanged(float dtime) {
            if (TimeChanged != null)
                TimeChanged (this, dtime);
        }

        protected virtual float CalculateElapsedTime (out float dtime) {
            var nextElapsedTime = Time.timeSinceLevelLoad - timeOfStart;
            dtime = nextElapsedTime - elapsedTime;
            return nextElapsedTime;
        }

    }
    #endregion

    #region Progress Timer class
    public class ProgressTimer : Timer {
        public event System.Action<ProgressTimer, float> ProgressChanged;

        protected float progress;
        protected float dProgressDTime;

        public ProgressTimer(float interval) : base(interval) {
        }

        public float Progress { get { return progress; } }

        public override void Start(float interval) {
            base.Start (interval);
            dProgressDTime = 1f / interval;
            progress = 0f;
        }

        protected override void Update(float dtime) {
            float dprogress;
            progress = CalculateProgress (dtime, out dprogress);
            NotifyProgressChanged (dprogress);
            base.Update (elapsedTime);
        }

        void NotifyProgressChanged(float delta) {
            if (ProgressChanged != null)
                ProgressChanged (this, delta);
        }

        float CalculateProgress (float dtime, out float dprogress) {
            dprogress = dtime * dProgressDTime;
            return Mathf.Clamp01 (elapsedTime * dProgressDTime);
        }
    }
    #endregion
}
