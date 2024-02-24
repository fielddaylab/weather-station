using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class PlayerLocationSystem : SharedStateSystemBehaviour<PlayerLocator> {
		
		bool DidTurnLeft = false;
		bool DidTurnRight = false;
		
        public override void ProcessWork(float deltaTime) {
			
            //m_State.UpdateStates();
			VRInputState inputState = Find.State<VRInputState>();
			if(inputState.LeftHand.AxisTiltedLeft() && !DidTurnLeft && !DidTurnRight) {
				m_State.RotatePlayer(true);
				DidTurnLeft = true;
			} else if(inputState.LeftHand.AxisTiltedRight() && !DidTurnRight && !DidTurnLeft) {
				m_State.RotatePlayer(false);
				DidTurnRight = true;
			}
			
			if(!inputState.LeftHand.AxisTiltedLeft() && !inputState.LeftHand.AxisTiltedRight()) {
				DidTurnLeft = false;
				DidTurnRight = false;
			}
        }
    }
}