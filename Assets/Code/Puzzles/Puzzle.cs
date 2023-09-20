using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class Puzzle : BatchedComponent {
        #region Inspector
		//difficulty level
		public int Level = 0;
        #endregion // Inspector
		
		[NonSerialized] public PuzzleState State = PuzzleState.Inactive;
		
		public virtual bool IsComplete() { return false; }
		
        public virtual void UpdatePuzzle() {}

        private void Awake() {
            
        }
    }

    public enum PuzzleState {
		Inactive,
        Active,
        Complete
    }
}
