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
		
		private void LogLevelComplete()
		{
			WSAnalytics w = Find.State<WSAnalytics>();
			if(w != null) 
			{
				w.LogLevelComplete();
			}
		}

		[LeafMember("SwitchScenes"), Preserve]
        public void SwitchScenes() {
			m_Loader.SwitchScenes();
        }
		
		[LeafMember("LoadWest"), Preserve]
		public void LoadWest() {
			m_Loader.SwitchScenes();
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("LoadWest");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("LoadWestSp");
			}
		}
		
		[LeafMember("LoadNorthwest"), Preserve]
		public void LoadNorthwest() {
			m_Loader.SwitchScenes();
			LogLevelComplete();
			
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("LevelOneFinished");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("LevelOneFinishedSp");
			}
		}
		
		[LeafMember("LoadSouth"), Preserve]
		public void LoadSouth() {
			m_Loader.SwitchScenes();
			LogLevelComplete();
			
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("LevelTwoFinished");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("LevelTwoFinishedSp");
			}
		}
		
		[LeafMember("LoadEast"), Preserve]
		public void LoadEast() {
			m_Loader.SwitchScenes();
			LogLevelComplete();
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("LevelThreeFinished");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("LevelThreeFinishedSp");
			}
		}
		
		[LeafMember("LoadSouthEast"), Preserve]
		public void LoadSouthEast() {
			m_Loader.SwitchScenes();
			LogLevelComplete();
			
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("LevelFourFinished");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("LevelFourFinishedSp");
			}
		}
		
		[LeafMember("LoadEpilogue"), Preserve]
		public void LoadEpilogue() {
			m_Loader.SwitchScenes();
			WSAnalytics w = Find.State<WSAnalytics>();
			if(w != null) 
			{
				w.LogEpilogueStart();
			}
			
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("EpilogueReady");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("EpilogueReadySp");
			}
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
			if(VoiceoverUtility.Loader.LanguagePath == "en/")
			{
				ScriptUtility.Trigger("StartBatteryPuzzleSouth");
			}
			else if(VoiceoverUtility.Loader.LanguagePath == "sp/")
			{
				ScriptUtility.Trigger("StartBatteryPuzzleSouthSp");
			}
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