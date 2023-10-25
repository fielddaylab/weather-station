using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {

    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class SceneLoadingSystem : SharedStateSystemBehaviour<SceneLoader> {
         public override void ProcessWork(float deltaTime) {
			
            VRInputState data = Game.SharedState.Get<VRInputState>();
			
            if(data.RightHand.Pressed(VRControllerButtons.Primary)) {
                //switch scenes..
            }

            m_State.UpdateStates();

         }
    }
}