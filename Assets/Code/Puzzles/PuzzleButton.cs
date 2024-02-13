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
        
		public AudioSource SoundEffect;
        #endregion // Inspector
		
		[NonSerialized] public bool WasPressed = false;

        private bool On;
        //private Color PriorColor;
        //private MeshRenderer CachedMeshRenderer;

        public readonly CastableEvent<PuzzleButton> OnPressed = new CastableEvent<PuzzleButton>();
		
		public void Untoggle() {
			On = false;
			Vector3 vPos = transform.position;
			vPos.y += YShift;
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
					if(SoundEffect != null && SoundEffect.clip != null) {
						SoundEffect.Play();
					}
					

					
					if(!On) {
						Vector3 vPos = transform.position;
						vPos.y += YShift;
						vPos.x += XShift;
						transform.position = vPos;
						//CachedMeshRenderer.material.color = PriorColor;
					} else {
						Vector3 vPos = transform.position;
						vPos.y -= YShift;
						vPos.x -= XShift;
						transform.position = vPos;
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
							SceneLoader sceneInfo = Game.SharedState.Get<SceneLoader>();
							if(sceneInfo.GetCurrentSceneIndex() == 0) {
								ScriptPlugin.ForceKill = true;
								StartCoroutine(ArgoWasPressed(1f));
							}
						}

						WasPressed = true;
					}
					
					
					if(SoundEffect != null && SoundEffect.clip != null) {
						SoundEffect.Play();
					}
					
					Vector3 vPos = transform.position;
					vPos.y -= YShift;
					vPos.x -= XShift;
					transform.position = vPos;
					

					StartCoroutine(ShiftBack(c));
				}
				
				//haptics...
				//todo - optimize
				VRInputState data = Game.SharedState.Get<VRInputState>();
				if(c.gameObject.name.StartsWith("Left")) {
					data.LeftHand.HapticImpulse = 0.25f;
				} else if(c.gameObject.name.StartsWith("Right")) {
					data.RightHand.HapticImpulse = 0.25f;
				}

				OnPressed.Invoke(this);
			}
		}
		
		IEnumerator ShiftBack(Collider c) {
			yield return new WaitForSeconds(0.5f);
			Vector3 vPos = transform.position;
			vPos.y += YShift;
			vPos.x += XShift;
			transform.position = vPos;
			Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
			if(rb != null) {
				rb.detectCollisions = true;
			}
			
			/*if(Toggleable) {
				CachedMeshRenderer.material.color = PriorColor;
			}*/
		}
		
		IEnumerator TurnBackOn(Collider c) {
			yield return new WaitForSeconds(0.5f);
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
			ScriptUtility.Trigger("ArgoPressed");
		}
    }
}
