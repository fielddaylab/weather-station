using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class PuzzleButton : BatchedComponent {
        #region Inspector
        
        public bool Locked = false;
        
        public bool Toggleable = false;

        public float YShift = 0.012f;
		
		public float XShift = 0.0f;

        public Color ButtonColor;
        
		public AudioSource SoundEffect;
        #endregion // Inspector
		
        private bool On;

        private Color PriorColor;
        private MeshRenderer CachedMeshRenderer;

        public readonly CastableEvent<PuzzleButton> OnPressed = new CastableEvent<PuzzleButton>();
		
		public void Untoggle() {
			On = false;
			Vector3 vPos = transform.position;
			vPos.y += YShift;
			transform.position = vPos;
			CachedMeshRenderer.material.color = PriorColor;
		}
		
		public void ButtonTrigger(Collider c) {
            if(Toggleable) {
                On = !On;
                if(SoundEffect != null && SoundEffect.clip != null) {
                    SoundEffect.Play();
                }
                if(!On) {
                    Vector3 vPos = transform.position;
                    vPos.y += YShift;
					vPos.x += XShift;
                    transform.position = vPos;
                    CachedMeshRenderer.material.color = PriorColor;
                } else {
                    Vector3 vPos = transform.position;
                    vPos.y -= YShift;
					vPos.x -= XShift;
                    transform.position = vPos;
                    CachedMeshRenderer.material.color = ButtonColor;
                }
                
            } else {
                if(SoundEffect != null && SoundEffect.clip != null) {
                    SoundEffect.Play();
                }
				
				Vector3 vPos = transform.position;
				vPos.y -= YShift;
				vPos.x -= XShift;
				transform.position = vPos;
				
				Rigidbody rb = c.gameObject.GetComponent<Rigidbody>();
				if(rb != null) {
					rb.detectCollisions = false;
				}
				StartCoroutine(ShiftBack(c));
            }
			
			//haptics...
			//todo - optimize
			VRInputState data = Game.SharedState.Get<VRInputState>();
			if(c.gameObject.name.StartsWith("Left")) {
				data.LeftHand.HapticImpulse = 0.1f;
			} else if(c.gameObject.name.StartsWith("Right")) {
				data.RightHand.HapticImpulse = 0.1f;
			}

            OnPressed.Invoke(this);
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
			
			if(Toggleable) {
				CachedMeshRenderer.material.color = PriorColor;
			}
		}
		
        private void Awake() {
            if(Toggleable) {
                CachedMeshRenderer = GetComponent<MeshRenderer>();
                PriorColor = CachedMeshRenderer.material.color;
            }
        }
    }
}
