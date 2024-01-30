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
    public class ScriptTexture : ScriptComponent {
		
        #region Inspector
		
		[SerializeField, Required] private int m_MaterialIndex=0;
        [SerializeField, Required] private MeshRenderer m_MR=null;
		
		public List<Texture2D> Textures = new List<Texture2D>(8);
        
		#endregion // Inspector
		
		private int m_CurrentTexture = -1;
		
		private void Awake() {
			
		}
		
        #region Leaf
		
        [LeafMember("NextTexture"), Preserve]
        public void NextTexture() {
			m_CurrentTexture++;
			if(m_CurrentTexture < Textures.Count) {
				m_MR.materials[m_MaterialIndex].mainTexture = Textures[m_CurrentTexture];
			}
        }
		
		[LeafMember("SetTexture"), Preserve]
		public void SetTexture(int textureID) {
			m_CurrentTexture = textureID;
			if(m_CurrentTexture < Textures.Count) {
				m_MR.materials[m_MaterialIndex].mainTexture = Textures[m_CurrentTexture];
			}			
		}

        #endregion // Leaf

        #region Unity Events

#if UNITY_EDITOR

#endif // UNITY_EDITOR

        #endregion // Unity Events
    }
}