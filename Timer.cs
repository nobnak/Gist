using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gist { 

    public class Timer {
        public enum StateEnum { Init = 0, Active, Completed }

        public event System.Action<Timer> Elapsed;

        protected bool active;
        protected bool completed;

        protected float interval;
        protected float timeOfStart;

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

        public virtual void Start() {
            Start (interval);
        }
        public virtual void Start(float interval) {
            Goto (StateEnum.Active);
            Interval = interval;
            timeOfStart = Time.timeSinceLevelLoad;
        }
        public virtual void Update() {
            Update (Time.timeSinceLevelLoad - timeOfStart);
        }
        public virtual void Stop() {
            Goto (StateEnum.Init);
        }

        protected virtual void Update(float elapsedTime) {
            if (active && interval <= elapsedTime) {
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
    }
}
