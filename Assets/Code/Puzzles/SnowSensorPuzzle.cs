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

namespace WeatherStation {
    public class SnowSensorPuzzle : Puzzle {
        #region Inspector
        public List<Material> SensorMaterials = new List<Material>(4);

		public List<Material> PairedMaterials = new List<Material>(4);

		public List<Texture2D> StartingTextures = new List<Texture2D>(4);
        public List<Texture2D> SolutionTextures = new List<Texture2D>(4); 

		public Color GlowColor;
		public Material SensorMaterial;

        #endregion // Inspector
		
		const int TOTAL_BUTTONS = 4;

		public override bool CheckComplete() {
			int totalComplete = 0;
            for(int i = 0; i < TOTAL_BUTTONS; ++i) {
                if(SensorMaterials[i].mainTexture == SolutionTextures[i]) {
					//PairedMaterials[PairedMaterials.Count-1].color = PriorColors[PairedMaterials.Count-1];
					totalComplete++;
                } 
            }
			
			if(totalComplete != 4)
			{
				if(totalComplete >= 2)
				{
					State = PuzzleState.AlmostComplete;
				}

				return false;
			}

			//PairedMaterials[PairedMaterials.Count-1].color = GlowColor;
			if(State != PuzzleState.Complete)
			{
				//Debug.Log("Completed Temperature Sensor Puzzle!");
				State = PuzzleState.Complete;
			}

            return true;
		}
		
		public void SensorPlaced() {
			if(SensorMaterial != null) {
				SensorMaterial.SetColor("_BaseColor", GlowColor);
			}
		}
		
        private void Awake() {
            
			for(int i = 0; i < TOTAL_BUTTONS; ++i) {
				SensorMaterials[i].mainTexture = StartingTextures[i];
				//PuzzleButtons[i].OnPressed.Register(SensorButtonPressed);
				//SensorMaterials[i].mainTexture = PuzzleSlots[i].SlotTextures[0];
				//PriorColors.Add(PairedMaterials[i].color);
			}
        }
    }

}
