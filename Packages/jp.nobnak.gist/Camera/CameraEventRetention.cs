using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace nobnak.Gist.Cameras {

	public class CameraEventRetention {

		protected Camera targetCam;

		public CameraEventRetention(Camera targetCam) {
			this.TargetCam = targetCam;
		}

		#region interface
		public Camera TargetCam {
			get => targetCam;
			set {
				Reset();
				targetCam = value;
			}
		}

		public Camera CurrCam { get; protected set; }
		public CommandBuffer CurrCommand { get; protected set; }
		public CameraEvent CurrEvent { get; protected set; } = (CameraEvent)(-1);

		public CameraEventRetention Reset() {
			if (Valid)
				CurrCam.RemoveCommandBuffer(CurrEvent, CurrCommand);
			CurrCam = null;
			CurrEvent = (CameraEvent)(-1);
			CurrCommand = null;
			return this;
		}

		public CameraEventRetention Set(CameraEvent evt, CommandBuffer cmd) {
			Reset();
			CurrCam = targetCam;
			CurrEvent = evt;
			CurrCommand = cmd;
			if (Valid)
				CurrCam.AddCommandBuffer(evt, cmd);
			return this;
		}
		#endregion

		#region member
		protected virtual bool Valid => CurrCam != null && CurrEvent >= 0 && CurrCommand != null;
		#endregion
	}
}
