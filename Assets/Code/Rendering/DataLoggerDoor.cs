using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class DataLoggerDoor : MonoBehaviour {
        #region Inspector

		
        #endregion // Inspector
		
		private Grabbable Handle;
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		[NonSerialized] public bool WasGrabbed = false;
		
		private Vector3 LastPos = Vector3.zero;
		
		void Update() {

			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();

			Vector3 currPos = Vector3.zero;
			Vector3 euler = transform.rotation.eulerAngles;
			//Debug.Log(euler.ToString("F5"));
			if(LeftGrabbed || RightGrabbed) {
				
				WasGrabbed = true;
				
				if(LeftGrabbed) {
					currPos = handRig.LeftHand.Visual.position;
				} else {
					currPos = handRig.RightHand.Visual.position;
				}
				
				float dotProd = Vector3.Dot(transform.forward, Vector3.Normalize(LastPos - currPos));
				float dir = -1f;
				if(dotProd > 0f) {
					dir = 1f;
				}
				
				if((euler.y + 10f > 275f && euler.y <= 360f) || (euler.y >= 0f && euler.y + 10f < 65f)) {
					transform.RotateAround(transform.position, Vector3.up * dir, Vector3.Distance(LastPos, currPos)*100f);
				}
				LastPos = currPos;
			} 
			
			//Debug.Log(euler.z);
		}
		
        private void Awake() {
			Handle = GetComponent<Grabbable>();
			if(Handle != null) {
				Handle.OnGrabbed.Register(OnGrabPanel);
				Handle.OnReleased.Register(OnReleasePanel);
			}
        }
		
		private void OnGrabPanel(Grabber grabber) {
			
			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
			
			if(grabber == handRig.RightHand.Physics) {
				RightGrabbed = true;
				LastPos = handRig.RightHand.Visual.position;
			}
			
			if(grabber == handRig.LeftHand.Physics) {
				LeftGrabbed = true;
				LastPos = handRig.LeftHand.Visual.position;
			}	
		}
		
		private void OnReleasePanel(Grabber grabber) {
			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
			
			if(grabber == handRig.RightHand.Physics) {
				RightGrabbed = false;
			}
			
			if(grabber == handRig.LeftHand.Physics) {
				LeftGrabbed = false;
			}
		}
    }

}