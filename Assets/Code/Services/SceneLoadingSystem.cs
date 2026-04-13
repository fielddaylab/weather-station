//#define ENABLE_INPUT_SHORTCUTS
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {

    [SysUpdate(GameLoopPhaseMask.PreUpdate)]
    public class SceneLoadingSystem : SharedStateSystemBehaviour<SceneLoader> {
		
		private int CurrentSceneIndex = 0;
		private bool WaitingForSceneSwitch = false;
		
		public override void ProcessWork(float deltaTime) {

			VRInputState data = Find.State<VRInputState>();

			if (Game.Scenes.IsMainLoading()) {
				return;
			}

			//both below button functionalities are temporary.
			//temp
			if(data.RightHand.Released(VRControllerButtons.Secondary) || Input.GetKeyDown(KeyCode.Tab)) {
				//switch scenes.. why is this hitting twice?
				//Debug.Log("SWITCH SCENES");
				//m_State.SwitchScenes();
#if ENABLE_INPUT_SHORTCUTS
				FieldDay.Scripting.ScriptPlugin.ForceVOSkipSet = true;
#endif
				data.RightHand.PrevButtons = 0;

			} else if(data.RightHand.Released(VRControllerButtons.Primary) || Input.GetKeyDown(KeyCode.Q)) {
#if ENABLE_INPUT_SHORTCUTS
				PlayerLocator player = Find.State<PlayerLocator>();
				
				SubtitleDisplay sd = player.gameObject.transform.GetChild(0).GetChild(5).GetComponent<SubtitleDisplay>();
				if(sd != null) {
					bool sdOn = sd.SubtitlesOn;
					sdOn = !sdOn;
					sd.SetOn(sdOn);
				}
#endif
				data.RightHand.PrevButtons = 0;
			}

			//temp
			if(data.LeftHand.Released(VRControllerButtons.Secondary) || Input.GetKeyDown(KeyCode.S)) {
#if ENABLE_INPUT_SHORTCUTS
				if(!WaitingForSceneSwitch)
				{
					if(ScriptPlugin.LastAudioSource != null)
					{
						ScriptPlugin.LastAudioSource.Stop();
					}
					
					ScriptPlugin.CompleteForceKill = true;
					WaitingForSceneSwitch = true;
					
					StartCoroutine("WaitForKill");
				}
#endif
				data.LeftHand.PrevButtons = 0;
			}

			m_State.UpdateStates();

		}
		
		IEnumerator WaitForKill()
		{
			yield return new WaitForSeconds(2f);
			
			ScriptPlugin.CompleteForceKill = false;
			
			//Debug.Log("Switching scenes...");
			m_State.SwitchScenes();
			
			PlayerLocator playerLocator = Find.State<PlayerLocator>();
				
			if(playerLocator.IsInside) {
				playerLocator.Teleport();
			}
			
			if(CurrentSceneIndex == -1)
			{
				WSAnalytics w = Find.State<WSAnalytics>();
				if(w != null)
				{
					w.LogStartGame();
				}
				
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("LoadWest");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("LoadWestSp");
				}

				CurrentSceneIndex++;		
			}
			else if(CurrentSceneIndex == 0)
			{
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("LevelOneFinished");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("LevelOneFinishedSp");
				}

				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 1)
			{
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("LevelTwoFinished");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("LevelTwoFinishedSp");
				}
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 2)
			{
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("LevelThreeFinished");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("LevelThreeFinishedSp");
				}
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 3)
			{
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("LevelFourFinished");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("LevelFourFinishedSp");
				}
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 4)
			{
				if(VoiceoverUtility.Loader.LanguagePath == "en/")
				{
					ScriptUtility.Trigger("EpilogueReady");
				}
				else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
				{
					ScriptUtility.Trigger("EpilogueReadySp");
				}
				
				CurrentSceneIndex++;
				
				/*if(CurrentSceneIndex == 5)
				{
					CurrentSceneIndex = 0;
				}*/
			}
			
			WaitingForSceneSwitch = false;
		}
    }
}