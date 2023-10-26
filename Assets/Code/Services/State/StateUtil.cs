using System.Collections;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Services;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation {
    static public class StateUtil {
        private const SceneLoadFlags LoadFlags = SceneLoadFlags.DoNotDispatchPreUnload;

        private const float DefaultFadeDuration = 0.25f;
        private const float PauseDuration = 0.15f;
        private const string DefaultBackScene = "Helm";

        static private SceneLoadFlags s_LastFlags;
        static private bool s_IsLoading = false;

        static private ScreenFaderDisplay screenFaderDisplay = null;

        static public bool IsLoading {
            get { return s_IsLoading || (Services.Valid && Services.State.IsLoadingScene()); }
        }

        /*static public void InitFaders() {
            Debug.Log("In ALLOC");
            screenFaderDisplay = new ScreenFaderDisplay();
        }*/

        static public IEnumerator SwapSceneWithFader(string oldSceneName, string newSceneName, object inContext = null, SceneLoadFlags inFlags = SceneLoadFlags.Default, float inFadeDuration = DefaultFadeDuration) {
            inFlags |= LoadFlags;
            
            if (!BeforeLoad(inFlags, inFadeDuration)) {
                return null;
            }

            return Sequence.Create(Services.State.SwapCurrentScene(oldSceneName, newSceneName, inFadeDuration)).Then(() => AfterLoad(inFadeDuration));

            /*return screenFaderDisplay.FadeTransition(Color.black, inFadeDuration, PauseDuration,
                () => Sequence.Create(Services.State.SwapCurrentScene(oldSceneName, newSceneName)).Then(() => AfterLoad(inFadeDuration))
            );*/
        }

        static public IEnumerator LoadSceneWithFader(string inSceneName, StringHash32 inEntrance = default(StringHash32), object inContext = null, SceneLoadFlags inFlags = SceneLoadFlags.Default, float inFadeDuration = DefaultFadeDuration) {
            inFlags |= LoadFlags;

            if (!BeforeLoad(inFlags, inFadeDuration)) {
                return null;
            }

            return screenFaderDisplay.FadeTransition(Color.black, inFadeDuration, PauseDuration,
                () => Sequence.Create(Services.State.LoadScene(inSceneName, inEntrance, inContext, inFlags)).Then(() => AfterLoad(inFadeDuration))
            );
        }

        static public IEnumerator LoadSceneWithWipe(string inSceneName, StringHash32 inEntrance = default(StringHash32), object inContext = null, SceneLoadFlags inFlags = SceneLoadFlags.Default) {
            inFlags |= LoadFlags;

            if (!BeforeLoad(inFlags, DefaultFadeDuration)) {
                return null;
            }

            return screenFaderDisplay.WipeTransition(PauseDuration,
                () => Sequence.Create(Services.State.LoadScene(inSceneName, inEntrance, inContext, inFlags)).Then(AfterLoad)
            );
        }

        /*static public IEnumerator LoadMapWithWipe(StringHash32 inMapId, StringHash32 inEntrance = default(StringHash32), object inContext = null, SceneLoadFlags inFlags = SceneLoadFlags.Default, float inFadeDuration = DefaultFadeDuration) {
            inFlags |= LoadFlags;

            if (!BeforeLoad(inFlags, inFadeDuration)) {
                return null;
            }

            StringHash32 currentMapId = default(StringHash32);//MapDB.LookupCurrentMap();
            if (!currentMapId.IsEmpty) {
                if (inEntrance.IsEmpty && (inFlags & SceneLoadFlags.DoNotOverrideEntrance) == 0)
                    inEntrance = currentMapId;
            }

            return screenFaderDisplay.WipeTransition(PauseDuration,
                () => Sequence.Create(Services.State.LoadSceneFromMap(inMapId, inEntrance, inContext, inFlags)).Then(() => AfterLoad(inFadeDuration))
            );
        }*/

        static public IEnumerator LoadPreviousSceneWithFader(StringHash32 inEntrance = default(StringHash32), object inContext = null, SceneLoadFlags inFlags = SceneLoadFlags.Default, float inFadeDuration = DefaultFadeDuration) {
            inFlags |= LoadFlags;

            if (!BeforeLoad(inFlags, inFadeDuration)) {
                return null;
            }

            return screenFaderDisplay.FadeTransition(Color.black, inFadeDuration, PauseDuration,
                () => Sequence.Create(Services.State.LoadPreviousScene(DefaultBackScene, inEntrance, inContext, inFlags)).Then(() => AfterLoad(inFadeDuration))
            );
        }

        static private bool BeforeLoad(SceneLoadFlags inFlags, float inFadeDuration) {
            if (IsLoading)
                return false;

            s_LastFlags = inFlags;
            if ((s_LastFlags & SceneLoadFlags.Cutscene) != 0) {
                //Services.UI.ShowLetterbox();
            }
            if ((s_LastFlags & SceneLoadFlags.StopMusic) != 0) {
                //Services.Audio.StopMusic();
            }
            if ((s_LastFlags & SceneLoadFlags.SuppressAutoSave) != 0) {
                //AutoSave.Suppress();
            }
            //Services.Input.PauseAll();
            //Services.Audio.FadeOut(inFadeDuration);
            //Services.Script.KillLowPriorityThreads(TriggerPriority.Cutscene, true);
            //Services.Events.Dispatch(GameEvents.SceneWillUnload);
            s_IsLoading = true;
            return true;
        }

        static private void PreSceneChange() {
            s_IsLoading = false;
        }

        static private void AfterLoad(float inFadeDuration) {
            if ((s_LastFlags & SceneLoadFlags.Cutscene) != 0) {
                //Services.UI.HideLetterbox();
            }
            //Services.Audio.FadeIn(inFadeDuration);
            //Services.Input.ResumeAll();
            s_IsLoading = false;
        }

        static private void AfterLoad() {
            AfterLoad(DefaultFadeDuration);
        }
    }
}