using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace nobnak.Gist.Extensions.EditorExt {

    public static class EditorExtension {
		public const string T_GameView = "UnityEditor.GameView,UnityEditor";

		public const string P_LOWRES = "lowResolutionForAspectRatios";
		public const string P_FORCE_LOWRES = "forceLowResolutionAspectRatios";

#if UNITY_EDITOR
		public static readonly System.Type TYPE_GAME_VIEW = System.Type.GetType(T_GameView);
		
		static EditorWindow gameView;
        static System.Reflection.PropertyInfo propLowRes;
		static System.Reflection.PropertyInfo propForce;

		public static EditorWindow GameView {
            get {
                if (gameView == null) gameView = EditorWindow.GetWindow(TYPE_GAME_VIEW);
                return gameView;
            }
        }
        public static System.Reflection.PropertyInfo LowResolutionAcpectRatioPropertyOfGameView {
            get {
                if (propLowRes == null) propLowRes = TYPE_GAME_VIEW.GetProperty(P_LOWRES);
                return propLowRes;
            }
        }
		public static System.Reflection.PropertyInfo ForceLowResolutionAspectRatios {
			get {
				if (propForce == null) propForce = TYPE_GAME_VIEW.GetProperty(P_FORCE_LOWRES);
				return propForce;
			}
		}
#endif

		public static bool LowResolutionAspectRatio {
            get {
#if UNITY_EDITOR
				return (bool)LowResolutionAcpectRatioPropertyOfGameView.GetValue(GameView)
					&& !(bool)ForceLowResolutionAspectRatios.GetValue(GameView);
#else
                return false;
#endif
            }
        }        
    }
}
