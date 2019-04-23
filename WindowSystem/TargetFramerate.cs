using nobnak.Gist.IMGUI.Scope;
using nobnak.Gist.InputDevice;
using nobnak.Gist.Loader;
using UnityEngine;

namespace nobnak.Gist.WindowSystem {

	public class TargetFramerate : MonoBehaviour {
		[SerializeField]
		protected Data data;
		[SerializeField]
		protected FilePath serialized;
		[SerializeField]
		protected KeycodeToggle toggleUI = new KeycodeToggle();
		[SerializeField]
		protected FrameRateJob frameRate;

		protected Rect viewWindow;
		protected Coroutine job;

		protected Reactive<string> reactiveTargetFrameRate = "-1";
		protected Reactive<string> reactiveVSyncCount = "0";

		#region Unity
		protected void Awake() {
			reactiveTargetFrameRate.Changed += v => {
				int iv;
				if (int.TryParse(v.Value, out iv)) {
					iv = (iv > 0 ? iv : -1);
					data.targetFrameRate = iv;
					if (iv == -1)
						reactiveVSyncCount.Value = "0";
				}
			};
			reactiveVSyncCount.Changed += v => {
				int iv;
				if (int.TryParse(v.Value, out iv)) {
					iv = Mathf.Clamp(iv, 0, 4);
					if (iv > 0)
						reactiveTargetFrameRate.Value = "-1";
				}
			};
		}
		protected void OnEnable() {
			viewWindow = new Rect(10, 10, 200, 100);

			serialized.TryLoadOverwrite(ref data);
			ApplyDataToReactive();
			data.Apply();

			job = StartCoroutine(frameRate.GetEnumerator());
		}

		protected void ApplyDataToReactive() {
			reactiveTargetFrameRate.Value = data.targetFrameRate.ToString();
			reactiveVSyncCount.Value = data.vSyncCount.ToString();
		}

		protected void Update() {
			toggleUI.Update();
		}
		protected void OnGUI() {
			if (toggleUI.Visible)
				viewWindow = GUILayout.Window(GetInstanceID(), viewWindow, Window, typeof(TargetFramerate).Name);
		}
		protected void OnDisable() {
			if (job != null) {
				StopCoroutine(job);
				job = null;
			}
		}
		#endregion

		protected void Window(int id) {
			using (new GUILayout.VerticalScope()) {
				GUILayout.Label(frameRate.ToString());

				using (new GUIChangedScope(Changed)) {
					using (new GUILayout.HorizontalScope()) {
						GUILayout.Label("Target frame rate");
						reactiveTargetFrameRate.Value =
							GUILayout.TextField(reactiveTargetFrameRate.Value);
					}
					using (new GUILayout.HorizontalScope()) {
						GUILayout.Label("V-Sync");
						reactiveVSyncCount.Value =
							GUILayout.TextField(reactiveVSyncCount.Value);
					}
				}
			}
			UnityEngine.GUI.DragWindow();
		}
		protected void Changed() {
			serialized.TrySave(data);
			data.Apply();
		}

		[System.Serializable]
		public class Data {
			public int targetFrameRate = -1;
			public int vSyncCount = 0;

			public void Apply() {
				Application.targetFrameRate = (targetFrameRate >= 0 ? targetFrameRate : -1);
				QualitySettings.vSyncCount = Mathf.Clamp(vSyncCount, 0, 4);
			}
		}
	}
}
