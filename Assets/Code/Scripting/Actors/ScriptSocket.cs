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
	[RequireComponent(typeof(ItemSocket))]
    public class ScriptSocket : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private ItemSocket m_Socket=null;
        
        private void Awake() {
            m_Socket = GetComponent<ItemSocket>();
        }
		
		public bool IsSocketed() { return m_Socket.Current != null; }

        [LeafMember("SetLocked"), Preserve]
        public void SetLocked(bool lockParam) {
			m_Socket.Locked = lockParam;
        }

		[LeafMember("NotIsSocketed"), Preserve]
		static public bool NotIsSocketed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptSocket ss = ((ScriptObject)act).gameObject.GetComponent<ScriptSocket>();
				if(ss != null) {
					return !ss.IsSocketed();
				}
			}
			
			return false;
		}
		
		[LeafMember("IsSocketed"), Preserve]
		static public bool IsSocketed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptSocket ss = ((ScriptObject)act).gameObject.GetComponent<ScriptSocket>();
				if(ss != null) {
					return ss.IsSocketed();
				}
			}
			
			return false;
		}
        #endregion // Leaf
		
    }
}