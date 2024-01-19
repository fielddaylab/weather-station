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
		
		private bool TriggeredLevelThreePuzzlesDone = false;
		private bool TriggeredLevelFivePuzzlesDone = false;
		
        public override void ProcessWorkForComponent(Puzzle component, float deltaTime) {

            component.UpdatePuzzle();
            
			if(CurrentGameLevel != component.GameLevel) {
				CurrentGameLevel = component.GameLevel;
			}

            //Log.Msg("Processing work for: " + component.gameObject.name);
			if(/*component.State != PuzzleState.Complete &&*/ component.CheckComplete()) {
				//do something cool, advance game, etc.
                component.State = PuzzleState.Complete;
                //component.OnCompleted.Invoke();
				if(CurrentGameLevel == 3 && !TriggeredLevelThreePuzzlesDone) {
					CheckLevelThreeDone();
				} else if(CurrentGameLevel == 5 && !TriggeredLevelFivePuzzlesDone) {
					CheckLevelFiveDone();
				}
			}
        }
		
		private void CheckLevelThreeDone() {
			
			bool Done = true;
			
			for (int i = 0, count = m_Components.Count; i < count; i++) {
                if(m_Components[i].GameLevel == 3 && m_Components[i].State != PuzzleState.Complete)
				{
					Done = false;
				}
            }
			
			if(Done && !TriggeredLevelThreePuzzlesDone) {
				ScriptUtility.Trigger("LevelThreePuzzlesFinished");
				TriggeredLevelThreePuzzlesDone = true;
			}
		}
		
		private void CheckLevelFiveDone() {
			
			bool Done = true;
			
			for (int i = 0, count = m_Components.Count; i < count; i++) {
                if(m_Components[i].GameLevel == 5 && m_Components[i].State != PuzzleState.Complete)
				{
					Done = false;
				}
            }
			
			if(Done && !TriggeredLevelFivePuzzlesDone) {
				ScriptUtility.Trigger("LevelFivePuzzlesFinished");
				TriggeredLevelFivePuzzlesDone = true;
			}
		}
    }
}
