using UnityEngine;



namespace Gist.Events {

    [System.Serializable]
    public class TextureEvent : UnityEngine.Events.UnityEvent<Texture> {}
    [System.Serializable]
    public class Vector3Event : UnityEngine.Events.UnityEvent<Vector3> {}
}