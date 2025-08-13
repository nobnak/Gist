namespace nobnak.Gist.IMGUI.Scope {

	public class GUIChangedScope : System.IDisposable {
        protected bool initialState;
        protected System.Action callIfChnaged;

        public GUIChangedScope(System.Action callIfChnaged) {
            this.callIfChnaged = callIfChnaged;
            initialState = UnityEngine.GUI.changed;
			UnityEngine.GUI.changed = false;
        }

        public void Dispose() {
            if (UnityEngine.GUI.changed)
                callIfChnaged();
			UnityEngine.GUI.changed = UnityEngine.GUI.changed || initialState;
        }
    }
}