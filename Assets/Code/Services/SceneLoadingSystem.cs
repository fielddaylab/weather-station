using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;

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

            if(data.RightHand.Released(VRControllerButtons.Secondary) || Input.GetKeyDown(KeyCode.Tab)) {
                //switch scenes.. why is this hitting twice?
				//Debug.Log("SWITCH SCENES");
				//m_State.SwitchScenes();
				data.RightHand.PrevButtons = 0;
            }

            m_State.UpdateStates();

         }
    }
}