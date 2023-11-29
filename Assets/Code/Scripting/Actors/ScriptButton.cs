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

    public class ScriptButton : MonoBehaviour, IScriptActorComponent {
		
        #region Inspector
		[SerializeField, Required] private PuzzleButton m_Button =null;
        
		
		#endregion // Inspector
		
        #region Leaf

        [LeafMember("SetButtonLocked")]
        public void SetButtonLocked(bool lockParam) {
			m_Button.Locked = lockParam;
        }

        #endregion // Leaf
		
    }
}