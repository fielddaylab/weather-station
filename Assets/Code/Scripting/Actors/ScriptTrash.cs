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
	[RequireComponent(typeof(TrashChute))]
    public class ScriptTrash : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private TrashChute m_Source =null;
        
        #region Leaf
		
        private void Awake() {
            m_Source = GetComponent<TrashChute>();
        }
		
		public bool WasGrabbed() {
			return m_Source.WasGrabbed;
		}
		
		[LeafMember("TrashWasNotGrabbed"), Preserve]
		static bool TrashWasGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptTrash sb = ((ScriptObject)act).gameObject.GetComponent<ScriptTrash>();
				if(sb != null) {
					return !sb.WasGrabbed();
				}
			}
			
			return false;
		}
		
        #endregion // Leaf
		
    }
}