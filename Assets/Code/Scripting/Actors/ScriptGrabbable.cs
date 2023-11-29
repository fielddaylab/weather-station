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
	[RequireComponent(typeof(Grabbable))]
    public class ScriptGrabbable : MonoBehaviour, IScriptActorComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private Grabbable m_Grabbable =null;
        
        #region Leaf
		
        private void Awake() {
            m_Grabbable = GetComponent<Grabbable>();
        }

        [LeafMember("SetGrabbable")]
        public void SetGrabbable(bool grabParam) {
			m_Grabbable.GrabEnabled = grabParam;
        }

        #endregion // Leaf
		
    }
}