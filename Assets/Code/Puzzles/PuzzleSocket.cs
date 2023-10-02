using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WeatherStation {
    public class PuzzleSocket : ItemSocket {
        #region Inspector
		public Socketable MatchingSocket = null;

        public Socketable AltMatchingSocket = null;

		public List<Material> InMaterials = new List<Material>();

        public List<Material> OutMaterials = new List<Material>();

        public Color NewColor;
        public float BlinkTiming = 1f;

        #endregion // Inspector
		
        [NonSerialized] public bool PulseSet = false;

        private List<Color> OldColorsIn;
        private List<Color> OldColorsOut;

        private float BlinkTime = 0;
        private bool BlinkOn = false;

        private void Awake() {
            base.Awake();
#if UNITY_EDITOR
            ResetColors();
#endif
            if(InMaterials.Count > 0) {
                OldColorsIn = new List<Color>(InMaterials.Count);
                for(int i = 0; i < InMaterials.Count; ++i) {
                    OldColorsIn.Add(InMaterials[i].color);
                }
                BlinkTime = Time.time;
            }

            if(OutMaterials.Count > 0) {
                OldColorsOut = new List<Color>(OutMaterials.Count);
                for(int i = 0; i < OutMaterials.Count; ++i) {
                    OldColorsOut.Add(OutMaterials[i].color);
                } 
            }
        }

        public bool IsMatched() { return (MatchingSocket == Current) || ((AltMatchingSocket != null) && (AltMatchingSocket == Current)); }

        public void SetPulse() {
            if(InMaterials.Count > 0) {
                for(int i = 0; i < InMaterials.Count; ++i) {
                    InMaterials[i].color = NewColor;
                }

                for(int i = 0; i < OutMaterials.Count; ++i) {
                    OutMaterials[i].color = NewColor;
                }

                PulseSet = true;
            }
        }

        public void UnsetPulse() {
            if(InMaterials.Count > 0) {
                for(int i = 0; i < InMaterials.Count; ++i) {
                    InMaterials[i].color = OldColorsIn[i];
                }

                for(int i = 0; i < OutMaterials.Count; ++i) {
                    OutMaterials[i].color = OldColorsOut[i];
                }

                PulseSet = false;
            }
        }

        public void BlinkIncoming() {
            if(InMaterials.Count > 0) {
                float t = Time.time;
                if(t - BlinkTime > BlinkTiming) {
                    if(BlinkOn) {
                        for(int i = 0; i < InMaterials.Count; ++i) {
                            InMaterials[i].color = NewColor;
                        }
                    } else {
                        for(int i = 0; i < InMaterials.Count; ++i) {
                            InMaterials[i].color = OldColorsIn[i];
                        }
                    }

                    BlinkOn = !BlinkOn;
                    BlinkTime = t;
                } 
            }
        }

#if UNITY_EDITOR
        public void ResetColors() {
            Color c1 = new Color(141f/255f,141f/255f,141f/255f,1f);
            Color c2 = new Color(185f/255f,185f/255f,185f/255f,1f);
            Color c3 = new Color(74f/255f,74f/255f,74f/255f,1f);
            for(int i = 0; i < InMaterials.Count; ++i) {
                if(InMaterials[i].name.Contains("S")) {
                    InMaterials[i].color = c1;
                } else if(InMaterials[i].name.Contains("Prop")) {
                    InMaterials[i].color = c3;
                } else {
                    InMaterials[i].color = c2;
                }
            }
            for(int i = 0; i < OutMaterials.Count; ++i) {
                if(OutMaterials[i].name.Contains("S")) {
                    OutMaterials[i].color = c1;
                } else {
                    OutMaterials[i].color = c2;
                }
            }
        }
#endif
    }

#if UNITY_EDITOR
[CustomEditor(typeof(PuzzleSocket))]
    public class PuzzleSocketEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            PuzzleSocket p = (PuzzleSocket)target;
            if(GUILayout.Button("Reset Colors")) {
                p.ResetColors();
            }
        }
    }
#endif
}   
