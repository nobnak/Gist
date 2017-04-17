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
        protected float timeOfElapsed;

        public Timer(float interval) {
            Goto (StateEnum.Init);
            Interval = interval;
        }

        public float Interval {
            get { return interval; }
            set {
                interval = value;
                if (interval < 0f)
                    Goto (StateEnum.Init);
            }
        }
        public bool Active { get { return active; } }
        public bool Completed { get { return completed; } }

        public void Start() {
            Start (interval);
        }
        public void Start(float interval) {
            Goto (StateEnum.Active);
            Interval = interval;
            timeOfElapsed = Time.timeSinceLevelLoad + interval;
        }
        public void Update() {
            var currentTime = Time.timeSinceLevelLoad;
            if (active && timeOfElapsed <= currentTime) {
                Goto (StateEnum.Completed);
                NotifyElapsed ();
            }
        }
        public void Stop() {
            Goto (StateEnum.Init);
        }

        void Goto(StateEnum state) {
            switch (state) {
            case StateEnum.Init:
                active = false;
                completed = false;
                break;
            case StateEnum.Active:
                active = true;
                completed = false;
                break;
            case StateEnum.Completed:
                active = false;
                completed = true;
                break;
            }
        }
        void NotifyElapsed () {
            if (Elapsed != null)
                Elapsed (this);
        }
    }
}
