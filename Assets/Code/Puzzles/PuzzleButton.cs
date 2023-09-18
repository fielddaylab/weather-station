using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class PuzzleButton : BatchedComponent {
        #region Inspector
        public bool On;
        public float YShift = 0.012f;
		public AudioSource SoundEffect;
        #endregion // Inspector
		

        public readonly CastableEvent<PuzzleButton> OnPressed = new CastableEvent<PuzzleButton>();
		
		public void ButtonTrigger() {
			OnPressed.Invoke(this);
		}
		
        private void Awake() {
            OnPressed.Register(OnButtonPressed);
        }
        
        private void OnButtonPressed(PuzzleButton button) {
            PuzzleButtonUtility.ToggleButton(button);
        }
    }

    static public class PuzzleButtonUtility {

        static public void ToggleButton(PuzzleButton button) {
            button.On = !button.On;
			if(button.SoundEffect != null && button.SoundEffect.clip != null) {
				button.SoundEffect.Play();
			}
            if(!button.On) {
                Vector3 vPos = button.transform.position;
                vPos.y += button.YShift;
                button.transform.position = vPos;
            } else {
                Vector3 vPos = button.transform.position;
                vPos.y -= button.YShift;
                button.transform.position = vPos;   
            }
        }
    }
}
