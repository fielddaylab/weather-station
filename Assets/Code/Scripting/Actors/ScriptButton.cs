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

    public class ScriptButton : ScriptComponent {
		
        #region Inspector
		[SerializeField, Required] private PuzzleButton m_Button = null;
        
		#endregion // Inspector
		
        #region Leaf
		
		private void Awake() {
			if(m_Button == null) {
				m_Button = GetComponent<PuzzleButton>();
			}
        }
		
		public bool WasButtonPressed() { return ((m_Button != null) && m_Button.WasPressed); }
		
        [LeafMember("SetButtonLocked"), Preserve]
        public void SetButtonLocked(bool lockParam) {
			m_Button.Locked = lockParam;
        }
		
		[LeafMember("SetButtonPressed"), Preserve]
        public void SetButtonPressed(bool pressed) {
			m_Button.WasPressed = pressed;
        }
		
		[LeafMember("SetCurrentClip"), Preserve]
		public void SetCurrentClip(int clipIndex) {
			ArgoHelp argo = Find.State<ArgoHelp>();
			if(argo != null) {
				argo.SetCurrentClip(clipIndex);
			}
		}

		[LeafMember("ButtonNotPressed"), Preserve]
		static bool ButtonNotPressed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptButton sb = ((ScriptObject)act).gameObject.GetComponent<ScriptButton>();
				if(sb != null) {
					return !sb.WasButtonPressed();
				}
			}
			
			return false;
		}
		
		[LeafMember("ButtonPressed"), Preserve]
		static bool ButtonPressed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptButton sb = ((ScriptObject)act).gameObject.GetComponent<ScriptButton>();
				if(sb != null) {
					return sb.WasButtonPressed();
				}
			}
			
			return false;
		}
		
        #endregion // Leaf
		
    }
}