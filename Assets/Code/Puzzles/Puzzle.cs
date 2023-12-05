using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class Puzzle : BatchedComponent {
        #region Inspector
		public int Level = 0;  //difficulty level
		
        #endregion // Inspector
		  
		[NonSerialized] public PuzzleState State = PuzzleState.Inactive;
		
        public ActionEvent OnCompleted = new ActionEvent();
        
		public bool IsComplete() { return (State == PuzzleState.Complete); }
		
		public virtual bool CheckComplete() { return false; }
		
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
