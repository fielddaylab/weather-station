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
        //public List<PuzzleButton> PuzzleButtons = new List<PuzzleButton>();
   
        public List<Material> SensorMaterials = new List<Material>();

		public List<Material> PairedMaterials = new List<Material>();

        /*[Serializable]
        public class PuzzleSlot {
            public List<Texture2D> SlotTextures = new List<Texture2D>();
        }
        
        public List<PuzzleSlot> PuzzleSlots = new List<PuzzleSlot>();*/
        public List<Texture2D> Solution = new List<Texture2D>(); 
        public List<Texture2D> SolutionTextures = new List<Texture2D>(); 

		public Color GlowColor;
		public Material SensorMaterial;

        #endregion // Inspector

        //private int[] ButtonIndices;

		//private List<Color> PriorColors = new List<Color>();
		
		const int TOTAL_BUTTONS = 4;

		public override bool CheckComplete() {
            for(int i = 0; i < TOTAL_BUTTONS; ++i) {
                if(SensorMaterials[i].mainTexture != SolutionTextures[i]) {
					//PairedMaterials[PairedMaterials.Count-1].color = PriorColors[PairedMaterials.Count-1];
					return false;
                } 
            }

			//PairedMaterials[PairedMaterials.Count-1].color = GlowColor;
			if(State != PuzzleState.Complete)
			{
				Debug.Log("Completed Temperature Sensor Puzzle!");
				State = PuzzleState.Complete;
			}

            return true;
		}
		
		public void SensorPlaced() {
			if(SensorMaterial != null) {
				SensorMaterial.color = GlowColor;
			}
		}
		
        private void Awake() {
            
			for(int i = 0; i < TOTAL_BUTTONS; ++i) {
				//PuzzleButtons[i].OnPressed.Register(SensorButtonPressed);
				//SensorMaterials[i].mainTexture = PuzzleSlots[i].SlotTextures[0];
				//PriorColors.Add(PairedMaterials[i].color);
			}

			//ButtonIndices = new int[TOTAL_BUTTONS];

			//for(int i = 0; i < TOTAL_BUTTONS; ++i) {
			//	ButtonIndices[i] = 0;
			//}
		
        }

        /*private void SensorButtonPressed(PuzzleButton button) {
            //switch between textures...
            int buttonIndex = (int)char.GetNumericValue(button.gameObject.name[button.gameObject.name.Length-1])-1;
            ButtonIndices[buttonIndex]++;
            ButtonIndices[buttonIndex] = ButtonIndices[buttonIndex] % PuzzleSlots[buttonIndex].SlotTextures.Count;
            SensorMaterials[buttonIndex].mainTexture = PuzzleSlots[buttonIndex].SlotTextures[ButtonIndices[buttonIndex]];
			
			//the algorithm here should be - if pressing a button - look ahead, and any button that is locked (until you get to an incorrect button) 
			//and part of the solution, switch to solution texture
            //walk from beginning to current texture, and switch red to blue if all preceeding textures are good... if one isn't good, turn off all textures up to current from that one.
	
			if(SensorMaterials[buttonIndex].mainTexture == Solution[buttonIndex]) {
				//check to see if all before this are on solutionTextures, if so, change this one too...
				bool allSetBefore = true;
				for(int i = 0; i < buttonIndex; ++i) {
					if(SensorMaterials[i].mainTexture != SolutionTextures[i]) {
						allSetBefore = false;
						break;
					}
				}
				
				if(allSetBefore) {
					SensorMaterials[buttonIndex].mainTexture = SolutionTextures[buttonIndex];
					if(buttonIndex > 0) {
						PairedMaterials[buttonIndex-1].color = GlowColor;
					}
					//if this happens, then also walk ahead and see if we can switch any that are on a solution to final..
					for(int i = buttonIndex+1; i < PuzzleButtons.Count; ++i) {
						if(SensorMaterials[i].mainTexture == Solution[i]) {
							SensorMaterials[i].mainTexture = SolutionTextures[i];
							PairedMaterials[i-1].color = GlowColor;
						} else {
							break;
						}
					}
				}
			} else {
				//if we turn off.. then anything ahead of me should also go red...
				for(int i = buttonIndex; i < PuzzleButtons.Count; ++i) {
					if(SensorMaterials[i].mainTexture == SolutionTextures[i]) {
						SensorMaterials[i].mainTexture = Solution[i];
					}
					
					if(i > 0) {
						PairedMaterials[i-1].color = PriorColors[i-1];
					}
				}
			}
        }*/
    }

}
