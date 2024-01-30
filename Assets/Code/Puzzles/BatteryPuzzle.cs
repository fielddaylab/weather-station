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
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WeatherStation {
    public class BatteryPuzzle : Puzzle {
        #region Inspector
        public List<PuzzleSocket> PuzzleSockets = new List<PuzzleSocket>();
		public List<Material> PowerMeter = new List<Material>(8);
		
        public Color BlinkColor;
		public Color FinalColor;
		public Color BlinkOldColor;
		
        public float BlinkTiming = 1f;
		
        #endregion // Inspector

        private float BlinkTime = 0;

        private bool BlinkOn = false;
		
		public override bool CheckComplete() {
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
					return false;
                }
            }
			
			if(State != PuzzleState.Complete) {
				//ScriptPlugin.ForceKill = true;
				//StartCoroutine(WindSensorComplete(1f));
				State = PuzzleState.Complete;
			}
			
			return true;
		}
		
        public override void UpdatePuzzle() {

			float t = Time.time;
			if(t - BlinkTime > BlinkTiming) {
				int matchCount = 0;
				for(int i = 0; i < PuzzleSockets.Count; ++i) {
					if(PuzzleSockets[i].IsMatched()) {
						matchCount++;
					}
				}
				
				for(int i = 0; i < matchCount; ++i) {
					PowerMeter[i*2].color = FinalColor;
					PowerMeter[i*2+1].color = FinalColor;
				}
				
				for(int i = matchCount; i < PuzzleSockets.Count; ++i) {
					if(BlinkOn) {
						PowerMeter[i*2].color = BlinkColor;
						PowerMeter[i*2+1].color = BlinkColor;
					} else {
						PowerMeter[i*2].color = BlinkOldColor;
						PowerMeter[i*2+1].color = BlinkOldColor;  
					}
				} 
				
				BlinkOn = !BlinkOn;
				BlinkTime = t;
			}			
        }
		
		public void OnDisable() {
			for(int i = 0; i < PowerMeter.Count; ++i) {
				PowerMeter[i].color = BlinkOldColor;
			}
		}
		
        private void Awake() {
			
        }
    }
}
