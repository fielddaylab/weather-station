using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scripting;
using UnityEngine;

namespace WeatherStation {
    public class PuzzleButton : BatchedComponent {
        #region Inspector
        
        public bool Locked = false;
        
        public bool Toggleable = false;

        public float YShift = 0.012f;
		
		public float XShift = 0.0f;
		
		public bool UseLocalSpace = false;
        
		public AudioSource SoundEffect;
        #endregion // Inspector
		
		[NonSerialized] public bool WasPressed = false;

        private bool On = false;
		
		public bool IsOn() { return On; }

        public readonly CastableEvent<PuzzleButton> OnPressed = new CastableEvent<PuzzleButton>();
		
		public void Untoggle() {
			On = false;
			Vector3 vPos = transform.position;
			vPos.y += YShift;
			//Debug.Log("Untoggle: " + vPos.ToString("F4"));
			transform.position = vPos;
			//CachedMeshRenderer.material.color = PriorColor;
		}
		
		public void ButtonTrigger(Collider c) {
			if(!Locked) {
				if(Toggleable) {					
				
					Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
					if(rb != null) {
						rb.detectCollisions = false;
					}
					
					if(!WasPressed) {
						WasPressed = true;
					}
					
					On = !On;
                    //if(SoundEffect != null && SoundEffect.clip != null) {
                    //	SoundEffect.Play();
                    //}
                    Sfx.OneShot("button-click", transform.position);
					
					if(!On) {
						Vector3 vPos = transform.position;
						vPos.y += YShift;
						vPos.x += XShift;
						transform.position = vPos;
						//Debug.Log("PuzzleButtonToggle Off: " + vPos.ToString("F4"));
						//CachedMeshRenderer.material.color = PriorColor;
					} else {
						Vector3 vPos = transform.position;
						vPos.y -= YShift;
						vPos.x -= XShift;
						transform.position = vPos;
						//Debug.Log("PuzzleButtonToggle On: " + vPos.ToString("F4"));
						//CachedMeshRenderer.material.color = ButtonColor;
					}
					
					StartCoroutine(TurnBackOn(c));
					
				} else {
					
					Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
					if(rb != null) {
						rb.detectCollisions = false;
					}
					
					if(!WasPressed) {
						//this should only happen if we're on the first level...
						if(gameObject.name == "ArgoFaceButton") {
							SceneLoader sceneInfo = Find.State<SceneLoader>();
							if(sceneInfo.GetCurrentSceneIndex() == 0) {
								ScriptPlugin.ForceKill = true;
								StartCoroutine(ArgoWasPressed(1f));
							}
						}

						WasPressed = true;
					}


                    //if(SoundEffect != null && SoundEffect.clip != null) {
                    //	SoundEffect.Play();
                    //}
                    Sfx.OneShot("button-click", transform.position);

                    if (UseLocalSpace)
					{
						Vector3 vPos = transform.localPosition;
						vPos.y += YShift;
						vPos.x += XShift;
						transform.localPosition = vPos;
					}
					else
					{
						Vector3 vPos = transform.position;
						vPos.y -= YShift;
						vPos.x -= XShift;
						transform.position = vPos;
					}
					

					StartCoroutine(ShiftBack(c));
				}
				
				//haptics...
				//todo - optimize
				VRInputState data = Find.State<VRInputState>();
				if(c.gameObject.name.StartsWith("Left")) {
					data.LeftHand.HapticImpulse = 0.25f;
				} else if(c.gameObject.name.StartsWith("Right")) {
					data.RightHand.HapticImpulse = 0.25f;
				}

				WSAnalytics w = Find.State<WSAnalytics>();
				if(w != null) {
					if(gameObject.name == "Uplink_Button") {
						SceneLoader s = Find.State<SceneLoader>();
						w.LogTestUplink((c.gameObject.name == "LeftPointer"), s.GetCurrentSceneIndex());
					} else if(gameObject.name == "s.Cabinet_controlButton") {
						w.LogRotateDrawer((c.gameObject.name == "LeftPointer"));
					} else if(gameObject.name == "SSBay_Button1" || gameObject.name == "SSBay_Button12" || gameObject.name == "SSBay_Button_13") {
						TempSensorButton t = gameObject.GetComponent<TempSensorButton>();
						w.LogClickTemperatureComponent(c.gameObject.name == "LeftPointer", 1, t.GetCurrentSlotTexture(), t.GetNextSlotTexture() );
					} else if(gameObject.name == "SSBay_Button2" || gameObject.name == "SSBay_Button22" || gameObject.name == "SSBay_Button_2") {
						TempSensorButton t = gameObject.GetComponent<TempSensorButton>();
						w.LogClickTemperatureComponent(c.gameObject.name == "LeftPointer", 2, t.GetCurrentSlotTexture(), t.GetNextSlotTexture() );
					} else if(gameObject.name == "SSBay_Button3" || gameObject.name == "SSBay_Button32" || gameObject.name == "SSBay_Button_33") {
						TempSensorButton t = gameObject.GetComponent<TempSensorButton>();
						w.LogClickTemperatureComponent(c.gameObject.name == "LeftPointer", 3, t.GetCurrentSlotTexture(), t.GetNextSlotTexture() );
					} else if(gameObject.name == "SSBay_Button4" || gameObject.name == "SSBay_Button42" || gameObject.name == "SSBay_Button_43") {
						TempSensorButton t = gameObject.GetComponent<TempSensorButton>();
						w.LogClickTemperatureComponent(c.gameObject.name == "LeftPointer", 4, t.GetCurrentSlotTexture(), t.GetNextSlotTexture() );
					}
				}

				OnPressed.Invoke(this);
			}
		}
		
		IEnumerator ShiftBack(Collider c) {
			yield return new WaitForSeconds(0.3f);
			if(UseLocalSpace)
			{
				Vector3 vPos = transform.localPosition;
				vPos.y -= YShift;
				vPos.x -= XShift;
				//Debug.Log("Shifted back : " + vPos.ToString("F4"));
				transform.localPosition = vPos;
			}
			else
			{
				Vector3 vPos = transform.position;
				vPos.y += YShift;
				vPos.x += XShift;
				//Debug.Log("Shifted back : " + vPos.ToString("F4"));
				transform.position = vPos;
			}
			
			StartCoroutine(TurnBackOn(c));
			/*if(Toggleable) {
				CachedMeshRenderer.material.color = PriorColor;
			}*/
		}
		
		IEnumerator TurnBackOn(Collider c) {
			yield return new WaitForSeconds(0.3f);
			Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
			if(rb != null) {
				rb.detectCollisions = true;
			}
			
		}
		
        private void Awake() {
            if(Toggleable) {
                //CachedMeshRenderer = GetComponent<MeshRenderer>();
                //PriorColor = CachedMeshRenderer.material.color;
            }
        }
		
		IEnumerator ArgoWasPressed(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			while(ScriptPlugin.ForceKill) {
				yield return null;
			}
			//Debug.Log("TRIGGERING NEXT SCRIPT");
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("ArgoPressed");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("ArgoPressedSp");
			}
		}
    }
}
