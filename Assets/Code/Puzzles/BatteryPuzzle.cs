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
        public Color BlinkColor;

        public float BlinkTiming = 1f;
        public GameObject PowerMeter;
		
		public Animator CoverAnim;
		
		public ItemSocket BatteryBase;
		
		public int PowerMeterOffset = 6;
		
        #endregion // Inspector
		
        private Color NewColor;
		private Color BlinkOldColor;
		
        private float BlinkTime = 0;

        private bool BlinkOn = false;
		
			
		/*public void UnlockCover() {
			if(Cover) {
				Cover.GrabEnabled = true;
			}
		}*/
		
		public override bool CheckComplete() {
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
					return false;
                }
            }
			
			if(BatteryBase.Locked)
			{
				BatteryBase.Locked = false;
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", false);
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
			if(PowerMeter != null) {
                float t = Time.time;
                if(t - BlinkTime > BlinkTiming) {
                    for(int i = 0; i < PuzzleSockets.Count; ++i) {
						if(BlinkOn) {
							PowerMeter.transform.GetChild(i*2).gameObject.GetComponent<MeshRenderer>().material.color = BlinkColor;
							PowerMeter.transform.GetChild(i*2+1).gameObject.GetComponent<MeshRenderer>().material.color = BlinkColor;
						} else {
							PowerMeter.transform.GetChild(i*2).gameObject.GetComponent<MeshRenderer>().material.color = BlinkOldColor;
							PowerMeter.transform.GetChild(i*2+1).gameObject.GetComponent<MeshRenderer>().material.color = BlinkOldColor;  
						}
                    } 
                    BlinkOn = !BlinkOn;
                    BlinkTime = t;
                }
				
				int matchCount = 0;
				for(int i = 0; i < PuzzleSockets.Count; ++i) {
					if(PuzzleSockets[i].IsMatched()) {
						matchCount++;
					}
					
					PowerMeter.transform.GetChild(PowerMeterOffset + i*2).gameObject.SetActive(false);
					PowerMeter.transform.GetChild(PowerMeterOffset + i*2+1).gameObject.SetActive(false);
				}
				
				for(int i = 0; i < matchCount; ++i) {
					PowerMeter.transform.GetChild(PowerMeterOffset + i*2).gameObject.SetActive(true);
					PowerMeter.transform.GetChild(PowerMeterOffset + i*2+1).gameObject.SetActive(true);
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
            if(PowerMeter != null) {
                NewColor = PowerMeter.transform.GetChild(PowerMeterOffset).gameObject.GetComponent<MeshRenderer>().material.color;
				BlinkOldColor = PowerMeter.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color;
            }            
			
			if(BatteryBase != null) {
				BatteryBase.OnAdded.Register(LockBase);
			}
        }
		
		private void LockBase(Socketable s)
		{
			if(BatteryBase) {
				BatteryBase.Locked = true;
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", true);
				}
			}
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
