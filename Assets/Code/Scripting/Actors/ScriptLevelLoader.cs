using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Variants;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using FieldDay.Scripting;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting {
	[RequireComponent(typeof(SceneLoader))]
    public class ScriptLevelLoader : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private SceneLoader m_Loader =null;
        
        #region Leaf
		
        private void Awake() {
            m_Loader = GetComponent<SceneLoader>();
        }
		
		[LeafMember("SwitchScenes"), Preserve]
        public void SwitchScenes() {
			m_Loader.SwitchScenes();
        }
		
		[LeafMember("LoadNorthwest"), Preserve]
		public void LoadNorthwest() {
			m_Loader.SwitchScenes();
			ScriptUtility.Trigger("LevelOneFinished");
		}
		
		[LeafMember("LoadSouth"), Preserve]
		public void LoadSouth() {
			m_Loader.SwitchScenes();
			ScriptUtility.Trigger("LevelTwoFinished");
		}
		
		[LeafMember("LoadEast"), Preserve]
		public void LoadEast() {
			m_Loader.SwitchScenes();
			ScriptUtility.Trigger("LevelThreeFinished");
		}
		
		[LeafMember("LoadSouthEast"), Preserve]
		public void LoadSouthEast() {
			m_Loader.SwitchScenes();
			ScriptUtility.Trigger("LevelFourFinished");
		}
		
		[LeafMember("LoadEpilogue"), Preserve]
		public void LoadEpilogue() {
			m_Loader.SwitchScenes();
			ScriptUtility.Trigger("EpilogueReady");
		}
		
		[LeafMember("LoadSouthTempPuzzle"), Preserve]
		public void LoadSouthTempPuzzle() {
			ScriptUtility.Trigger("StartTempSensorSouth");
		}
		
		[LeafMember("LoadSouthWindPuzzle"), Preserve]
		public void LoadSouthWindPuzzle() {
			ScriptUtility.Trigger("StartWindSensorSouth");
		}
		
		[LeafMember("LoadSouthBatteryPuzzle"), Preserve]
		public void LoadSouthBatteryPuzzle() {
			ScriptUtility.Trigger("StartBatteryPuzzleSouth");
		}
		
		[LeafMember("LoadSoutheastBatteryPuzzle"), Preserve]
		public void LoadSoutheastBatteryPuzzle() {
			ScriptUtility.Trigger("StartBatteryPuzzleSoutheast");
		}
		
		[LeafMember("LoadSoutheastTempPuzzle"), Preserve]
		public void LoadSoutheastTempPuzzle() {
			ScriptUtility.Trigger("StartTempSensorSoutheast");
		}
		
        #endregion // Leaf
		
    }
}