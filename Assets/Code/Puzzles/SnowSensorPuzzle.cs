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
        public List<PuzzleButton> PuzzleButtons = new List<PuzzleButton>();
   
        public List<Material> SensorMaterials = new List<Material>();

        [Serializable]
        public class PuzzleSlot {
            public List<Texture2D> SlotTextures = new List<Texture2D>();
        }
        
        public List<PuzzleSlot> PuzzleSlots = new List<PuzzleSlot>();
        public List<Texture2D> Solution = new List<Texture2D>(); 
        public List<Texture2D> SolutionTextures = new List<Texture2D>(); 
        #endregion // Inspector

        private int[] ButtonIndices;

		public override bool IsComplete() {
            for(int i = 0; i < PuzzleButtons.Count; ++i) {
                if(SensorMaterials[i].mainTexture != SolutionTextures[i]) {
					/*if(i == 1) {
						if(SensorMaterials[1].mainTexture != SolutionTextures[i]) {
							return false;
						}
					} else {
						return false;
					}*/
					return false;
                } 
            }

            /*for(int i = 0; i < PuzzleButtons.Count; ++i) {
                SensorMaterials[i].mainTexture = SolutionTextures[i];
            }*/

            State = PuzzleState.Complete;

            return true;
		}
		
        private void Awake() {
            
            if(PuzzleButtons.Count > 0) {
                for(int i = 0; i < PuzzleButtons.Count; ++i) {
                    PuzzleButtons[i].OnPressed.Register(SensorButtonPressed);
                    SensorMaterials[i].mainTexture = PuzzleSlots[i].SlotTextures[0];
                }

                ButtonIndices = new int[PuzzleButtons.Count];

                for(int i = 0; i < PuzzleButtons.Count; ++i) {
                    ButtonIndices[i] = 0;
                }
            }
        }

        private void SensorButtonPressed(PuzzleButton button) {
            //switch between textures...
            int buttonIndex = (int)char.GetNumericValue(button.gameObject.name[button.gameObject.name.Length-1])-1;
            ButtonIndices[buttonIndex]++;
            ButtonIndices[buttonIndex] = ButtonIndices[buttonIndex] % PuzzleSlots[buttonIndex].SlotTextures.Count;
            SensorMaterials[buttonIndex].mainTexture = PuzzleSlots[buttonIndex].SlotTextures[ButtonIndices[buttonIndex]];
			
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
					
					//if this happens, then also walk ahead and see if we can switch any that are on a solution to final..
					for(int i = buttonIndex; i < PuzzleButtons.Count; ++i) {
						if(SensorMaterials[i].mainTexture == Solution[i]) {
							SensorMaterials[i].mainTexture = SolutionTextures[i];
						}
					}
				}
			} else {
				//if we turn off.. then anything ahead of me should also go red...
				for(int i = buttonIndex; i < PuzzleButtons.Count; ++i) {
					if(SensorMaterials[i].mainTexture == SolutionTextures[i]) {
						SensorMaterials[i].mainTexture = Solution[i];
					}
				}
			}
            
			//the algorithm here should be - if pressing a button - look ahead, and any button that is locked (until you get to an incorrect button) 
			//and part of the solution, switch to solution texture
            //walk from beginning to current texture, and switch red to blue if all preceeding textures are good... if one isn't good, turn off all textures up to current from that one.
			/*if(SensorMaterials[1].mainTexture == Solution[1] || SensorMaterials[1].mainTexture == SolutionTextures[1]) {
				SensorMaterials[2].mainTexture = Solution[2];
			} else {
				SensorMaterials[2].mainTexture = PuzzleSlots[2].SlotTextures[0];
				SensorMaterials[3].mainTexture = PuzzleSlots[3].SlotTextures[ButtonIndices[3]];
			}
			
			if(SensorMaterials[1].mainTexture == Solution[1])
			{
				SensorMaterials[1].mainTexture = SolutionTextures[1];
			}*/
        }
    }

}
