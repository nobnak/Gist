using UnityEngine;



namespace nobnak.Gist.Events {

	[System.Serializable]
	public class BoolEvent : UnityEngine.Events.UnityEvent<bool> { }

    [System.Serializable]
    public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
    [System.Serializable]
    public class Texture2DEvent : UnityEngine.Events.UnityEvent<Texture2D> {}
	[System.Serializable]
	public class RenderTextureEvent : UnityEngine.Events.UnityEvent<RenderTexture> { }
    [System.Serializable]
    public class FloatEvent : UnityEngine.Events.UnityEvent<float> {}
    [System.Serializable]
    public class Vector2Event : UnityEngine.Events.UnityEvent<Vector2> {}
    [System.Serializable]
    public class Vector3Event : UnityEngine.Events.UnityEvent<Vector3> { }

	[System.Serializable]
	public class Matrix4x4Event : UnityEngine.Events.UnityEvent<Matrix4x4> { }
	[System.Serializable]
	public class NamedMatrix4x4Event : UnityEngine.Events.UnityEvent<string, Matrix4x4> { }
}
