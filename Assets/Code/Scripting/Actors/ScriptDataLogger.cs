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
	[RequireComponent(typeof(DataLoggerDoor))]
    public class ScriptDataLogger : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		private DataLoggerDoor m_Source =null;
        
        #region Leaf
		
        private void Awake() {
            m_Source = GetComponent<DataLoggerDoor>();
        }
		
		public bool WasGrabbed() {
			return m_Source.WasGrabbed;
		}
		
		[LeafMember("DoorWasNotGrabbed"), Preserve]
		static bool DoorWasNotGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptDataLogger sb = ((ScriptObject)act).gameObject.GetComponent<ScriptDataLogger>();
				if(sb != null) {
					return !sb.WasGrabbed();
				}
			}
			
			return false;
		}
		
		[LeafMember("DoorWasGrabbed"), Preserve]
		static bool DoorWasGrabbed(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptDataLogger sb = ((ScriptObject)act).gameObject.GetComponent<ScriptDataLogger>();
				if(sb != null) {
					return sb.WasGrabbed();
				}
			}
			
			return false;
		}
		
        #endregion // Leaf
		
    }
}