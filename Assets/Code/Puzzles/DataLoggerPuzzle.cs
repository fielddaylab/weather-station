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
            bool anyNotMatched = false;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    //unhighlight appropriate renderables
                    anyNotMatched = true;
                }
                else {
                    //highlight appropriate renderables
                }
            }
			return !anyNotMatched;
		}
		
        private void Awake() {
            
        }
    }

}
