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
                if(SensorMaterials[i].mainTexture != Solution[i]) {
                    return false;
                }
            }

            for(int i = 0; i < PuzzleButtons.Count; ++i) {
                SensorMaterials[i].mainTexture = SolutionTextures[i];
            }

            State = PuzzleState.Complete;

            return true;
		}
		
        private void Awake() {
            
            if(PuzzleButtons.Count > 0) {
                for(int i = 0; i < PuzzleButtons.Count; ++i) {
                    PuzzleButtons[i].OnPressed.Register(SensorButtonPressed);
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
        }
    }

}
