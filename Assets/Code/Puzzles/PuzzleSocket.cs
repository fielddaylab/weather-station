using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class PuzzleSocket : ItemSocket {
        #region Inspector
		public Socketable MatchingSocket = null;
		
        #endregion // Inspector
		
        private void Awake() {
            
        }

        public bool IsMatched() { return (MatchingSocket == Current); }
    }

}
