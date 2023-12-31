using System;
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
    public class ScriptParticleFX : ScriptComponent {
		
        #region Inspector
		[SerializeField, Required] private ParticleSystem m_PS = null;
        #endregion // Inspector
		
        #region ILeafActor

        #endregion // ILeafActor

        #region Leaf

        [LeafMember("PlayFX"), Preserve]
        public void PlayFX() {
			
			if(m_PS != null) {
				m_PS.Play();
			}
        }

        [LeafMember("StopFX"), Preserve]
        public void StopFX() {
            if(m_PS != null) {
				m_PS.Stop();
			}
        }


        #endregion // Leaf

        #region Unity Events

#if UNITY_EDITOR

#endif // UNITY_EDITOR

        #endregion // Unity Events
    }
}