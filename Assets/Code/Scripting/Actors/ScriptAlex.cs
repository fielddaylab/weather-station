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
	[RequireComponent(typeof(AlexAnimation))]
    public class ScriptAlex : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private AlexAnimation m_Alex =null;
        
        private void Awake() {
            m_Alex = GetComponent<AlexAnimation>();
        }
		
		[LeafMember("StartKneeling"), Preserve]
		public void StartKneeling() {
			m_Alex.StopAllAnimations();
			m_Alex.StartKneeling();	
		}
		
        #endregion // Leaf
		
    }
}