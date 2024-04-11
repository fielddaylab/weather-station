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
		float _gazeLogTimer = 0f;
		const float GAZE_LOG_TIMER_SEND = 1.0f;
		uint _gazeLogFrameCount = 0;
		
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
			
			WSAnalytics w = Find.State<WSAnalytics>();
			if(w != null) {
				m_State.GetHeadTransform(out Vector3 pos, out Quaternion quat);

				float t = UnityEngine.Time.time;

				if(t - _gazeLogTimer > GAZE_LOG_TIMER_SEND) {
					_gazeLogTimer = t;

					w.LogGaze(pos, quat, _gazeLogFrameCount, true);
					
					_gazeLogFrameCount++;
				}
				else {
					if(_gazeLogFrameCount % 2 == 0) {
						bool sentData = w.LogGaze(pos, quat, _gazeLogFrameCount);
						if(sentData) {
							_gazeLogTimer = t;
						}
					}
					
					_gazeLogFrameCount++;
				}
			}
        }
    }
}