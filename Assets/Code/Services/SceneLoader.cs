using FieldDay.SharedState;
using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldDay;

namespace WeatherStation {

    public class SceneLoader : SharedStateComponent {
        #region Inspector
        public GameObject Root;
        
		public List<string> SceneList = new List<string>(8);
        public GameObject FanBlade;	//temp
		#endregion // Inspector
		
		private int CurrentSceneIndex = 0;
		
		public ActionEvent OnSceneLoaded = new ActionEvent();
		
        void Start() {
            StartCoroutine(BroadcastSwitch(SceneList[0], 3f));
        }

        public void UpdateStates() {

        }
		
		public void SwitchScenes() {
			int nextIndex = CurrentSceneIndex+1;
			nextIndex = nextIndex % SceneList.Count;
            Game.Scenes.UnloadScenesByTag("Exterior");
            Game.Scenes.LoadAuxScene(SceneList[nextIndex], "Exterior");
			CurrentSceneIndex = nextIndex;
			StartCoroutine(BroadcastSwitch(SceneList[CurrentSceneIndex], 3f));
		}

        IEnumerator BroadcastSwitch(string newScene, float duration) {
            yield return new WaitForSeconds(duration);
			//Debug.Log(newScene);
			if(newScene == "Assets/Scenes/West.unity" || newScene == "Assets/Scenes/South.unity") {	//temp
				FanBlade.SetActive(false);
			} else {
				FanBlade.SetActive(true);
			}
            OnSceneLoaded.Invoke();
        }
    }
}