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
		[SerializeField] float RotateMin = 0f;
		[SerializeField] float RotateMax = 65f;
		[SerializeField] float RotateMin2 = 273f;
		[SerializeField] float RotateMax2 = 360f;
		
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
				
				if((euler.y >= RotateMin2 && euler.y <= RotateMax2) || (euler.y >= RotateMin && euler.y <= RotateMax)) {
					transform.RotateAround(transform.position, Vector3.up * dir, Vector3.Distance(LastPos, currPos)*150f);
				}
				
				Vector3 angles = transform.rotation.eulerAngles;
				
				if(angles.y < RotateMin2 && angles.y > RotateMax) {
					if(Mathf.Abs(angles.y - RotateMax) < Mathf.Abs(angles.y - RotateMin2)) {
						angles.y = RotateMax-1f;
					} else {
						angles.y = RotateMin2+1f;
					}
					transform.rotation = Quaternion.Euler(angles);
				} else if(angles.y > RotateMax2) {
					angles.y = RotateMax2-1f;
					transform.rotation = Quaternion.Euler(angles);
				} else if(angles.y < RotateMin) {
					angles.y = RotateMin+1f;
					transform.rotation = Quaternion.Euler(angles);
				} else if(angles.y > RotateMax && angles.y < RotateMin2) {
					angles.y = RotateMax-1f;
					transform.rotation = Quaternion.Euler(angles);
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
