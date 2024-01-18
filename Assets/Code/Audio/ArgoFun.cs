using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using FieldDay.Scripting;
using Leaf.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation {
	[RequireComponent(typeof(AudioSource))]
public class ArgoFun : SharedStateComponent {

	public List<AudioClip> m_ArgoFunAudio = new List<AudioClip>(64);
	public List<string> m_ArgoFunSubtitles = new List<string> (64);
	
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
		
	}
	
	public void ArgoFunPressed() {
		
		if(!m_Button.Locked && m_Button.WasPressed) {
			if(m_CurrentClip != -1) {
				ArgoAudio.Stop();
				ArgoAudio.clip = m_ArgoFunAudio[m_CurrentClip];
				ArgoAudio.Play();
				//AudioSource.PlayClipAtPoint(m_ArgoHelpAudio[m_CurrentClip], transform.position);
				if(m_SubTitles != null) {
					StartCoroutine(m_SubTitles.TypeLineString("Argo", m_ArgoFunSubtitles[m_CurrentClip]));
				}
			}
		}
	}
}
}