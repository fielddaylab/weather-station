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
    public class DataLoggerPuzzle : Puzzle {
        #region Inspector
        public List<PuzzleSocket> PuzzleSockets = new List<PuzzleSocket>();
        public Material FinalMaterial;
        public Color NewColor;
        public List<Material> DoorPieceMaterials = new List<Material>(6);
		
        #endregion // Inspector
		
        private Color OldColor;
		private Color DoorPieceColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		public override bool CheckComplete() {
            bool allMatched = true;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    allMatched = false;
                    FinalMaterial.color = OldColor;
                }
                else {
                    FinalMaterial.color = NewColor;
                }
            }
			return allMatched;
		}

        public override void UpdatePuzzle() {
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    if(PuzzleSockets[i].PulseSet) {
                        PuzzleSockets[i].UnsetPulse();
                    } else {
                        PuzzleSockets[i].BlinkIncoming();
                    }
                }
                else {
                    if(!PuzzleSockets[i].PulseSet) {
                        PuzzleSockets[i].SetPulse();
                    }
                }
            }
        }
		
        private void Awake() {
            OldColor = FinalMaterial.color;
			ResetColors();
			
			for(int i = 0; i < PuzzleSockets.Count; ++i) {
				PuzzleSockets[i].OnRemoved.Register(OnDataPieceRemoved);
			}
        }
		
		private void OnDataPieceRemoved(Socketable s)
		{
			Debug.Log("Piece removed");
			Material[] m = s.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
			for(int i = 0; i < m.Length; ++i)
			{
				for(int j = 0; j < DoorPieceMaterials.Count; ++j)
				{
					if(DoorPieceMaterials[j] == m[i])
					{
						m[i].color = DoorPieceColor;
						break;
					}
				}
			}
		}
		
		public void ResetColors() {
			for(int i = 0; i < DoorPieceMaterials.Count; ++i) {
				DoorPieceMaterials[i].color = DoorPieceColor;
			}
		}
    }

#if UNITY_EDITOR
[CustomEditor(typeof(DataLoggerPuzzle))]
    public class DataLoggerPuzzleEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            DataLoggerPuzzle p = (DataLoggerPuzzle)target;
            if(GUILayout.Button("Reset Colors")) {
                Color c1 = new Color(185f/255f,185f/255f,185f/255f,1f);
                p.FinalMaterial.color = c1;
				p.ResetColors();
            }
        }
    }
#endif
}
