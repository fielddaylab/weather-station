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
public class ArgoHelp : SharedStateComponent {

	public List<AudioClip> m_ArgoHelpAudio = new List<AudioClip>(20);
	public List<string> m_ArgoHelpSubtitles = new List<string> (20);
	
	[SerializeField] private PuzzleButton m_Button;
	[SerializeField] private SubtitleDisplay m_SubTitles;
	
	private int m_CurrentClip = -1;
	
    // Start is called before the first frame update
    private void Awake() {
        
    }
	
	[LeafMember("SetCurrentClip"), Preserve]
	public void SetCurrentClip(int clip) { m_CurrentClip = clip; }
	
    // Update is called once per frame
    public void UpdateStates() {
		
	}
	
	public void ArgoHelpPressed() {
		
		if(!m_Button.Locked && m_Button.WasPressed) {
			if(m_CurrentClip == -1) {
				m_CurrentClip = 0;
			}
			
			if(m_CurrentClip != -1) {
				AudioSource.PlayClipAtPoint(m_ArgoHelpAudio[m_CurrentClip], transform.position);
				if(m_SubTitles != null) {
					StartCoroutine(m_SubTitles.TypeLineString("Argo:", m_ArgoHelpSubtitles[m_CurrentClip]));
				}
			}
		}
	}
}
}