using UnityEngine;
using BeauUtil;
#if UNITY_EDITOR

#endif // UNITY_EDITOR

namespace WeatherStation
{
    [RequireComponent(typeof(Canvas)), ExecuteAlways]
    public class AttachToUICamera : MonoBehaviour
    {
        private void OnEnable() 
        {
            #if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null)
                return;
            #endif // UNITY_EDITOR

            if (TransformHelper.TryGetCameraFromLayer(transform, out Camera camera))
                GetComponent<Canvas>().worldCamera = camera;
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null)
                return;
            #endif // UNITY_EDITOR

            GetComponent<Canvas>().worldCamera = null;
        }

    }
}