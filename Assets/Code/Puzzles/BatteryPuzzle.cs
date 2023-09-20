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
        public Material FinalMaterial;
        public Color NewColor;

        public GameObject PowerMeter;

        #endregion // Inspector
		
        private Color OldColor;

		public override bool IsComplete() {
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
					return false;
                }
            }
			return true;
		}

        public override void UpdatePuzzle() {
			if(PowerMeter != null) {
				for(int i = 0; i < PuzzleSockets.Count-1; ++i) {
					PowerMeter.transform.GetChild(i*3).gameObject.SetActive(PuzzleSockets[i].IsMatched());
					PowerMeter.transform.GetChild(i*3+1).gameObject.SetActive(PuzzleSockets[i].IsMatched());
					PowerMeter.transform.GetChild(i*3+2).gameObject.SetActive(PuzzleSockets[i].IsMatched());
				}
			
				if(PuzzleSockets[0].IsMatched()) {
					if(!PuzzleSockets[1].IsMatched()) {
						PuzzleSockets[1].BlinkIncoming();
						PuzzleSockets[2].BlinkIncoming();
					} else {
						PuzzleSockets[1].UnsetPulse();
						PuzzleSockets[2].UnsetPulse();
					}
				} 
			}
        }
		
		
        private void Awake() {
            //OldColor = FinalMaterial.color;
            
        }

    }

#if UNITY_EDITOR
[CustomEditor(typeof(BatteryPuzzle))]
    public class BatteryPuzzleEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            BatteryPuzzle p = (BatteryPuzzle)target;
            if(GUILayout.Button("Reset Colors")) {
                Color c1 = new Color(141f/255f,141f/255f,141f/255f,1f);
                p.FinalMaterial.color = c1;
            }
        }
    }
#endif
}
