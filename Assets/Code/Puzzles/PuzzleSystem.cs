using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using FieldDay.Scripting;
using UnityEngine;
using BeauUtil;
using BeauUtil.Debugger;

namespace WeatherStation {
    //[SysUpdate(GameLoopPhase.LateUpdate)]
    public class PuzzleSystem : ComponentSystemBehaviour<Puzzle> {
		
		private int CurrentGameLevel = 1;
		
		private bool AllLevelThreePuzzlesDone = false;
		private bool AllLevelFivePuzzlesDone = false;
		private bool TriggeredLevelThreePuzzlesDone = false;
		private bool TriggeredLevelFivePuzzlesDone = false;
		
        public override void ProcessWorkForComponent(Puzzle component, float deltaTime) {

            component.UpdatePuzzle();
            
			if(CurrentGameLevel != component.GameLevel) {
				CurrentGameLevel = component.GameLevel;
			}
			
			if(CurrentGameLevel == 3) {
				if(component.State != PuzzleState.Complete) {
					AllLevelThreePuzzlesDone = false;
				}
			}
			
			if(CurrentGameLevel == 5) {
				if(component.State != PuzzleState.Complete) {
					AllLevelFivePuzzlesDone = false;
				}
			}
			
            //Log.Msg("Processing work for: " + component.gameObject.name);
			if(/*component.State != PuzzleState.Complete &&*/ component.CheckComplete()) {
				//do something cool, advance game, etc.
                component.State = PuzzleState.Complete;
                //component.OnCompleted.Invoke();
			}
			
			if(AllLevelThreePuzzlesDone) {
				if(!TriggeredLevelThreePuzzlesDone) {
					LevelThreeDone();
				}
			}
			
			if(AllLevelFivePuzzlesDone) {
				if(!TriggeredLevelFivePuzzlesDone) {
					LevelFiveDone();
				}
			}
        }
		
		private void LevelThreeDone() {
			ScriptUtility.Trigger("LevelThreePuzzlesFinished");
			TriggeredLevelThreePuzzlesDone = true;
		}
		
		private void LevelFiveDone() {
			ScriptUtility.Trigger("LevelFivePuzzlesFinished");
			TriggeredLevelFivePuzzlesDone = true;
		}
    }
}
