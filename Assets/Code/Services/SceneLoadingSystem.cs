#define ENABLE_INPUT_SHORTCUTS
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

			} else if(data.RightHand.Released(VRControllerButtons.Primary)) {
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
					ScriptPlugin.LastAudioSource.Stop();
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
			
			if(CurrentSceneIndex == 0)
			{
				ScriptUtility.Trigger("LevelOneFinished");
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 1)
			{
				ScriptUtility.Trigger("LevelTwoFinished");
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 2)
			{
				ScriptUtility.Trigger("LevelThreeFinished");
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 3)
			{
				ScriptUtility.Trigger("LevelFourFinished");
				CurrentSceneIndex++;
			}
			else if(CurrentSceneIndex == 4)
			{
				ScriptUtility.Trigger("EpilogueReady");
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