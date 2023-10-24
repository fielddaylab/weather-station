using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class PlayerLocationSystem : SharedStateSystemBehaviour<PlayerLocator> {
		
		
        public override void ProcessWork(float deltaTime) {
			
            //m_State.UpdateStates();
			
			
        }
    }
}