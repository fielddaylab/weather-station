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
        public List<PuzzleSocket> PuzzleSockets = new List<PuzzleSocket>();
        public Material FinalMaterial;
        public Color NewColor;

        public PuzzleButton TestButton;
		
		public GameObject FanBlade;
		public GameObject BrokenProp;
		public ItemSocket BaySocket;
		public Transform FanRotate;

        #endregion // Inspector
		
        private Color OldColor;
		private bool IsTesting = false;
		private bool IsStopped = false;
		private bool TestSuccess = false;
		
		public void UnlockSocket(Socketable s) {
			if(PuzzleSockets.Count > 0) {
				for(int i = 0; i < PuzzleSockets.Count; ++i) {
					PuzzleSockets[i].Locked = false;
				}
			}
		}
		
		public override bool CheckComplete() {
            bool allMatched = true;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    allMatched = false;
                    FinalMaterial.color = OldColor;
                } else {
                    FinalMaterial.color = NewColor;
                }
            }
			
			if(allMatched && TestSuccess) {
				if(State != PuzzleState.Complete) {
					//ScriptPlugin.ForceKill = true;
					//StartCoroutine(WindSensorComplete(1f));
					ScriptUtility.Trigger("WindSensorComplete");
				}
				State = PuzzleState.Complete;
				return true;
			}
			
			return false;
		}

        public override void UpdatePuzzle() {
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched() && PuzzleSockets[i].Current != null) {
                    if(PuzzleSockets[i].PulseSet) {
                        PuzzleSockets[i].UnsetPulse();
                    } else {
                        PuzzleSockets[i].BlinkIncoming();
                    }
                } else {
                    if(!PuzzleSockets[i].PulseSet) {
                        PuzzleSockets[i].SetPulse();
                    }
                }
            }
        }
		
        private void Awake() {
            OldColor = FinalMaterial.color;
            TestButton.OnPressed.Register(TestComplete);
			if(BaySocket != null) {
				BaySocket.OnRemoved.Register(OnSensorRemoved);
				BaySocket.OnAdded.Register(UnlockSocket);
			}
        }
		
		private void OnSensorRemoved() {
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
		}
		
        private void TestComplete(PuzzleButton button) {
			if(PuzzleSockets[0].Current) {
				if(PuzzleSockets[0].IsMatched()) {
					//rotate propeller, highlight green and chime sound...
					if(IsTesting) {
						//stop the propeller
						IsStopped = true;
					} else {
						IsTesting = true;
						PuzzleSockets[0].Locked = true;
						StartCoroutine(RotateAndFinish(PuzzleSockets[0], PuzzleSockets[0].Current, 120f));
						TestSuccess = true;
					}
				} else if(PuzzleSockets[0].Current == BrokenProp) {
					if(!IsTesting) {
						IsTesting = true;
						PuzzleSockets[0].Locked = true;
						//rotate a bit, then have it detach and fall..
						StartCoroutine(RotateAndFall(PuzzleSockets[0], PuzzleSockets[0].Current, 3f));
					}
					else
					{
						IsStopped = true;
					}				
				} else {
					if(!IsTesting) {
						IsTesting = true;
						PuzzleSockets[0].Locked = true;
						//rotate a bit, then have it detach and fall..
						StartCoroutine(RotateAndFail(PuzzleSockets[0], PuzzleSockets[0].Current, 10f));
					}
					else
					{
						IsStopped = true;
					}
				}
			}
			else
			{
				TestButton.Untoggle();
			}
        }

        private IEnumerator RotateAndFall(ItemSocket socket, Socketable socketable, float duration) {
            float t = 0f;
            while(t < duration && !IsStopped) {
				//Debug.Log("Rotating");
				if(PuzzleSockets[0].Current)
				{
					SocketUtility.RotateSocketed(PuzzleSockets[0], PuzzleSockets[0].Current, 0.5f);
				}
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            //unsocket and have it fall to the ground...
			SocketUtility.TryReleaseFromCurrentSocket(PuzzleSockets[0].Current, true);
			
			TestButton.Untoggle();
			
			PuzzleSockets[0].Locked = false;
			IsTesting = false;
			IsStopped = false;
        }
		
		private IEnumerator RotateAndFail(ItemSocket socket, Socketable socketable, float duration) {
            float t = 0f;
            while(t < duration && !IsStopped) {
				//Debug.Log("Rotating");
				if(PuzzleSockets[0].Current)
				{
					SocketUtility.RotateSocketed(PuzzleSockets[0], PuzzleSockets[0].Current, 2.5f);
				}
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            //unsocket and have it fall to the ground...
			SocketUtility.TryReleaseFromCurrentSocket(PuzzleSockets[0].Current, true);
			
			TestButton.Untoggle();
			
			PuzzleSockets[0].Locked = false;
			IsTesting = false;
			IsStopped = false;
        }


        private IEnumerator RotateAndFinish(ItemSocket socket, Socketable socketable, float duration) {
            
			AudioSource audioSource = GetComponent<AudioSource>();
			if(audioSource != null && audioSource.clip != null) {
				audioSource.Play();
			}
			
			float t = 0f;
            while(t < duration && !IsStopped) {
                //Debug.Log("Rotating");
				SocketUtility.RotateSocketed(PuzzleSockets[0], PuzzleSockets[0].Current, 10f);
				FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, 10f);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
			
			TestButton.Untoggle();
			
			IsTesting = false;
			IsStopped = false;
        }
		
		IEnumerator WindSensorComplete(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			//while(ScriptPlugin.ForceKill) {
			//	yield return null;
			//}
			//Debug.Log("TRIGGERING NEXT SCRIPT");
			ScriptUtility.Trigger("WindSensorComplete");
		}
    }

#if UNITY_EDITOR
[CustomEditor(typeof(WindSensorPuzzle))]
    public class WindSensorEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            WindSensorPuzzle p = (WindSensorPuzzle)target;
            if(GUILayout.Button("Reset Colors")) {
                Color c1 = new Color(74f/255f,74f/255f,74f/255f,1f);
                p.FinalMaterial.color = c1;
            }
        }
    }
#endif
}
