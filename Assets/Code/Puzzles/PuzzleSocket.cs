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
		//public List<MeshRenderer> RenderersToChange = new List<MeshRenderer>();
        public Color NewColor;
        #endregion // Inspector
		
        private void Awake() {
            base.Awake();
        }

        public bool IsMatched() { return (MatchingSocket == Current); }

    }

}   
