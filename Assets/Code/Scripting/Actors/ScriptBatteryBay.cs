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
	[RequireComponent(typeof(BatterySocket))]
    public class ScriptBatteryBay : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private BatterySocket m_Socket=null;
        
        private void Awake() {
            m_Socket = GetComponent<BatterySocket>();
        }
		
		[LeafMember("OpenCover"), Preserve]
		public void OpenCover() {
			m_Socket.OpenCover();	
		}
		
		[LeafMember("CloseCover"), Preserve]
		public void CloseCover() {
			m_Socket.CloseCover();	
		}
		
		[LeafMember("LockBase"), Preserve]
		public void LockBase(bool lockBase) {
			m_Socket.LockBase(lockBase);
		}

        #endregion // Leaf
		
    }
}