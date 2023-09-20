using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class PuzzleButton : BatchedComponent {
        #region Inspector
        
        public bool Locked = false;
        
        public bool Toggleable = false;

        public float YShift = 0.012f;
		public AudioSource SoundEffect;
        #endregion // Inspector
		
        private bool On;

        public readonly CastableEvent<PuzzleButton> OnPressed = new CastableEvent<PuzzleButton>();
		
		public void ButtonTrigger() {
            if(Toggleable) {
                On = !On;
                if(SoundEffect != null && SoundEffect.clip != null) {
                    SoundEffect.Play();
                }
                if(!On) {
                    Vector3 vPos = transform.position;
                    vPos.y += YShift;
                    transform.position = vPos;
                } else {
                    Vector3 vPos = transform.position;
                    vPos.y -= YShift;
                    transform.position = vPos;   
                }
            } else {
                if(SoundEffect != null && SoundEffect.clip != null) {
                    SoundEffect.Play();
                } 
            }

            OnPressed.Invoke(this);
		}
		
        private void Awake() {
           
        }
    }

}
