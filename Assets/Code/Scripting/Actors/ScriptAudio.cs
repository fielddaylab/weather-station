using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Variants;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using FieldDay.Scripting;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting {
	[RequireComponent(typeof(AudioSource))]
    public class ScriptAudio : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private AudioSource m_Source =null;
        
        #region Leaf
		
        private void Awake() {
            m_Source = GetComponent<AudioSource>();
        }
		
		[LeafMember("PlaySound"), Preserve]
        public void PlaySound() {
			m_Source.Play();
			//if(m_Source.clip != null) {
			//	AudioSource.PlayClipAtPoint(m_Source.clip, transform.position);
			//}
        }
		
		[LeafMember("StopSound"), Preserve]
        public void StopSound() {
			m_Source.Stop();
        }
		
        #endregion // Leaf
		
    }
}