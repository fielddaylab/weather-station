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
        public string InitialScenePath;
        #endregion // Inspector

        void Awake() {
            Services.AutoSetup(Root);
            StartCoroutine(Services.State.ImportInitialScene(InitialScenePath));
        }

        public void UpdateStates() {

        }
    }
}