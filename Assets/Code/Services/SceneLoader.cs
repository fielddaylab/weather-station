using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scenes;
using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {

    public class SceneLoader : SharedStateComponent {
        #region Inspector

		public Material MapMaterial;
		
		public List<Texture2D> MapTextures = new List<Texture2D>(5);

		public List<string> SceneList = new List<string>(8);
		
		public List<ItemSocket> SledSockets = new List<ItemSocket>(8);
		
		public List<Material> SkyboxMaterials = new List<Material>(8);
		
		#endregion // Inspector
		
		private int CurrentSceneIndex = 0;
		
		private bool SwitchingScenes = false;
			
        void Start() {
			Game.Scenes.OnSceneReady.Register(SceneIsReady);
            Game.Scenes.LoadAuxScene(SceneList[0], "Additional", null, SceneImportFlags.ImportLightingSettings);
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
				
				PlayerLocator playerLocator = Game.SharedState.Get<PlayerLocator>();
				
				GrabUtility.ReturnToOriginalSpawnPoint(playerLocator.Argo.gameObject.GetComponent<Grabbable>());
				
				GrabReturnSystem.ForceSkip = true;
				
				int nextIndex = CurrentSceneIndex+1;
				nextIndex = nextIndex % SceneList.Count;
                Game.Scenes.UnloadScene(SceneList[CurrentSceneIndex]);
				Game.Scenes.LoadAuxScene(SceneList[nextIndex], "Additional", null, SceneImportFlags.ImportLightingSettings);
				if(MapMaterial != null) {
					MapMaterial.mainTexture = MapTextures[nextIndex];
				}
				RenderSettings.skybox = SkyboxMaterials[nextIndex];
				CurrentSceneIndex = nextIndex;
				StartCoroutine(PostLoad(3f));
			}
		}

		IEnumerator PostLoad(float waitTime) {
			yield return new WaitForSeconds(waitTime);

			GrabReturnSystem.ForceSkip = false;
			SwitchingScenes = false;
		}
		
		private void SceneIsReady(SceneEventArgs sceneArgs) {
			
			//Debug.Log("Scene ready");
			
			if(sceneArgs.Scene.path.Contains("Interior"))
			{
				GameObject[] roots = sceneArgs.Scene.GetRootGameObjects();
				PlayerLocator playerLocator = Game.SharedState.Get<PlayerLocator>();
	            foreach(var root in roots)
				{
					//Debug.Log(root);
					if(root.name == "PlaneInterior")
					{
						playerLocator.PlaneInterior = root;
					}
					else if(root.name == "Directional Light")
					{
						playerLocator.InteriorLight = root;
					}
				}			
			}
			else
			{
				GameObject[] roots = sceneArgs.Scene.GetRootGameObjects();
				PlayerLocator playerLocator = Game.SharedState.Get<PlayerLocator>();
	            foreach(var root in roots)
				{
					
					if(root.name == "Directional Light")
					{
						playerLocator.ExteriorLight = root;
					}
					else if(root.name == "SunLight00")
					{
						playerLocator.ExteriorLight = root.transform.GetChild(0).gameObject;
					}
					else if(root.name.Contains("AWS"))
					{
						//Debug.Log(root.name);
						ArgoMount am = root.GetComponent<ArgoMount>();
						if(am != null)
						{
							playerLocator.ArgoOutsideSocket = am.ArgoOutsideMount;
							playerLocator.SocketArgoOutside();
						}
					}
				}
				
				if(sceneArgs.Scene.path.Contains("SouthEast"))
				{
					RepairDesk rd = Game.SharedState.Get<RepairDesk>();
					
					for(int i = 0; i < rd.TemperatureSensorButtons1.Count; ++i)
					{
						rd.TemperatureSensorButtons1[i].SetActive(false);
					}
									
					for(int i = 0; i < rd.TemperatureSensorButtons2.Count; ++i)
					{
						rd.TemperatureSensorButtons2[i].SetActive(false);
					}
					
					for(int i = 0; i < rd.TemperatureSensorButtons3.Count; ++i)
					{
						rd.TemperatureSensorButtons3[i].SetActive(true);
					}
				}
				else if(sceneArgs.Scene.path.Contains("NorthWest"))
				{
					RepairDesk rd = Game.SharedState.Get<RepairDesk>();
					
					for(int i = 0; i < rd.TemperatureSensorButtons1.Count; ++i)
					{
						rd.TemperatureSensorButtons1[i].SetActive(true);
					}
									
					for(int i = 0; i < rd.TemperatureSensorButtons2.Count; ++i)
					{
						rd.TemperatureSensorButtons2[i].SetActive(false);
					}
					
					for(int i = 0; i < rd.TemperatureSensorButtons3.Count; ++i)
					{
						rd.TemperatureSensorButtons3[i].SetActive(false);
					}
				}
				else if(sceneArgs.Scene.path.Contains("South"))
				{
					RepairDesk rd = Game.SharedState.Get<RepairDesk>();
					
					for(int i = 0; i < rd.TemperatureSensorButtons1.Count; ++i)
					{
						rd.TemperatureSensorButtons1[i].SetActive(false);
					}
									
					for(int i = 0; i < rd.TemperatureSensorButtons2.Count; ++i)
					{
						rd.TemperatureSensorButtons2[i].SetActive(true);
					}
					
					for(int i = 0; i < rd.TemperatureSensorButtons3.Count; ++i)
					{
						rd.TemperatureSensorButtons3[i].SetActive(false);
					}
				}
			}
		}
    }
}