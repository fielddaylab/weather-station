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
    public class TempSensorButton : MonoBehaviour {
        #region Inspector
        
        [SerializeField] private List<Texture2D> SlotTextures = new List<Texture2D>();
        
		[SerializeField] private Material SensorMaterial;
		[SerializeField] private Material BayMaterial;

		[SerializeField] private PuzzleButton PB;
		[SerializeField] private Texture2D Solution;
		[SerializeField] private Texture2D SolutionTexture;
		
		[SerializeField] private bool PuzzleLocked = false;
		
		public GameObject ParticleFX;
		public string PrevState;
		public string NextState;
		
		public TempSensorButton PrevButton;
		public TempSensorButton NextButton;
		
        #endregion // Inspector

        private int ButtonIndex;

        private void Awake() {
			PB.OnPressed.Register(SensorButtonPressed);
			SensorMaterial.mainTexture = SlotTextures[0];
			//PriorColor = Color.white;//SensorMaterial.color;
			ButtonIndex = 0;
        }
		
		public void OnEnable()
		{
			if(ParticleFX != null) {
				PuzzleButton p = GetComponent<PuzzleButton>();
				if(PuzzleLocked) {
					//only want to set this if prior buttons are also set... or if we are just the first button.
					if(PrevButton == null) {
						Animator anim = ParticleFX.GetComponent<Animator>();
						anim.SetBool(NextState, true);
					}
					//Debug.Log("Setting next state");
				} else {
					Animator anim = ParticleFX.GetComponent<Animator>();
					anim.SetBool(NextState, false);
				}
			}
		}
		
		private void SensorButtonPressed(PuzzleButton button) {
            //switch between textures...
            //int buttonIndex = (int)char.GetNumericValue(button.gameObject.name[button.gameObject.name.Length-1])-1;
            ButtonIndex++;
            ButtonIndex = ButtonIndex % SlotTextures.Count;
            SensorMaterial.mainTexture = SlotTextures[ButtonIndex];
			
			//the algorithm here should be - if pressing a button - look ahead, and any button that is locked (until you get to an incorrect button) 
			//and part of the solution, switch to solution texture
            //walk from beginning to current texture, and switch red to blue if all preceeding textures are good... if one isn't good, turn off all textures up to current from that one.
			if(SensorMaterial.mainTexture == Solution) {
				//check to see if all before this are on solutionTextures, if so, change this one too...
				bool allSetBefore = true;
				TempSensorButton PriorButton = PrevButton;
				while(PriorButton != null)
				{
					if(PriorButton.SensorMaterial.mainTexture != PriorButton.SolutionTexture)
					{
						allSetBefore = false;
					}
					PriorButton = PriorButton.PrevButton;
				}
				
				if(allSetBefore) 
				{
					SensorMaterial.mainTexture = SolutionTexture;

					TempSensorButton CurrentButton = this;
					
					//if this happens, then also walk ahead and see if we can switch any that are on a solution to final..
					TempSensorButton AfterButton = NextButton;
					while(AfterButton != null)
					{
						if(AfterButton.SensorMaterial.mainTexture == AfterButton.Solution)
						{
							AfterButton.SensorMaterial.mainTexture = AfterButton.SolutionTexture;

							CurrentButton = AfterButton;
							
							AfterButton = AfterButton.NextButton;
						}
						else
						{
							break;
						}
					}
					
					AfterButton = this;
					while(AfterButton != CurrentButton)
					{
						if(AfterButton.ParticleFX != null)
						{
							Animator anim = AfterButton.ParticleFX.GetComponent<Animator>();
							if(AfterButton.PrevState.Length > 0)
							{
								anim.SetBool(AfterButton.PrevState, false);
							}
						}
						AfterButton = AfterButton.NextButton;
					}
					
					if(CurrentButton.ParticleFX != null)
					{
						Animator anim = CurrentButton.ParticleFX.GetComponent<Animator>();
						if(anim != null)
						{
							anim.SetBool(CurrentButton.PrevState, false);
							anim.SetBool(CurrentButton.NextState, true);
						}
					}
				}
			} else {
				
				
				//if we turn off.. then anything ahead of me should also go red...
				TempSensorButton AfterButton = this;
				while(AfterButton != null)
				{
					if(AfterButton.SensorMaterial.mainTexture == AfterButton.SolutionTexture)
					{
						AfterButton.SensorMaterial.mainTexture = AfterButton.Solution;
						if(AfterButton.ParticleFX != null)
						{
							Animator anim = AfterButton.ParticleFX.GetComponent<Animator>();
							anim.SetBool(AfterButton.NextState, false);
						}
					}
					
					AfterButton = AfterButton.NextButton;
				}
				
				if(ParticleFX != null)
				{
					Animator anim = ParticleFX.GetComponent<Animator>();
					anim.SetBool(NextState, false);
				}
				
				//also if we turn off, we want to walk up to this spot, or back from it, until we hit a spot that matches the solution
				TempSensorButton PB = PrevButton;
				while(PB != null)
				{
					if(PB.SensorMaterial.mainTexture == PB.SolutionTexture)
					{
						if(PB.ParticleFX != null)
						{
							Animator anim = PB.ParticleFX.GetComponent<Animator>();
							anim.SetBool(PB.NextState, true);
							break;
						}
					}
					else
					{
						if(PB.ParticleFX != null)
						{
							Animator anim = PB.ParticleFX.GetComponent<Animator>();
							anim.SetBool(PB.NextState, false);
						}
					}
					
					PB = PB.PrevButton;
				}
				
				
			}
        }
    }

}
