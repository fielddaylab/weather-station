#if (UNITY_EDITOR && !IGNORE_UNITY_EDITOR) || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif // (UNITY_EDITOR && !IGNORE_UNITY_EDITOR) || DEVELOPMENT_BUILD

using System.Collections;
using BeauUtil;
using ScriptableBake;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using BeauUtil.Debugger;
using BeauRoutine;
using BeauPools;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif // UNITY_EDITOR

namespace FieldDay.Scenes {
    public sealed class SceneMgr {
        #region Types

        private class LoadProcess {
            public SceneReference MainLoad;
            public readonly RingBuffer<SceneImportSettings> ImportQueue = new RingBuffer<SceneImportSettings>();
            public readonly RingBuffer<SceneDataExt> AllScenes = new RingBuffer<SceneDataExt>();

            public readonly PreloadManifest.Reader PreloadReader = new PreloadManifest.Reader();
            public WorkSlicer.EnumeratedState PreloadState;
            public readonly RingBuffer<IScenePreload> PreloadQueue = new RingBuffer<IScenePreload>();
        }

        private class UnloadProcess {
            public readonly RingBuffer<SceneBinding> UnloadQueue = new RingBuffer<SceneBinding>();
        }

        #region Operations

        private struct LateEnableOperation
        {
            public GameObject[] Objects;
            public CounterHandle Counter;
        }

        #endregion // Operations

        private struct QueuedLoadItem {
            public SceneBinding Scene;
            public SceneLoadType LoadType;
            public StringHash32 Tag;
            public AsyncOperation Operation;
            public CounterHandle Counter;
        }

        private struct QueuedUnloadItem {
            public SceneBinding Scene;
            public SceneLoadType LoadType;
            public AsyncOperation Operation;
            public CounterHandle Counter;
        }

        public enum SceneLoadPhase {
            BeforeActivation,
            Awake,
            Ready
        }

        public struct SceneArgs {
            public SceneBinding Scene;
            public SceneLoadType LoadType;
        }

        private struct SceneSlot {
            public SceneBinding Scene;
            public StringHash32 Tag;
        }

        #endregion // Types

        #region State

        // current state
        private SceneBinding m_MainScene;
        private readonly LLTable<SceneSlot> m_SceneSlots = new LLTable<SceneSlot>(32);
        private LLIndexList m_AuxSlotList = LLIndexList.Empty;
        private LLIndexList m_PersistentSlotList = LLIndexList.Empty;

        // queues
        private readonly RingBuffer<LateEnableOperation> m_QueuedLateEnable = new RingBuffer<LateEnableOperation>(64, RingBufferMode.Expand);

        // resources
        private readonly IPool<LoadProcess> m_LoadProcessPool = new FixedPool<LoadProcess>(4, Pool.DefaultConstructor<LoadProcess>());

        #endregion // State

        #region Exposed Events

        public readonly CastableEvent<SceneArgs> OnPrepareScene = new CastableEvent<SceneArgs>();
        public readonly CastableEvent<SceneArgs> OnSceneAwake = new CastableEvent<SceneArgs>();
        public readonly CastableEvent<SceneArgs> OnSceneReady = new CastableEvent<SceneArgs>();

        #endregion // Exposed Events

        internal SceneMgr() {
        }

        #region Public API

        public void LoadMainScene(SceneReference scene) {

        }

        public void LoadAuxScene(SceneReference scene, Matrix4x4? transformBy = null) {
            LoadAuxScene(scene, null, transformBy);
        }

        public void LoadAuxScene(SceneReference scene, StringHash32 tag, Matrix4x4? transformBy = null) {

        }

        public void LoadPersistentScene(SceneReference reference) {
            LoadPersistentScene(reference, null);
        }

        public void LoadPersistentScene(SceneReference reference, StringHash32 tag) {

        }

        public void AddScene(SceneImportSettings importSettings) {

        }

        public void UnloadScene(SceneReference reference) {

        }

        public void UnloadScenesByTag(StringHash32 tag) {

        }

        #endregion // Public API

        #region Events

        internal void Update() {
            //if (UpdateSceneLoadQueue()) {
            //    return;
            //}

            //if (UpdateSceneUnloadQueue()) {
            //    return;
            //}
        }

        internal void Shutdown() {
        }

        #endregion // Events

        #region Internal

//        private bool UpdateSceneLoadQueue() {
//            if (m_LoadQueue.Count <= 0) {
//                return false;
//            }
//            ref QueuedLoadItem data = ref m_LoadQueue[0];
//            if (data.Operation == null) {
//                switch (data.LoadType) {
//                    case SceneLoadType.Main: {
//                        m_MainScene = data.Scene;
//                        break;
//                    }

//                    case SceneLoadType.Aux: {
//                        AddToSlotList(m_SceneSlots, ref m_AuxSlotList, data.Scene, data.Tag);
//                        break;
//                    }

//                    case SceneLoadType.Persistent: {
//                        AddToSlotList(m_SceneSlots, ref m_PersistentSlotList, data.Scene, data.Tag);
//                        break;
//                    }
//                }

//                Scene unityScene = data.Scene.Scene;
//#if UNITY_EDITOR
//                if (IsLoadingOrLoaded(unityScene)) {

//                }
//#endif // UNITY_EDITOR
//                data.Operation = SceneManager.UnloadSceneAsync(data.Scene.Scene);
//                return true;
//            } else {
//                if (data.Operation.isDone) {
//                    data.Counter.Decrement();
//                    m_LoadQueue.PopFront();
//                    return true;
//                }
//            }

//            return false;
//        }

        //private bool UpdateSceneUnloadQueue() {
        //    if (m_UnloadQueue.Count <= 0) {
        //        return false;
        //    }

        //    ref QueuedUnloadItem data = ref m_UnloadQueue[0];
        //    if (data.Operation == null) {
        //        switch (data.LoadType) {
        //            case SceneLoadType.Main: {
        //                m_MainScene = default;
        //                break;
        //            }

        //            case SceneLoadType.Aux: {
        //                RemoveFromSlotList(m_SceneSlots, ref m_AuxSlotList, data.Scene);
        //                break;
        //            }

        //            case SceneLoadType.Persistent: {
        //                RemoveFromSlotList(m_SceneSlots, ref m_PersistentSlotList, data.Scene);
        //                break;
        //            }
        //        }
        //        data.Scene.BroadcastUnload();
        //        data.Operation = SceneManager.UnloadSceneAsync(data.Scene.Scene);
        //        return true;
        //    } else {
        //        if (data.Operation.isDone) {
        //            data.Counter.Decrement();
        //            m_UnloadQueue.PopFront();
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        static internal readonly Predicate<SceneBinding, Scene> FindBindingByScene = (a, b) => a.Scene == b;

        private IEnumerator ImportAdditiveScene(SceneImportSettings scene, SceneLoadType loadType, CounterHandle dependentCounter) {
            Assert.True(loadType != SceneLoadType.Main);

            AsyncOperation op;
#if !UNITY_EDITOR
            Scene editorScene = EditorSceneManager.GetSceneByPath(scene.Path);
            if (!editorScene.isLoaded) {
                op = EditorSceneManager.LoadSceneAsyncInPlayMode(scene.Path, new LoadSceneParameters(LoadSceneMode.Additive));
            } else {
                op = null;
            }
#else
            op = SceneManager.LoadSceneAsync(scene.Path, LoadSceneMode.Additive);
#endif // UNITY_EDITOR

            op.allowSceneActivation = false;

            while (op.progress < 0.9f) {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByPath(scene.Path);

            OnPrepareScene.Invoke(new SceneArgs() {
                Scene = loadedScene,
                LoadType = loadType,
                //Phase = SceneLoadPhase.BeforeActivation,
            });

            yield return null;

            op.allowSceneActivation = true;

            while (!op.isDone) {
                yield return null;
            }

            OnSceneAwake.Invoke(new SceneArgs() {
                Scene = loadedScene,
                LoadType = loadType,
                //Phase = SceneLoadPhase.Awake
            });

            yield return null;
        }

        private void ProcessSubScenes(SceneDataExt ext, CounterHandle waitCounter) {
            for (int i = 0, len = ext.SubScenes.Length; i < len; i++) {
                SceneImportSettings importSettings = ext.SubScenes[i].GetImportSettings();
                // TODO: Queue a scene
                waitCounter.Increment();
            }
        }

        private void ProcessDynamicSubScenes(SceneDataExt ext, CounterHandle waitCounter) {
            for (int i = 0, len = ext.DynamicSubscenes.Length; i < len; i++) {
                foreach (var importSettings in ((IDynamicSceneImport) ext.DynamicSubscenes[i]).GetSubscenes()) {
                    // TODO: Queue a scene
                    waitCounter.Increment();
                }
            }
        }

        #endregion // Internal

        #region Slots

        static private void AddToSlotList(LLTable<SceneSlot> table, ref LLIndexList list, SceneBinding scene, StringHash32 tag) {
            table.PushBack(ref list, new SceneSlot() { Scene = scene, Tag = tag });
        }

        static private void RemoveFromSlotList(LLTable<SceneSlot> table, ref LLIndexList list, SceneBinding scene) {
            var enumerator = table.GetEnumerator(list);
            while(enumerator.MoveNext()) {
                if (enumerator.Current.Tag.Scene == scene) {
                    table.Remove(ref list, enumerator.Current.Index);
                    return;
                }
            }
        }

        #endregion // Slots

        #region Is Loaded

        static private bool IsLoadingOrLoaded(Scene scene) {
            SceneHelper.LoadingState loadingState = scene.GetLoadingState();
            return loadingState == SceneHelper.LoadingState.Loading || loadingState == SceneHelper.LoadingState.Loaded;
        }

        #endregion // Is Loaded

        #region Micro Operations



        #endregion // Micro Operations

        #region Routines

        //private IEnumerator LoadSequence(SceneReference scene) {

        //}

        #endregion // Routines
    }

    /// <summary>
    /// Types of scene load.
    /// </summary>
    public enum SceneLoadType : byte {
        Main,
        Aux,
        Persistent
    }

    public enum SceneLoadPriority : byte {
        High,
        Medium,
        Low
    }
}