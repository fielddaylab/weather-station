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
	[RequireComponent(typeof(PlayerLocator))]
    public class ScriptLocation : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private PlayerLocator m_Locator =null;
        
        #region Leaf
		
        private void Awake() {
            m_Locator = GetComponent<PlayerLocator>();
        }
		
		[LeafMember("Teleport"), Preserve]
        public void Teleport() {
			m_Locator.Teleport();
        }
		
		[LeafMember("SetFinalLocation"), Preserve]
		public void SetFinalLocation() {
			m_Locator.SetFinalLocation();
		}
		
        #endregion // Leaf
		
    }
}