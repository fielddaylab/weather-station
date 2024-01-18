using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class Puzzle : BatchedComponent {
        #region Inspector
		public int PuzzleLevel = 0;  //difficulty level
		public int GameLevel = 0;
        #endregion // Inspector
		 
		protected bool TestSuccess = false;
		
		[NonSerialized] public PuzzleState State = PuzzleState.Inactive;
		
        public ActionEvent OnCompleted = new ActionEvent();
        
		public bool IsComplete() { return (State == PuzzleState.Complete); }
		public bool IsAlmostComplete() { return (State == PuzzleState.Complete); }
		
		public void SetTestSuccess(bool success) {
			TestSuccess = success;
		}
		
		public virtual bool CheckComplete() { return false; }
		
        public virtual void UpdatePuzzle() {}

        private void Awake() {
            
        }
    }

    public enum PuzzleState {
		Inactive,
        Active,
		AlmostComplete,
        Complete
    }
}
