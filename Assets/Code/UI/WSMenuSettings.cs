using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;

namespace WeatherStation
{
	public class WSMenuSettings : MonoBehaviour
	{
		public enum WSMenuOption
		{
			CAPTIONS,
			VOLUME,
			//LANGUAGE
		};

		SubtitleDisplay _subTitles;
		
		Camera _musicCamera = null;
		
		int _currentLabel = 0;
		int[] _currentOption = new int[] {0,1,0};
		int[][] _currentOptionChoice = { new int[] {0,1},
								new int[] {0,1,2},
								new int[] {0,1}
							  };
		Color32 LabelHighlightColor = new Color32(255,251,0,255);
		Color32 LabelUnhighlightColor = new Color32(121,141,123,255);

		Color32 OptionHighlightColor = new Color32(232, 239, 210, 255);
		Color32 OptionUnhighlightColor = new Color32(109, 128, 111, 255);
		
		[SerializeField]
		TMPro.TextMeshPro _playerCodeText;

		// Start is called before the first frame update
		void Start()
		{
			_musicCamera = Camera.main;
			_subTitles = _musicCamera.transform.parent.GetChild(_musicCamera.transform.parent.childCount-1).GetComponent<SubtitleDisplay>();
			//WSAnalytics w = Find.State<WSAnalytics>();
			//if(w != null)
			//{
				//_playerCodeText.text = "Player Code: " + w.GetSessionID().ToString();
			//}
		}

		// Update is called once per frame
		void Update()
		{
			
		}

		[ContextMenu("MoveDown")]
		public void MoveMenuDown()
		{
			GameObject labelParent = transform.GetChild(0).gameObject;
			if(labelParent.transform.childCount-1 > _currentLabel+1)        //take off -1 once language option available.
			{
				labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelUnhighlightColor;
				_currentLabel+=1;
				labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelHighlightColor;
			}

			AdjustSettings();
		}

		[ContextMenu("MoveUp")]
		public void MoveMenuUp()
		{
			GameObject labelParent = transform.GetChild(0).gameObject;
			if(_currentLabel - 1 >= 0 )
			{
				labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelUnhighlightColor;
				_currentLabel -= 1;
				labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelHighlightColor;
			}
			
			AdjustSettings();
		}

		[ContextMenu("MoveRight")]
		public void MoveOptionRight()
		{
			GameObject optionParent = transform.GetChild(1).gameObject;
			if(_currentOption[_currentLabel] + 1 < _currentOptionChoice[_currentLabel].Length)
			{
				optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionUnhighlightColor;
				_currentOption[_currentLabel] +=1;
				optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionHighlightColor;
			}

			AdjustSettings();
		}

		[ContextMenu("MoveLeft")]
		public void MoveOptionLeft()
		{
			GameObject optionParent = transform.GetChild(1).gameObject;
			if(_currentOption[_currentLabel] - 1 >= 0)
			{
				optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionUnhighlightColor;
				_currentOption[_currentLabel] -=1;
				optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionHighlightColor;
			}

			AdjustSettings();
		}
		
		void AdjustSettings()
		{
			if(_currentLabel == (uint)WSMenuOption.CAPTIONS)
			{
				if(_currentOption[_currentLabel] == 0)
				{
					//captions off
					if(_subTitles != null)
					{
						_subTitles.SubtitlesOn = false;
					}
				}
				else if(_currentOption[_currentLabel] == 1)
				{
					//captions on
					if(_subTitles != null)
					{
						_subTitles.SubtitlesOn = true;
					}
				}
			}
			else if(_currentLabel == (uint)WSMenuOption.VOLUME)
			{
				if(_currentOption[_currentLabel] == 0)
				{
					//low volume
					if(_musicCamera != null) 
					{
						_musicCamera.GetComponent<AudioSource>().volume = 0.2f;
					}
				}
				else if(_currentOption[_currentLabel] == 1)
				{
					//medium volume
					if(_musicCamera != null) 
					{
						_musicCamera.GetComponent<AudioSource>().volume = 0.6f;
					}
				}
				else if(_currentOption[_currentLabel] == 2)
				{
					//high volume
					if(_musicCamera != null) 
					{
						_musicCamera.GetComponent<AudioSource>().volume = 1.0f;
					}
				}
			}
			//else if(_currentLabel == (uint)WSMenuOption.LANGUAGE)
			//{
				
			//}
		}
	}
}