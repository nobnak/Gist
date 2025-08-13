//#define VERBOSE

using System.Collections.Generic;
using System.Text;
using System.Threading;
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
        Dictionary<T, State> _stateMap = new Dictionary<T, State>();

        bool _enabled;
        FSMRunner _runner;
		int _lock = 0;

        State _current;
        State _last;

		StateData _currentData;
		StateData _lastData;

        T _lastQueuedStateName;
        Queue<StateData> stateQueue;

        public FSM(MonoBehaviour target, TransitionModeEnum transitionMode) : base(transitionMode) {
            if (target != null) {
                if ((_runner = target.GetComponent<FSMRunner>()) == null) {
                    _runner = target.gameObject.AddComponent<FSMRunner>();
                    _runner.hideFlags = HideFlags.DontSaveInEditor;
                }
                _runner.Add(this);
            }
            _enabled = true;
            this.stateQueue = new Queue<StateData>();
        }
        public FSM(MonoBehaviour target) : this(target, TransitionModeEnum.Queued) { }
        public FSM(TransitionModeEnum transitionMode) : this(null, transitionMode) { }
        public FSM() : this(null) { }

		public State StateFor(T name) {
            State state;
            if (!TryGetState (name, out state))
                state = _stateMap [name] = new State (name);
            return state;
        }

		public T Current { get { return (_current == null ? default(T) : _current.name); } }
        public T Last { get { return (_last == null ? default(T) : _last.name); } }

		public object[] CurrentData { get { return _currentData.reason; } }
		public object[] LastData { get { return _lastData.reason; } }

        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public FSM<T> Init(T initialStateName = default(T)) {
			_Goto(new StateData(initialStateName));
			return this;
        }
        public FSM<T> Goto(T nextStateName, params object[] reason) {
			Enqueue(nextStateName, reason);

			switch (transitionMode) {
				case TransitionModeEnum.Immediate:
					_GotoInQueue();
					break;
			}

			return this;
		}
		public FSM<T> GotoQueued(T nextStateName, params object[] reason) {
			Enqueue(nextStateName, reason);
			return this;
        }

		public FSM<T> GotoImmediate(T nextStateName, params object[] reason) {
			Enqueue(nextStateName, reason);
			_GotoInQueue();
            return this;
        }

        public override void Update() {
            if (!_enabled)
                return;

            _GotoInQueue();

			if (_current != null) {
				try {
					if (Interlocked.Increment(ref _lock) == 1)
						_current.UpdateState(this);
				} finally {
					Interlocked.Decrement(ref _lock);
				}
			}

			_GotoInQueue();
		}

        public bool TryGetState(T name, out State state) {
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

		protected void Enqueue(T nextStateName, object[] reason) {
			if (!EqualsToLastQueued(nextStateName))
				_Enqueue(new StateData(nextStateName, reason));
		}
		protected void _Goto(StateData nextData) {
            State next;
            if (!TryGetState(nextData, out next) || next == null) {
                Debug.LogWarningFormat("There is no state {0}", nextData.state);
                return;
            }

			if (_current == next)
				Debug.LogFormat("State already in {0}", next);

			if (_current != null)
				_current.ExitState(this);

			_last = _current;
            _current = next;

			_lastData = _currentData;
			_currentData = nextData;

            _current.EnterState(this);
            return;
        }

		protected void _GotoInQueue() {
			try {
				if (Interlocked.Increment(ref _lock) == 1) {
					while (stateQueue.Count > 0) {
						var next = stateQueue.Dequeue();
						_Goto(next);
					}
				}
			} finally {
				Interlocked.Decrement(ref _lock);
			}
        }
        protected void _Enqueue(StateData next) {
            _lastQueuedStateName = next;
            stateQueue.Enqueue(next);
        }
        protected bool TryGetLastFromQueue(out T last) {
            last = default(T);

            var result = stateQueue.Count > 0;
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
			tmp.AppendFormat("count={0} ", stateQueue.Count);
			if (_current != null)
				tmp.AppendFormat("{0}", _current.name);
			foreach (var s in stateQueue)
				tmp.AppendFormat("->{0}", s);
			var log = tmp.ToString();
			return log;
		}

		#region Classes
		public class State {
            public readonly T name;

            System.Action<FSM<T>> _enter;
            System.Action<FSM<T>> _update;
            System.Action<FSM<T>> _exit;

            public State(T name) {
                this.name = name;
            }

            public State Enter(System.Action<FSM<T>> enter) {
                this._enter = enter;
                return this;
            }
            public State Update(System.Action<FSM<T>> update) {
                this._update = update;
                return this;
            }
            public State Exit(System.Action<FSM<T>> exit) {
                this._exit = exit;
                return this;
            }

            public State EnterState(FSM<T> fsm) {
                if (_enter != null)
                    _enter (fsm);
                return this;
            }
            public State UpdateState(FSM<T> fsm) {
                if (_update != null)
                    _update (fsm);
                return this;
            }
            public State ExitState(FSM<T> fsm) {
                if (_exit != null)
                    _exit (fsm);
                return this;
            }
        }

		public struct StateData {
			public readonly T state;
			public readonly object[] reason;

			public StateData(T state, params object[] reason) {
				this.state = state;
				this.reason = reason;
			}

			public override string ToString() {
				var tmp = new StringBuilder("<StateData:");
				foreach (var r in reason)
					tmp.AppendFormat("{0},", r);
				tmp.Append(">");
				return tmp.ToString();
			}

			public static implicit operator T(StateData data) {
				return data.state;
			}
		}
#endregion
    }
}
