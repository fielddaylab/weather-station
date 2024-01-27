using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;
using Leaf.Runtime;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Scripting;
using BeauRoutine;

namespace WeatherStation {
	[RequireComponent(typeof(AudioSource))]
public class ArgoHelp : SharedStateComponent {

	public List<AudioClip> m_ArgoHelpAudio = new List<AudioClip>(64);
	public List<string> m_ArgoHelpSubtitles = new List<string> (64);
	
	[NonSerialized] public bool HelpIsPlaying = false;
	
	[SerializeField] private PuzzleButton m_Button;
	[SerializeField] private SubtitleDisplay m_SubTitles;
	
	private int m_CurrentClip = -1;
	
	private AudioSource ArgoAudio = null;
	
    // Start is called before the first frame update
    private void Awake() {
        ArgoAudio = GetComponent<AudioSource>();
    }
	
	public void SetCurrentClip(int clip) { m_CurrentClip = clip; }
	
    // Update is called once per frame
    public void UpdateStates() {
		if(!ArgoAudio.isPlaying)
		{
			HelpIsPlaying = false;
		}
	}
	
	public void ArgoHelpPressed() {
		
		if(!m_Button.Locked && m_Button.WasPressed) {
			if(m_CurrentClip != -1) {
				ArgoAudio.Stop();
				ArgoAudio.clip = m_ArgoHelpAudio[m_CurrentClip];
				ArgoAudio.Play();
				HelpIsPlaying = true;
				//AudioSource.PlayClipAtPoint(m_ArgoHelpAudio[m_CurrentClip], transform.position);
				if(m_SubTitles != null) {
					
					m_SubTitles.ClipDisplayLength = m_ArgoHelpAudio[m_CurrentClip].length;
					if(m_SubTitles.SubtitlesOn) {
						StartCoroutine(m_SubTitles.TypeLineString("Argo", m_ArgoHelpSubtitles[m_CurrentClip]));
					} else {
						RoutineShortcuts.WaitToComplete(ArgoAudio);
					}
				}
			}
		}
	}
}
}