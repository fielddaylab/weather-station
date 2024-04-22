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
		public List<Socketable> MatchingSockets = new List<Socketable>();
		
		public List<MeshRenderer> InMaterials = new List<MeshRenderer>();
		
        public List<MeshRenderer> OutMaterials = new List<MeshRenderer>();
		public List<int> OutMaterialIndex = new List<int>();

        public Material NewColor;
        public float BlinkTiming = 1f;
		public GameObject ObjectToActivate = null;
		public GameObject NextObjectToActivate = null;
		
        #endregion // Inspector
		
        [NonSerialized] public bool PulseSet = false;

        private List<Material> OldColorsIn;
        private List<Material> OldColorsOut;

        private float BlinkTime = 0;
        private bool BlinkOn = false;

        protected override void Awake() {
            base.Awake();

            if(InMaterials.Count > 0) {
                OldColorsIn = new List<Material>(InMaterials.Count);
                for(int i = 0; i < InMaterials.Count; ++i) {
                    OldColorsIn.Add(InMaterials[i].material);
                }
                BlinkTime = Time.time;
            }

            if(OutMaterials.Count > 0) {
                OldColorsOut = new List<Material>(OutMaterials.Count);
                for(int i = 0; i < OutMaterials.Count; ++i) {
                    OldColorsOut.Add(OutMaterials[i].materials[OutMaterialIndex[i]]);
                } 
            }
        }

        public bool IsMatched() { 
			for(int i = 0; i < MatchingSockets.Count; ++i) {
				if(MatchingSockets[i] == Current) {
					return true;
				}
			}
			return false;
			//return ((MatchingSocket != null) && (MatchingSocket == Current)) || ((AltMatchingSocket != null) && (AltMatchingSocket == Current)); 
		}

        public void SetPulse() {
            if(InMaterials.Count > 0) {
                for(int i = 0; i < InMaterials.Count; ++i) {
                    InMaterials[i].material = NewColor;
                }

				if(OutMaterials.Count > 0)
				{
					for(int i = 0; i < OutMaterials.Count; ++i) {
						Material[] mats = OutMaterials[i].materials;
						mats[OutMaterialIndex[i]] = NewColor;
						OutMaterials[i].materials = mats;
					}
				}

                PulseSet = true;
            }
			
			if(ObjectToActivate != null) {
				ObjectToActivate.SetActive(true);
			}
			
			if(NextObjectToActivate != null) {
				NextObjectToActivate.SetActive(true);
			}
        }

        public void UnsetPulse() {
            if(InMaterials.Count > 0) {
                for(int i = 0; i < InMaterials.Count; ++i) {
                    InMaterials[i].material = OldColorsIn[i];
                }
				
				if(OutMaterials.Count > 0)
				{
					for(int i = 0; i < OutMaterials.Count; ++i) {
						Material[] mats = OutMaterials[i].materials;
						mats[OutMaterialIndex[i]] = OldColorsOut[i];
						OutMaterials[i].materials = mats;
					}
				}

                PulseSet = false;
            }
			
			if(ObjectToActivate != null) {
				ObjectToActivate.SetActive(false);
			}
			
			if(NextObjectToActivate != null) {
				NextObjectToActivate.SetActive(false);
			}
        }

        public void BlinkIncoming() {
            if(InMaterials.Count > 0) {
                float t = Time.time;
                if(t - BlinkTime > BlinkTiming) {
                    if(BlinkOn) {
                        for(int i = 0; i < InMaterials.Count; ++i) {
                            InMaterials[i].material = NewColor;
                        }
                    } else {
                        for(int i = 0; i < InMaterials.Count; ++i) {
                            InMaterials[i].material = OldColorsIn[i];
                        }
                    }

                    BlinkOn = !BlinkOn;
                    BlinkTime = t;
                } 
            }
        }

    }
}   
