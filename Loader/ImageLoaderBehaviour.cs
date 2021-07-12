using nobnak.Gist.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Loader {

    [ExecuteAlways]
	public class ImageLoaderBehaviour : MonoBehaviour {

		[SerializeField]
		protected Texture2DEvent Changed = new Texture2DEvent();

        [SerializeField]
        protected ImageLoader loader = new ImageLoader();

		#region Unity
		protected void OnEnable() {
			loader.Changed += ListenOnChanged;

			ListenOnChanged(loader.Target);
		}
		protected void Update() {
			loader.Validate();
		}
		protected void OnDisable() {
			loader.Changed -= ListenOnChanged;
			loader.Dispose();
		}
		#endregion

		protected void ListenOnChanged(Texture2D tex) {
			Changed.Invoke(tex);
		}
	}
}
