using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
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
		public Transform FanRotate;

        #endregion // Inspector
		
        private Color OldColor;

		public override bool IsComplete() {
            bool allMatched = true;
            for(int i = 0; i < PuzzleSockets.Count; ++i) {
                if(!PuzzleSockets[i].IsMatched()) {
                    allMatched = false;
                    FinalMaterial.color = OldColor;
                } else {
                    FinalMaterial.color = NewColor;
                }
            }
			return allMatched;
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
        }

        private void TestComplete(PuzzleButton button) {
            if(PuzzleSockets[0].IsMatched()) {
                //rotate propeller, highlight green and chime sound...
                StartCoroutine(RotateAndFinish(PuzzleSockets[0], PuzzleSockets[0].Current, 120f));
            } else {
                //rotate a bit, then have it detach and fall..
                StartCoroutine(RotateAndFall(PuzzleSockets[0], PuzzleSockets[0].Current, 3f));
            }
        }

        private IEnumerator RotateAndFall(ItemSocket socket, Socketable socketable, float duration) {
            float t = 0f;
            while(t < duration) {
				//Debug.Log("Rotating");
                SocketUtility.RotateSocketed(PuzzleSockets[0], PuzzleSockets[0].Current, 0.5f);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

            //unsocket and have it fall to the ground...
			SocketUtility.TryReleaseFromCurrentSocket(PuzzleSockets[0].Current, false);
        }

        private IEnumerator RotateAndFinish(ItemSocket socket, Socketable socketable, float duration) {
            
			AudioSource audioSource = GetComponent<AudioSource>();
			if(audioSource != null && audioSource.clip != null) {
				audioSource.Play();
			}
			
			float t = 0f;
            while(t < duration) {
                //Debug.Log("Rotating");
				SocketUtility.RotateSocketed(PuzzleSockets[0], PuzzleSockets[0].Current, 5f);
				FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, 5f);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
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
