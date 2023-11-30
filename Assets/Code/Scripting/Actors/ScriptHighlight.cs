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

namespace WeatherStation.Scripting {
    public class ScriptHighlight : ScriptComponent {
		
        #region Inspector
		
		[SerializeField, Required] private int m_MaterialIndex=0;
        [SerializeField, Required] private MeshRenderer m_MR=null;
        
		#endregion // Inspector
		
		private static Color HighlightOutline = new Color(0.12549f, 0.93725f, 0.294117f, 1f);
		private Color CurrentOutline;
		private bool Highlighting = false;
		
		private void Awake() {
			CurrentOutline = m_MR.materials[m_MaterialIndex].GetColor("_OutlineColor");
		}
		
        #region Leaf
		
        [LeafMember("ShowHighlight")]
        public void ShowHighlight() {
			if(!Highlighting) {
				Highlighting = true;
				StartCoroutine("PulseHighlight");
			}
			
        }

        [LeafMember("StopHighlight")]
        public void StopHighlight() {
			Highlighting = false;
        }

        #endregion // Leaf
		
		IEnumerator PulseHighlight() {
			while(Highlighting) {
				m_MR.materials[m_MaterialIndex].SetColor("_OutlineColor", Color.Lerp(CurrentOutline, HighlightOutline, Mathf.PingPong(Time.time, 1f)));
				yield return null;
			}
			m_MR.materials[m_MaterialIndex].SetColor("_OutlineColor", CurrentOutline);
		}
		
        #region Unity Events

#if UNITY_EDITOR

#endif // UNITY_EDITOR

        #endregion // Unity Events
    }
}