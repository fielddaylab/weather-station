using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {

    public class SceneLoader : SharedStateComponent {
        #region Inspector
        public GameObject Root;
        
		public List<string> SceneList = new List<string>(8);
        #endregion // Inspector
		
		private int CurrentSceneIndex = 0;
		
        void Awake() {
            //StartCoroutine(Services.State.ImportInitialScene(SceneList[0]));
            //StartCoroutine(TestSwitch(5f));
        }

        public void UpdateStates() {

        }
		
		public void SwitchScenes() {
			int nextIndex = CurrentSceneIndex+1;
			nextIndex = nextIndex % SceneList.Count;
			//StartCoroutine(StateUtil.SwapSceneWithFader(SceneList[CurrentSceneIndex], SceneList[nextIndex]));
			CurrentSceneIndex = nextIndex;
		}

        /*IEnumerator TestSwitch(float duration) {
            yield return new WaitForSeconds(duration);
            SwitchScenes();
        }*/
    }
}