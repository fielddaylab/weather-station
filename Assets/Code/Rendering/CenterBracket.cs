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
    public class CenterBracket : MonoBehaviour {
        #region Inspector
		
		[SerializeField]
		float MinHeight;
		
		[SerializeField]
		float MaxHeight;
		
        #endregion // Inspector
		
		private Grabbable Handle;
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		[NonSerialized] public bool WasGrabbed = false;
		
		private Vector3 LastPos = Vector3.zero;
		
		void Update() {

			PlayerHandRig handRig = Lookup.State<PlayerHandRig>();

			Vector3 currPos = Vector3.zero;

			if(LeftGrabbed || RightGrabbed) {
				
				WasGrabbed = true;
				
				if(LeftGrabbed) {
					currPos = handRig.LeftHand.Visual.position;
				} else {
					currPos = handRig.RightHand.Visual.position;
				}
				

				float dir = 1f;
				if(LastPos.y - currPos.y > 0f) {
					dir = -1f;
				}
				
				if(currPos.y > MinHeight && currPos.y < MaxHeight) {
					transform.Translate(Vector3.up * dir * Vector3.Distance(LastPos, currPos), Space.World);
				}
				
				LastPos = currPos;
			} 
			
		}
		
        private void Awake() {
			Handle = GetComponent<Grabbable>();
			if(Handle != null) {
				Handle.OnGrabbed.Register(OnGrabPanel);
				Handle.OnReleased.Register(OnReleasePanel);
			}
        }
		
		private void Start() {

		}
		
		private void OnGrabPanel(Grabber grabber) {
			
			PlayerHandRig handRig = Lookup.State<PlayerHandRig>();
			
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
			PlayerHandRig handRig = Lookup.State<PlayerHandRig>();
			
			if(grabber == handRig.RightHand.Physics) {
				RightGrabbed = false;
			}
			
			if(grabber == handRig.LeftHand.Physics) {
				LeftGrabbed = false;
			}
		}
    }

}
