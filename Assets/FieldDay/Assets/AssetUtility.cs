using UnityEngine;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace FieldDay.Assets {
    /// <summary>
    /// Asset utility methods.
    /// </summary>
    static public class AssetUtility {
        private delegate bool UnityObjectPredicate(UnityEngine.Object obj);

        static private readonly UnityObjectPredicate Object_IsPersistent;

        static AssetUtility() {
            Object_IsPersistent = (UnityObjectPredicate) typeof(UnityEngine.Object).GetMethod("IsPersistent", BindingFlags.Static | BindingFlags.NonPublic)?.CreateDelegate(typeof(UnityObjectPredicate));
        }

        /// <summary>
        /// Manually unloads the given object.
        /// </summary>
        static public void ManualUnload(UnityEngine.Object obj) {
            if (!ReferenceEquals(obj, null)) {
                bool isPersistent = IsPersistent(obj);
                if (IsPersistent(obj)) {
                    Debug.LogFormat("[AssetUtility] Manually unloading persistent object '{0}'", obj.name);
                    Resources.UnloadAsset(obj);
                } else {
                    Debug.LogFormat("[AssetUtility] Manually unloading object '{0}'", obj.name);
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
                Debug.LogWarningFormat("[AssetUtility] Manually destroying asset '{0}'!", asset.name);
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

        /// <summary>
        /// Returns if the given asset is persistent.
        /// </summary>
        static public bool IsPersistent(UnityEngine.Object obj) {
            if (ReferenceEquals(obj, null)) {
                return false;
            }

            if (Object_IsPersistent != null) {
                return Object_IsPersistent(obj);
            } else {
                return obj.GetInstanceID() > 0; // imperfect check
            }
        }
    }
}