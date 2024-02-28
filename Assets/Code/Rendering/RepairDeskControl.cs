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
	public class RepairDeskControl : MonoBehaviour
	{
		[SerializeField] GameObject RepairDesk;
		
		[SerializeField] float MaxMove = 0.8975f;
		[SerializeField] float MinMove = 0.615f;
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		private Grabbable Handle;
		
		[NonSerialized] public bool WasGrabbed = false;
		
		private Vector3 LastPos = Vector3.zero;
		
		// Start is called before the first frame update
        private void Awake() {
			Handle = GetComponent<Grabbable>();
			if(Handle != null) {
				Handle.OnGrabbed.Register(OnGrabPanel);
				Handle.OnReleased.Register(OnReleasePanel);
			}
        }
		
		void Start() {

		}
		
		
		public void MoveDesk()
		{ 
		
		}
		
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
				
				if(currPos.y > MinMove && currPos.y < MaxMove) {
					if(RepairDesk != null) {
						RepairDesk.transform.Translate(Vector3.up * dir * Vector3.Distance(LastPos, currPos), Space.World);
					}
				}
				
				LastPos = currPos;
			} 
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