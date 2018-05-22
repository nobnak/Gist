//#define VERBOSE

using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace nobnak.Gist.StateMachine {

    public class FSMRunner : MonoBehaviour {
        List<IFSM> _fsmlist = new List<IFSM>();

        protected FSMRunner() {}

        public FSMRunner Add(IFSM fsm){
            _fsmlist.Add (fsm);
            return this;
        }
        public FSMRunner Remove(IFSM fsm) {
            _fsmlist.Remove (fsm);
            return this;
        }

        void Update() {
            foreach (var fsm in _fsmlist)
                if (fsm != null)
                    fsm.Update ();
        }
    }

    public interface IFSM {
        void Update();
    }
    public abstract class FSM : System.IDisposable, IFSM {
        public enum TransitionModeEnum { Queued = 0, Immediate }

        protected TransitionModeEnum transitionMode;

        public FSM(TransitionModeEnum transitionMode) {
            this.transitionMode = transitionMode;
        }

        public abstract void Dispose();
        public abstract void Update();
    }
    public class FSM<T> : FSM where T : struct, System.IComparable {
        Dictionary<T, StateData> _stateMap = new Dictionary<T, StateData>();

        bool _enabled;
        FSMRunner _runner;
        StateData _current;
        StateData _last;

        bool _queueInProcess;
        T _lastQueuedStateName;
        Queue<T> nextStateNameQueue;

        public FSM(MonoBehaviour target, TransitionModeEnum transitionMode) : base(transitionMode) {
            if (target != null) {
                if ((_runner = target.GetComponent<FSMRunner>()) == null) {
                    _runner = target.gameObject.AddComponent<FSMRunner>();
                    _runner.hideFlags = HideFlags.DontSaveInEditor;
                }
                _runner.Add(this);
            }
            _enabled = true;
            this.nextStateNameQueue = new Queue<T>();
        }
        public FSM(MonoBehaviour target) : this(target, TransitionModeEnum.Queued) { }
        public FSM(TransitionModeEnum transitionMode) : this(null, transitionMode) { }
        public FSM() : this(null) { }

            public StateData State(T name) {
            StateData state;
            if (!TryGetState (name, out state))
                state = _stateMap [name] = new StateData (name);
            return state;
        }
        public T Current { get { return (_current == null ? default(T) : _current.name); } }
        public T Last { get { return (_last == null ? default(T) : _last.name); } }
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public FSM<T> Init(T initialStateName = default(T)) {
            return GotoImmediate(initialStateName);
        }
        public FSM<T> Goto(T nextStateName) {
            switch (transitionMode) {
                default:
                    return GotoQueued(nextStateName);
                case TransitionModeEnum.Immediate:
                    return GotoImmediate(nextStateName);
            }
        }
        public FSM<T> GotoQueued(T nextStateName) {
			if (!EqualsToLastQueued(nextStateName)) {
				Enqueue(nextStateName);
			}

			return this;
        }

		public FSM<T> GotoImmediate(T nextStateName) {
			if (!EqualsToCurrent(nextStateName))
				_Goto(nextStateName);
            return this;
        }

        public override void Update() {
            if (!_enabled)
                return;

            _GotoInQueue();

            if (_current != null)
                _current.UpdateState(this);
        }

        public bool TryGetState(T name, out StateData state) {
            return _stateMap.TryGetValue (name, out state);
        }

#region IDisposable implementation
        public override void Dispose () {
            if (_runner != null) {
                _runner.Remove (this);
                _runner = null;
            }
        }
#endregion

        protected void _Goto(T nextStateName) {
            StateData next;
            if (!TryGetState(nextStateName, out next) || next == null) {
                Debug.LogWarningFormat("There is no state {0}", nextStateName);
                return;
            }
			if (_current == next)
				Debug.LogFormat("State already in {0}", next);
            _last = _current;
            _current = next;
            if (_last != null)
                _last.ExitState(this);
            _current.EnterState(this);
            return;
        }
        protected void _GotoInQueue() {
            if (_queueInProcess)
                return;
            _queueInProcess = true;

            while (nextStateNameQueue.Count > 0) {
                var next = nextStateNameQueue.Dequeue();
                _Goto(next);
            }

            _queueInProcess = false;
        }
        protected void Enqueue(T nextStateName) {
            _lastQueuedStateName = nextStateName;
            nextStateNameQueue.Enqueue(nextStateName);
        }
        protected bool TryGetLastFromQueue(out T last) {
            last = default(T);

            var result = nextStateNameQueue.Count > 0;
            if (result)
                last = _lastQueuedStateName;
            return result;
        }
		protected bool EqualsToCurrent(T other) {
			return _current != null && EqualityComparer<T>.Default.Equals(_current.name, other);
		}
		protected bool EqualsToLastQueued(T other) {
			T last;
			return TryGetLastFromQueue(out last) ?
				EqualityComparer<T>.Default.Equals(last, other) : 
				EqualsToCurrent(other);
		}
		protected string QueueToString() {
			var tmp = new StringBuilder("FSM State Queue : ");
			tmp.AppendFormat("count={0} ", nextStateNameQueue.Count);
			if (_current != null)
				tmp.AppendFormat("{0}", _current.name);
			foreach (var s in nextStateNameQueue)
				tmp.AppendFormat("->{0}", s);
			var log = tmp.ToString();
			return log;
		}

		#region Classes
		public class StateData { 
            public readonly T name;

            System.Action<FSM<T>> _enter;
            System.Action<FSM<T>> _update;
            System.Action<FSM<T>> _exit;

            public StateData(T name) {
                this.name = name;
            }

            public StateData Enter(System.Action<FSM<T>> enter) {
                this._enter = enter;
                return this;
            }
            public StateData Update(System.Action<FSM<T>> update) {
                this._update = update;
                return this;
            }
            public StateData Exit(System.Action<FSM<T>> exit) {
                this._exit = exit;
                return this;
            }

            public StateData EnterState(FSM<T> fsm) {
                if (_enter != null)
                    _enter (fsm);
                return this;
            }
            public StateData UpdateState(FSM<T> fsm) {
                if (_update != null)
                    _update (fsm);
                return this;
            }
            public StateData ExitState(FSM<T> fsm) {
                if (_exit != null)
                    _exit (fsm);
                return this;
            }
        }
#endregion
    }
}
