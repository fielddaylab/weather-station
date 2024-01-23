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
		public List<string> m_ArgoSpeaker = new List<string> (64);
		
		[SerializeField] private ArgoHelp m_ArgoHelp;
		[SerializeField] private PuzzleButton m_Button;
		[SerializeField] private SubtitleDisplay m_SubTitles;
		
		private int m_CurrentClip = 0;
		private int m_EndCurrentClip = 6;
		
		private AudioSource ArgoAudio = null;
		
		// Start is called before the first frame update
		private void Awake() {
			ArgoAudio = GetComponent<AudioSource>();
		}
		
		public void SetCurrentClip(int clip) { m_CurrentClip = clip; }
		public void SetEndClip(int endClip) { m_EndCurrentClip = endClip; }
		
		// Update is called once per frame
		public void UpdateStates() {
			
		}
		
		public void ArgoFunPressed() {
			
			if(!m_Button.Locked && m_Button.WasPressed && !m_ArgoHelp.HelpIsPlaying) {
				
				if(m_CurrentClip != -1 && m_EndCurrentClip != -1) {
					//Debug.Log("Fun pressed");
					StartCoroutine(StartFunProcess());
				}
			}
		}
		
		IEnumerator StartFunProcess() {
			
			ArgoAudio.Stop();
			
			while(m_CurrentClip <= m_EndCurrentClip) {	
				//Debug.Log("Fun pressed " + m_CurrentClip);
				m_SubTitles.ClipDisplayLength = m_ArgoFunAudio[m_CurrentClip].length;
				ArgoAudio.clip = m_ArgoFunAudio[m_CurrentClip];
				ArgoAudio.Play();

				if(m_SubTitles != null) {
					yield return StartCoroutine(m_SubTitles.TypeLineString(m_ArgoSpeaker[m_CurrentClip], m_ArgoFunSubtitles[m_CurrentClip]));
				}
				m_CurrentClip++;
			}
			
		}
	}
}