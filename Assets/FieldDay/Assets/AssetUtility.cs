using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace FieldDay.Assets {
    /// <summary>
    /// Asset utility methods.
    /// </summary>
    static public class AssetUtility {
        /// <summary>
        /// Manually unloads the given object.
        /// </summary>
        static public void ManualUnload(UnityEngine.Object obj, bool isAsset = false) {
            if (!ReferenceEquals(obj, null)) {
                Debug.LogFormat("[AssetUtility] Manually unloading object '{0}'", obj.name);
                if (isAsset) {
                    Resources.UnloadAsset(obj);
                } else {
#if UNITY_EDITOR
                    UnityEngine.Object.Destroy(obj);
#else
                    UnityEngine.Object.DestroyImmediate(obj, true);
#endif // UNITY_EDITOR
                }
            }
        }

        /// <summary>
        /// Manually destroys the given asset.
        /// Use carefully! In builds you won't get this asset back.
        /// </summary>
        static public void DestroyAsset(UnityEngine.Object asset) {
            if (!ReferenceEquals(asset, null)) {
                Debug.LogWarningFormat("[AssetUtility] Manually destroying asset '{0}'", asset.name);
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(asset, true);
#endif // UNITY_EDITOR
            }
        }

        /// <summary>
        /// Unloads unused assets.
        /// Returns the async operation if asynchronous.
        /// </summary>
        static public AsyncOperation UnloadUnused() {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                EditorUtility.UnloadUnusedAssetsImmediate(true);
                return null;
            }
#endif // UNITY_EDITOR
            return Resources.UnloadUnusedAssets();
        }
    }
}