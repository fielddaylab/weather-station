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

		
        #endregion // Inspector
		
		private Grabbable Handle;
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		[NonSerialized] public bool WasGrabbed = false;
		
		private Vector3 LastPos = Vector3.zero;
		private List<GameObject> TowerObjects;
		
		void Update() {

			PlayerHandRig handRig = Lookup.State<PlayerHandRig>();

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
				

				float dir = 1f;
				if(LastPos.y - currPos.y > 0f) {
					dir = -1f;
				}
				
				if(currPos.y > 0.25f && currPos.y < 1.5f) {
					transform.Translate(Vector3.up * dir * Vector3.Distance(LastPos, currPos), Space.World);
					/*for(int i = 0; i < TowerObjects.Count; ++i)
					{
						TowerObjects[i].transform.Translate(Vector3.up * dir * Vector3.Distance(LastPos, currPos), Space.World);
					}*/
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
		
		private void Start() {
			//TowerObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Tower"));
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
