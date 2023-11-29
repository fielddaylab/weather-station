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
	[RequireComponent(typeof(ItemSocket))]
    public class ScriptSocket : MonoBehaviour, IScriptActorComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private ItemSocket m_Socket=null;
        
        private void Awake() {
            m_Socket = GetComponent<ItemSocket>();
        }

        [LeafMember("SetLocked")]
        public void SetLocked(bool lockParam) {
			m_Socket.Locked = lockParam;
        }

        #endregion // Leaf
		
    }
}