using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    public class Checkin {
        protected int initCounter;
        protected int currCounter;
        public event System.Action OnConsume;
        public event System.Action OnRelease;

        public Checkin(int initCounter) {
            this.initCounter = Mathf.Max(1, initCounter);
            Reset ();
        }

        public void Reset() {
            currCounter = initCounter;
        }
        public bool Consume() {
            if (currCounter > 0) {
                --currCounter;
                NotifyOnConsume ();
                return true;
            }
            return false;
        }
        public bool Release() {
            if (currCounter < initCounter) {
                ++currCounter;
                NotifyOnRelease ();
                return true;
            }
            return false;
        }

        void NotifyOnConsume() {
            if (OnConsume != null)
                OnConsume ();
        }
        void NotifyOnRelease() {
            if (OnRelease != null)
                OnRelease ();
        }
    }
}
