using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.LateFixedUpdate, 501)]
    public class PlayerHandPositionUpdateSystem : SharedStateSystemBehaviour<PlayerHandRig> {
        public override void ProcessWork(float deltaTime) {
			
            m_State.UpdateStates();
			
			if(m_State.LeftHandGrab != null) {
				if(m_State.LeftHandGrab.IsGrabPosed) {
					//update rotation transform of grip pose hand to match that of the tracked if we are currently grabbing something
					if(!m_State.LeftHandGrab.ConstrainGripPosition) {
						m_State.LeftHandGrab.gameObject.transform.rotation = m_State.LeftHandGrab.GrabberVisual.transform.rotation;
					} else {
						m_State.LeftHandGrab.gameObject.transform.rotation = m_State.LeftHandGrab.ConstrainedGripTransform.rotation;
					}
					
					if(m_State.LeftHandGrab.ConstrainGripPosition) {
						//Debug.Log("Left: " + m_State.LeftHandGrab.ConstrainedGripPosition.ToString("F2"));
						m_State.LeftHandGrab.gameObject.transform.position = m_State.LeftHandGrab.ConstrainedGripPosition;
					}
				}
			}
			
			if(m_State.RightHandGrab != null) {
				if(m_State.RightHandGrab.IsGrabPosed) {
					//update rotation transform of grip pose hand to match that of the tracked if we are currently grabbing something
					if(!m_State.RightHandGrab.ConstrainGripPosition) {
						m_State.RightHandGrab.gameObject.transform.rotation = m_State.RightHandGrab.GrabberVisual.transform.rotation;
					} else {
						m_State.RightHandGrab.gameObject.transform.rotation = m_State.RightHandGrab.ConstrainedGripTransform.rotation;
					}
					
					if(m_State.RightHandGrab.ConstrainGripPosition) {
						//Debug.Log("Right: " + m_State.RightHandGrab.ConstrainedGripPosition.ToString("F2"));
						m_State.RightHandGrab.gameObject.transform.position = m_State.RightHandGrab.ConstrainedGripPosition;
					}
				}
			}
        }
    }
}