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
    public class SolarPanelPuzzle : Puzzle {
        #region Inspector
		public GameObject SolarPanel;
		public GameObject DirectionalLight;
		public GameObject PowerMeter;
		
		public Grabbable RightHandle;
		public Grabber RightHand;
		public GameObject RightHandProxy;
		public GameObject RightHandMesh;
		public GameObject RightHandTracked;
		
		public Grabbable LeftHandle;
		public Grabber LeftHand;
		public GameObject LeftHandProxy;
		public GameObject LeftHandMesh;
		public GameObject LeftHandTracked;
		
        #endregion // Inspector
		
		private bool LeftGrabbed = false;
		private bool RightGrabbed = false;
		
		private Vector3 GrabPointLeft = Vector3.zero;
		private Vector3 GrabPointRight = Vector3.zero;
		
		public override bool IsComplete() {
			if(!PowerMeter || !SolarPanel || !DirectionalLight) {
				Log.Msg("[SolarPanelPuzzle] Required references not set.");
				return false;
			}
			
			//Ross:  todo - optimize below...
			if(LeftGrabbed || RightGrabbed) {
				if(LeftGrabbed && !RightGrabbed) {
					Vector3 toLeft = Vector3.Normalize(GrabPointLeft - SolarPanel.transform.position);
					Vector3 trackedLeft = LeftHandTracked.transform.position;
					trackedLeft.y = SolarPanel.transform.position.y;
					Vector3 newLeft = Vector3.Normalize(trackedLeft - SolarPanel.transform.position);
					SolarPanel.transform.RotateAround(SolarPanel.transform.position, Vector3.up, Vector3.SignedAngle(toLeft, newLeft, Vector3.up));
					GrabPointLeft = LeftHandTracked.transform.position;
					GrabPointLeft.y = SolarPanel.transform.position.y;
				} else if(!LeftGrabbed && RightGrabbed) {
					Vector3 toRight = Vector3.Normalize(GrabPointRight - SolarPanel.transform.position);
					Vector3 trackedRight = RightHandTracked.transform.position;
					trackedRight.y = SolarPanel.transform.position.y;
					Vector3 newRight = Vector3.Normalize(trackedRight - SolarPanel.transform.position);
					SolarPanel.transform.RotateAround(SolarPanel.transform.position, Vector3.up, Vector3.SignedAngle(toRight, newRight, Vector3.up));
					GrabPointRight = RightHandTracked.transform.position;
					GrabPointRight.y = SolarPanel.transform.position.y;
				} else if(LeftGrabbed && RightGrabbed) {
					Vector3 toRight = Vector3.Normalize(GrabPointRight - GrabPointLeft);
					Vector3 trackedRight = RightHandTracked.transform.position;
					trackedRight.y = SolarPanel.transform.position.y;
					Vector3 trackedLeft = LeftHandTracked.transform.position;
					trackedLeft.y = SolarPanel.transform.position.y;
					Vector3 newRight = Vector3.Normalize(trackedRight - trackedLeft);
					SolarPanel.transform.RotateAround(SolarPanel.transform.position, Vector3.up, Vector3.SignedAngle(toRight, newRight, Vector3.up) * 1.5f);
					GrabPointRight = RightHandTracked.transform.position;
					GrabPointRight.y = SolarPanel.transform.position.y;
					GrabPointLeft = LeftHandTracked.transform.position;
					GrabPointLeft.y = SolarPanel.transform.position.y;
				}
			}
			
			VRInputState data = Game.SharedState.Get<VRInputState>();
			
			Vector3 vSun = DirectionalLight.transform.forward;
			vSun.y = 0f;
			vSun = Vector3.Normalize(vSun);
			
			Vector3 vMeter = PowerMeter.transform.forward;
			vMeter.y = 0f;
			vMeter = Vector3.Normalize(vMeter);
			
			float closeNess = -Vector3.Dot(vSun, vMeter);
			if(closeNess > 0f) {
				int numToHighlight = ((int)(closeNess * 9.9f)) + 1;
				int cc = PowerMeter.transform.childCount;
				for(int i = 0; i < numToHighlight; ++i) {
					PowerMeter.transform.GetChild(i).gameObject.SetActive(true);
				}
				for(int i = numToHighlight; i < cc; ++i) {
					PowerMeter.transform.GetChild(i).gameObject.SetActive(false);
				}
				
				if(numToHighlight == cc-1) {
					//Log.Msg("[SolarPanelPuzzle] completed solar panel puzzle.");
					return true;
				} else {
					if(LeftGrabbed) {
						data.LeftHand.HapticImpulse = (float)numToHighlight / (float)cc;
					}
					
					if(RightGrabbed) {
						data.RightHand.HapticImpulse = (float)numToHighlight / (float)cc;
					}
				}
			} else {
				int cc = PowerMeter.transform.childCount;
				for(int i = 0; i < cc; ++i) {
					PowerMeter.transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			
			return false;
		}
		
        private void Awake() {
            RightHandle.OnGrabbed.Register(OnGrabPanel);
			RightHandle.OnReleased.Register(OnReleasePanel);
			LeftHandle.OnGrabbed.Register(OnGrabPanel);
			LeftHandle.OnReleased.Register(OnReleasePanel);
        }
		
		private void OnGrabPanel(Grabber grabber) {
			if(grabber == RightHand) {
				RightHandMesh.SetActive(false);
				RightHandProxy.SetActive(true);
				RightGrabbed = true;
				GrabPointRight = RightHandTracked.transform.position;
				GrabPointRight.y = SolarPanel.transform.position.y;
			}
			
			if(grabber == LeftHand) {
				LeftHandMesh.SetActive(false);
				LeftHandProxy.SetActive(true);
				LeftGrabbed = true;
				GrabPointLeft = LeftHandTracked.transform.position;
				GrabPointLeft.y = SolarPanel.transform.position.y;
			}	
		}
		
		private void OnReleasePanel(Grabber grabber) {
			if(grabber == RightHand) {
				RightHandMesh.SetActive(true);
				RightHandProxy.SetActive(false);
				RightGrabbed = false;
			}
			
			if(grabber == LeftHand) {
				LeftHandMesh.SetActive(true);
				LeftHandProxy.SetActive(false);
				LeftGrabbed = false;
			}
		}
    }

}
