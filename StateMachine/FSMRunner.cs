using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gist.StateMachine {

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
    
    public class FSM<T> : System.IDisposable, IFSM where T : struct {
        protected Dictionary<T, StateData> _stateMap = new Dictionary<T, StateData>();
        protected FSMRunner _runner;
        
        protected StateData _current;
        protected StateData _last;

        public FSM(MonoBehaviour target) {
            if ((_runner = target.GetComponent<FSMRunner> ()) == null)
                _runner = target.gameObject.AddComponent<FSMRunner> ();
            _runner.Add (this);
        }

        public StateData State(T name) {
            StateData state;
            if (!TryGetState (name, out state))
                state = _stateMap [name] = new StateData (name);
            return state;
        }
        public T Current { get { return (_current == null ? default(T) : _current.name); } }
        public T Last { get { return (_last == null ? default(T) : _last.name); } }

        public FSM<T> Goto(T nextStateName) {
            StateData next;
            if (!TryGetState (nextStateName, out next) || next == null) {
                Debug.LogWarningFormat ("There is no state {0}", nextStateName);
                return this;
            }
            if (next.name.Equals(nextStateName))
                return this;

            _last = _current;
            _current = next;
            if (_last != null)
                _last.ExitState (this);
            _current.EnterState (this);
            return this;
        }
        public void Update() {
            if (_current != null) {
                _current.UpdateState(this);
            }
        }
        public bool TryGetState(T name, out StateData state) {
            return _stateMap.TryGetValue (name, out state);
        }

        #region IDisposable implementation
        public void Dispose () {
            if (_runner != null) {
                _runner.Remove (this);
                _runner = null;
            }
        }
        #endregion

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
    }
}
