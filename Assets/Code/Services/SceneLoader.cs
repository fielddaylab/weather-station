using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {

    public class SceneLoader : SharedStateComponent {
        #region Inspector
        public GameObject Root;
        public string InteriorScene = "";

		public Material MapMaterial;
		
		public List<Texture2D> MapTextures = new List<Texture2D>(5);

		public List<string> SceneList = new List<string>(8);
		
		public List<ItemSocket> SledSockets = new List<ItemSocket>(8);
		
        //public GameObject FanBlade;	//temp
		#endregion // Inspector
		
		private int CurrentSceneIndex = 0;
		
		private bool SwitchingScenes = false;
		
		public ActionEvent OnSceneLoaded = new ActionEvent();
		
        void Start() {
            Game.Scenes.LoadAuxScene(SceneList[0], "Additional");
			//StartCoroutine(BroadcastSwitch(SceneList[0], 3f));
        }

        public void UpdateStates() {

        }
		
		public void SwitchScenes() {
			
			if(!SwitchingScenes) {
				SwitchingScenes = true;
				//return anything in your hands when switching scenes.
				PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
				
				//if(handRig.LeftHandGrab.IsGrabPosed) {
					GrabUtility.ForceGrabPoseOff(handRig.LeftHandGrab);
				//}
				
				if(handRig.LeftHand.Physics.State == GrabberState.Holding) {
					Grabbable h = handRig.LeftHand.Physics.Holding;
					if(h != null) {
						GrabUtility.ReturnToOriginalSpawnPoint(h);
					}
				}
				
				//if(handRig.RightHandGrab.IsGrabPosed) {
					GrabUtility.ForceGrabPoseOff(handRig.RightHandGrab);
				//}
				
				if(handRig.RightHand.Physics.State == GrabberState.Holding) {
					Grabbable h = handRig.RightHand.Physics.Holding;
					if(h != null) {
						GrabUtility.ReturnToOriginalSpawnPoint(h);
					}
				}
				
				//unsocket anything in the sled when switching scenes...
				for(int i = 0; i < SledSockets.Count; ++i) {
					if(SledSockets[i].Current != null) {
						Grabbable g = SledSockets[i].Current.gameObject.GetComponent<Grabbable>();
						if(g != null) {
							GrabUtility.ReturnToOriginalSpawnPoint(g);
						}
					}
				}
				
				GrabReturnSystem.ForceSkip = true;
				
				int nextIndex = CurrentSceneIndex+1;
				nextIndex = nextIndex % SceneList.Count;
                Game.Scenes.UnloadScene(SceneList[CurrentSceneIndex]);
                Game.Scenes.LoadAuxScene(SceneList[nextIndex], "Additional");
                CurrentSceneIndex = nextIndex;
				if(MapMaterial != null) {
					MapMaterial.mainTexture = MapTextures[nextIndex];
				}
				//StartCoroutine(BroadcastSwitch(SceneList[CurrentSceneIndex], 3f));
				StartCoroutine(PostLoad(3f));
			}
		}

		IEnumerator PostLoad(float waitTime) {
			yield return new WaitForSeconds(waitTime);
			GrabReturnSystem.ForceSkip = false;
			SwitchingScenes = false;
		}
		
        /*IEnumerator BroadcastSwitch(string newScene, float duration) {
            yield return new WaitForSeconds(duration);
			//Debug.Log(newScene);
			if(newScene == "Assets/Scenes/West.unity" || newScene == "Assets/Scenes/South.unity") {	//temp
				FanBlade.SetActive(false);
			} else {
				FanBlade.SetActive(true);
			}
            OnSceneLoaded.Invoke();
        }*/
    }
}