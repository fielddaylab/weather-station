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

namespace WeatherStation {
    public class DataLoggerPuzzle : Puzzle {
        #region Inspector
        public List<PuzzleSocket> PuzzleSockets = new List<PuzzleSocket>();
        #endregion // Inspector
		
		public override bool IsComplete() {
            bool allMatched = true;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    allMatched = false;
                }
                else {
                }
            }
			return allMatched;
		}
		
        private void Awake() {
            
        }
    }

}
