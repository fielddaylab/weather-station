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
		
		
		//public Color GlowColor;
		
        #endregion // Inspector

		//private Color PriorColor;
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
					Animator anim = ParticleFX.GetComponent<Animator>();
					anim.SetBool(NextState, true);
					if(PrevState.Length > 0) {
						anim.SetBool(PrevState, false);
					}
					//Debug.Log("Setting next state");
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
				
				/*for(int i = 0; i < buttonIndex; ++i) {
					if(SensorMaterials[i].mainTexture != SolutionTextures[i]) {
						allSetBefore = false;
						break;
					}
				}*/
				
				if(allSetBefore) {
					SensorMaterial.mainTexture = SolutionTexture;
					/*if(PrevButton != null) {
						PrevButton.BayMaterial.SetColor("_BaseColor", GlowColor);
					}*/
					
					if(ParticleFX != null)
					{
						Animator anim = ParticleFX.GetComponent<Animator>();
						anim.SetBool(NextState, true);
						if(PrevState.Length > 0)
						{
							anim.SetBool(PrevState, false);
						}
					}

					//if this happens, then also walk ahead and see if we can switch any that are on a solution to final..
					TempSensorButton AfterButton = NextButton;
					while(AfterButton != null)
					{
						if(AfterButton.SensorMaterial.mainTexture == AfterButton.Solution)
						{
							AfterButton.SensorMaterial.mainTexture = AfterButton.SolutionTexture;
							if(AfterButton.ParticleFX != null)
							{
								Animator anim = AfterButton.ParticleFX.GetComponent<Animator>();
								anim.SetBool(AfterButton.NextState, true);
								/*if(AfterButton.PrevState.Length > 0)
								{
									anim.SetBool(AfterButton.PrevState, false);
								}*/
							}
							
							/*if(AfterButton.PrevButton != null)
							{
								AfterButton.PrevButton.BayMaterial.SetColor("_BaseColor", GlowColor);
							}*/
							AfterButton = AfterButton.NextButton;
						}
						else
						{
							break;
						}
					}
					
					/*for(int i = buttonIndex+1; i < PuzzleButtons.Count; ++i) {
						if(SensorMaterials[i].mainTexture == Solution[i]) {
							SensorMaterials[i].mainTexture = SolutionTextures[i];
							PairedMaterials[i-1].color = GlowColor;
						} else {
							break;
						}
					}*/
				}
			} else {
				
				if(ParticleFX != null) {
					Animator anim = ParticleFX.GetComponent<Animator>();
					anim.SetBool(NextState, false);
					if(PrevState.Length > 0) {
						anim.SetBool(PrevState, true);
					}
				}
				
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
							/*if(AfterButton.PrevState.Length > 0)
							{
								anim.SetBool(AfterButton.PrevState, true);
							}*/
						}
					}
					
					/*if(AfterButton.PrevButton != null)
					{
						AfterButton.PrevButton.SensorMaterial.color = PriorColor;
					}*/
					
					AfterButton = AfterButton.NextButton;
				}
				/*for(int i = buttonIndex; i < PuzzleButtons.Count; ++i) {
					if(SensorMaterials[i].mainTexture == SolutionTextures[i]) {
						SensorMaterials[i].mainTexture = Solution[i];
					}
					
					if(i > 0) {
						PairedMaterials[i-1].color = PriorColors[i-1];
					}
				}*/
			}
        }
    }

}
