using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class SolarPanelPuzzle : Puzzle {
        #region Inspector
		public GameObject SolarPanel;
		public Transform DirectionalLight;
		public GameObject PowerMeter;
		
		public Grabbable RightHandle;
		public Grabbable LeftHandle;
		
		public bool LockRoll = false;
		
        #endregion // Inspector
		
		private bool RightGrabbed = false;
		private bool LeftGrabbed = false;
		
		private Vector3 GrabPointLeft = Vector3.zero;
		private Vector3 GrabPointRight = Vector3.zero;
		
		private Vector3 LastEuler = Vector3.zero;
		
		public override bool CheckComplete() {
			if(!PowerMeter || !SolarPanel || !DirectionalLight) {
				Log.Msg("[SolarPanelPuzzle] Required references not set.");
				return false;
			}
			
			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
			
			Vector3 euler = handRig.RightHand.Visual.rotation.eulerAngles;
			
			//bool RightGrabbed = (RightHandle.CurrentGrabbers[0] == handRig.RightHand.Physics);
			//bool LeftGrabbed = (LeftHandle.CurrentGrabbers[0] == handRig.LeftHand.Physics);

			//Ross:  todo - optimize below...
			if(LeftGrabbed || RightGrabbed) {
				
				Vector3 solarPanelPos = SolarPanel.transform.position;
				Vector3 trackedLeft = handRig.LeftHand.Visual.position;
				Vector3 trackedRight = handRig.RightHand.Visual.position;
				
				if(LeftGrabbed && !RightGrabbed) {
					Vector3 toLeft = Vector3.Normalize(GrabPointLeft - solarPanelPos);
					trackedLeft.y = solarPanelPos.y;
					Vector3 newLeft = Vector3.Normalize(trackedLeft - solarPanelPos);
					SolarPanel.transform.RotateAround(solarPanelPos, Vector3.up, Vector3.SignedAngle(toLeft, newLeft, Vector3.up));
					GrabPointLeft = trackedLeft;
					
				} else if(!LeftGrabbed && RightGrabbed) {
					Vector3 toRight = Vector3.Normalize(GrabPointRight - solarPanelPos);
					trackedRight.y = solarPanelPos.y;
					Vector3 newRight = Vector3.Normalize(trackedRight - solarPanelPos);
					SolarPanel.transform.RotateAround(solarPanelPos, Vector3.up, Vector3.SignedAngle(toRight, newRight, Vector3.up));
					GrabPointRight = trackedRight;
				} else if(LeftGrabbed && RightGrabbed) {
					Vector3 toRight = Vector3.Normalize(GrabPointRight - GrabPointLeft);
					trackedRight.y = solarPanelPos.y;
					trackedLeft.y = solarPanelPos.y;
					Vector3 newRight = Vector3.Normalize(trackedRight - trackedLeft);
					
					if(!LockRoll) {
						if(euler.x < 70f || euler.x > 290f) {
							float fDiff = Mathf.Abs(euler.x - LastEuler.x);
							if(fDiff > 0.2f)
							{
								SolarPanel.transform.RotateAround(solarPanelPos, toRight, euler.x - LastEuler.x);
							}
							else
							{
								SolarPanel.transform.RotateAround(solarPanelPos, Vector3.up, Vector3.SignedAngle(toRight, newRight, Vector3.up) * 1.5f);
							}
						}
					} else {
						SolarPanel.transform.RotateAround(solarPanelPos, Vector3.up, Vector3.SignedAngle(toRight, newRight, Vector3.up) * 1.5f);
					}
					
					//Debug.Log(euler.x);
					
					GrabPointRight = trackedRight;
					GrabPointLeft = trackedLeft;
				}
			}
			
			if(!LockRoll) {
				if(euler.x < 70f || euler.x > 290f) {
					LastEuler = euler;
				}
			}
			
			VRInputState data = Game.SharedState.Get<VRInputState>();
			
			Vector3 vSun = DirectionalLight.forward;
			if(PuzzleLevel == 0)
			{
				vSun.y = 0f;
			}
			vSun = Vector3.Normalize(vSun);
			
			Vector3 vMeter = PowerMeter.transform.forward;
			if(PuzzleLevel == 0)
			{
				vMeter.y = 0f;
			}
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
					if(State != PuzzleState.Complete) {
						if(GameLevel == 1) {
							ScriptPlugin.ForceKill = true;
							StartCoroutine(SolarPanelComplete(1f));
						}
					}
					State = PuzzleState.Complete;
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
			
			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
			
			if(grabber == handRig.RightHand.Physics) {
				RightGrabbed = true;
				GrabPointRight = handRig.RightHand.Visual.position;
				GrabPointRight.y = SolarPanel.transform.position.y;
			}
			
			if(grabber == handRig.LeftHand.Physics) {
				LeftGrabbed = true;
				GrabPointLeft = handRig.LeftHand.Visual.position;
				GrabPointLeft.y = SolarPanel.transform.position.y;
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
		
		IEnumerator SolarPanelComplete(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			while(ScriptPlugin.ForceKill) {
				yield return null;
			}
			//Debug.Log("TRIGGERING NEXT SCRIPT");
			ScriptUtility.Trigger("SolarPanelComplete");
		}
    }

}
