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
    public class TrashChute : MonoBehaviour {
        #region Inspector

		
        #endregion // Inspector
		
		private Grabbable Handle;
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		private Vector3 LastPos = Vector3.zero;
		
		void Update() {

			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();

			Vector3 currPos = Vector3.zero;
			
			if(LeftGrabbed || RightGrabbed) {
				
				if(LeftGrabbed) {
					currPos = handRig.LeftHand.Visual.position;
				} else {
					currPos = handRig.RightHand.Visual.position;
				}
				
				Vector3 euler = transform.rotation.eulerAngles;
				
				float dotProd = Vector3.Dot(transform.right, Vector3.Normalize(LastPos - currPos));
				float dir = -1f;
				if(dotProd > 0f) {
					dir = 1f;
				}
				//if(euler.x < 70f || euler.x > 290f) {
					transform.RotateAround(transform.position, transform.forward * dir, Vector3.Distance(LastPos, currPos)*100f);
				//}
			
				//Debug.Log(euler.x);
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
		
		public void OnItemEntered(Collision c)
		{
			Socketable s = c.gameObject.GetComponent<Socketable>();
			if(s != null) {	
				if(s.SocketType == SocketFlags.WindSensorBlade) {
					//temp - only can trash wind sensor blades at the moment
					Destroy(c.gameObject);
				}
			}
		}
    }

}
