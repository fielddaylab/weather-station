using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class PlayerHandPositionUpdateSystem : SharedStateSystemBehaviour<PlayerHandRig> {
        public override void ProcessWork(float deltaTime) {
			
            m_State.UpdateStates();
			
			if(m_State.LeftHandGrab.IsGrabPosed) {
				//update rotation transform of grip pose hand to match that of the tracked if we are currently grabbing something
				m_State.LeftHandGrab.gameObject.transform.rotation = m_State.LeftHandGrab.GrabberVisual.transform.rotation;
			}
			
			if(m_State.RightHandGrab.IsGrabPosed) {
				//update rotation transform of grip pose hand to match that of the tracked if we are currently grabbing something
				m_State.RightHandGrab.gameObject.transform.rotation = m_State.RightHandGrab.GrabberVisual.transform.rotation;
			}
        }
    }
}