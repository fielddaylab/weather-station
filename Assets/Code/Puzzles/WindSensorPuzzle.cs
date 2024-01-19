using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;
using UnityEngine;
using UnityEngine.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WeatherStation {
    public class WindSensorPuzzle : Puzzle {
        #region Inspector
        public PuzzleSocket Socket;

        #endregion // Inspector
		
		public override bool CheckComplete() {

			if(Socket.IsMatched() && TestSuccess) {
				if(State != PuzzleState.Complete) {
					//ScriptPlugin.ForceKill = true;
					//StartCoroutine(WindSensorComplete(1f));
					if(GameLevel == 1)
					{
						ScriptUtility.Trigger("WindSensorComplete");
					}
				}
				State = PuzzleState.Complete;
				return true;
			}
			
			return false;
		}

        private void Awake() {

        }
		
		/*private void OnSensorRemoved() {
			if(FanBlade != null) {
				IsStopped = true;
			}
			
			bool allMatched = true;
			for(int i = 0; i < PuzzleSockets.Count; ++i) {
				if(!PuzzleSockets[i].IsMatched() && PuzzleSockets[i].Current != null) {
					allMatched = false;
					break;
				}
			}
			
			TestButton.Untoggle();
			IsTesting = false;
		}*/
		
		
		IEnumerator WindSensorComplete(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			//while(ScriptPlugin.ForceKill) {
			//	yield return null;
			//}
			//Debug.Log("TRIGGERING NEXT SCRIPT");
			ScriptUtility.Trigger("WindSensorComplete");
		}
    }
}
