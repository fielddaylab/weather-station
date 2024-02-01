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
		public ItemSocket TowerSocket;
        #endregion // Inspector
		
		private bool FanRotating = false;
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
			if(TowerSocket != null) {
				TowerSocket.OnAdded.Register(OnWindSensorAdded);
				TowerSocket.OnRemoved.Register(OnWindSensorRemoved);
			}
			
			//FanRotating = true;
			//StartCoroutine("RotateBlade");
        }
		
		private void OnWindSensorAdded() {
			if(State == PuzzleState.Complete) {
				FanRotating = true;
				StartCoroutine("RotateBlade");
			}
		}
		
		private void OnWindSensorRemoved() {
			if(State == PuzzleState.Complete) {
				FanRotating = false;
			}
		}
		
		IEnumerator RotateBlade() {
			//todo - blade rotating sound...
			WindSocket SocketRotation = Socket.gameObject.transform.GetChild(1).gameObject.GetComponent<WindSocket>();
            while(FanRotating) {
				Socket.Current.gameObject.transform.RotateAround(SocketRotation.gameObject.transform.position, Socket.gameObject.transform.right, 30f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
