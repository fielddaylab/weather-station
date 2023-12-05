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
	[RequireComponent(typeof(PuzzleSocket))]
    public class ScriptPuzzleSocket : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private PuzzleSocket m_Socket=null;
        
        private void Awake() {
            m_Socket = GetComponent<PuzzleSocket>();
        }
		
		public bool IsSocketed() { return m_Socket.Current != null; }
		public bool IsSocketedAndMatched() { return m_Socket.Current != null && m_Socket.IsMatched(); }
		
        [LeafMember("SetLocked"), Preserve]
        public void SetLocked(bool lockParam) {
			m_Socket.Locked = lockParam;
        }

		[LeafMember("NotIsPuzzleSocketed"), Preserve]
		static public bool NotIsPuzzleSocketed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptPuzzleSocket ss = ((ScriptObject)act).gameObject.GetComponent<ScriptPuzzleSocket>();
				if(ss != null) {
					return !ss.IsSocketed();
				}
			}
			
			return false;
		}
		
		[LeafMember("IsNotMatched"), Preserve]
		static public bool IsNotMatched(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptPuzzleSocket ss = ((ScriptObject)act).gameObject.GetComponent<ScriptPuzzleSocket>();
				if(ss != null) {
					return !ss.IsSocketedAndMatched();
				}
			}
			
			return false;
		}
        #endregion // Leaf
		
    }
}