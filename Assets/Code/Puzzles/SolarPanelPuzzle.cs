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
		
		public Grabbable LeftHandle;
		public Grabber LeftHand;
		public GameObject LeftHandProxy;
		public GameObject LeftHandMesh;
		
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
			
			if(LeftGrabbed || RightGrabbed) {
				
			}
			//VRInputState data = Game.SharedState.Get<VRInputState>();
			
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
				
				//data.LeftHand.RequestHaptics = true;
				//data.RightHand.RequestHaptics = true;
				
				if(numToHighlight == cc-1) {
					//Log.Msg("[SolarPanelPuzzle] completed solar panel puzzle.");
					return true;
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
        }
		
		private void OnGrabPanel(Grabber grabber)
		{
			if(grabber == RightHand)
			{
				RightHandMesh.SetActive(false);
				RightHandProxy.SetActive(true);
			}
			
			if(grabber == LeftHand)
			{
				LeftHandMesh.SetActive(false);
				LeftHandProxy.SetActive(true);
			}	
		}
		
		private void OnReleasePanel(Grabber grabber)
		{
			if(grabber == RightHand)
			{
				RightHandMesh.SetActive(true);
				RightHandProxy.SetActive(false);
			}
			
			if(grabber == LeftHand)
			{
				LeftHandMesh.SetActive(true);
				LeftHandProxy.SetActive(false);
			}
		}
    }

}
