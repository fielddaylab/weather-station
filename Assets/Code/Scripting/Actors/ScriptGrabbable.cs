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
	[RequireComponent(typeof(Grabbable))]
    public class ScriptGrabbable : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private Grabbable m_Grabbable =null;
        
        #region Leaf
		
        private void Awake() {
            m_Grabbable = GetComponent<Grabbable>();
        }
		
		public bool WasGrabbed() { return m_Grabbable.WasGrabbed; }
		public bool IsGrabbed() { return m_Grabbable.CurrentGrabberCount > 0; }
        
		public bool IsSocketed() {
			Socketable s = m_Grabbable.gameObject.GetComponent<Socketable>();
			if(s != null) {
				return (s.CurrentSocket != null);
			}
			
			return false;
		}
		
		[LeafMember("SetGrabbable"), Preserve]
        public void SetGrabbable(bool grabParam) {
			m_Grabbable.GrabEnabled = grabParam;
        }
		
		[LeafMember("SetWasGrabbed"), Preserve]
        public void SetWasGrabbable(bool grabParam) {
			m_Grabbable.WasGrabbed = grabParam;
        }		
		
		[LeafMember("IsGrabbableSocketed"), Preserve]
		static public bool IsGrabbableSocketed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptGrabbable sg = ((ScriptObject)act).gameObject.GetComponent<ScriptGrabbable>();
				if(sg != null) {
					return sg.IsSocketed();
				}
			}
			
			return false;
		}
		
		[LeafMember("NotIsGrabbed"), Preserve]
		static public bool NotIsGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptGrabbable sg = ((ScriptObject)act).gameObject.GetComponent<ScriptGrabbable>();
				if(sg != null) {
					return !sg.IsGrabbed();
				}
			}
			
			return false;
		}
		
		[LeafMember("IsGrabbed"), Preserve]
		static public bool IsGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptGrabbable sg = ((ScriptObject)act).gameObject.GetComponent<ScriptGrabbable>();
				if(sg != null) {
					return sg.IsGrabbed();
				}
			}
			
			return false;
		}
		
		
		[LeafMember("NotWasGrabbed"), Preserve]
		static public bool NotWasGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptGrabbable sg = ((ScriptObject)act).gameObject.GetComponent<ScriptGrabbable>();
				if(sg != null) {
					return !sg.WasGrabbed();
				}
			}
			
			return false;
		}

		[LeafMember("WasGrabbed"), Preserve]
		static public bool WasGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptGrabbable sg = ((ScriptObject)act).gameObject.GetComponent<ScriptGrabbable>();
				if(sg != null) {
					return sg.WasGrabbed();
				}
			}
			
			return false;
		}
        #endregion // Leaf
		
    }
}