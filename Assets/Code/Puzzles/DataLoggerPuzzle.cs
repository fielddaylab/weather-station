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

        public List<Material> DoorPieceMaterials = new List<Material>(6);
		
        #endregion // Inspector
		
		public override bool CheckComplete() {
            bool allMatched = true;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    allMatched = false;
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

			for(int i = 0; i < PuzzleSockets.Count; ++i) {
				PuzzleSockets[i].OnRemoved.Register(OnDataPieceRemoved);
			}
        }
		
		private void OnDataPieceRemoved(Socketable s)
		{
			//Debug.Log("Piece removed");
			/*Material[] m = s.gameObject.GetComponent<MeshRenderer>().materials;
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
			}*/
		}
    }
}
