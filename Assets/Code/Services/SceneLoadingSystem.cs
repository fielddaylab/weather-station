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
         public override void ProcessWork(float deltaTime) {
			
            VRInputState data = Game.SharedState.Get<VRInputState>();
			
            if (Game.Scenes.IsMainLoading()) {
                return;
            }

			//both below button functionalities are temporary.
			//temp
            if(data.RightHand.Released(VRControllerButtons.Secondary) || Input.GetKeyDown(KeyCode.Tab)) {
                //switch scenes.. why is this hitting twice?
				//Debug.Log("SWITCH SCENES");
				//m_State.SwitchScenes();
				FieldDay.Scripting.ScriptPlugin.ForceVOSkipSet = true;
				data.RightHand.PrevButtons = 0;
            }
			
			//temp
			if(data.LeftHand.Released(VRControllerButtons.Secondary) || Input.GetKeyDown(KeyCode.S)) {
				m_State.SwitchScenes();
				ScriptUtility.Trigger("LevelOneFinished");
				data.LeftHand.PrevButtons = 0;
			}

            m_State.UpdateStates();

         }
    }
}