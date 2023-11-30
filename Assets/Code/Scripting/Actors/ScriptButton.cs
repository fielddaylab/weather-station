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

    public class ScriptButton : ScriptComponent {
		
        #region Inspector
		[SerializeField, Required] private PuzzleButton m_Button = null;
        
		#endregion // Inspector
		
        #region Leaf
		
		public bool WasButtonPressed() { return ((m_Button != null) && m_Button.WasPressed); }
		
        [LeafMember("SetButtonLocked")]
        public void SetButtonLocked(bool lockParam) {
			m_Button.Locked = lockParam;
        }
		
		//public bool ButtonNotPressed() {
		//	return !m_Button.WasPressed;
		//}
		
		[LeafMember("ButtonNotPressed")]
		static bool ButtonNotPressed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptButton sb = ((ScriptObject)act).gameObject.GetComponent<ScriptButton>();
				if(sb != null) {
					return !sb.WasButtonPressed();
				}
			}
			
			return false;
		}

        #endregion // Leaf
		
    }
}