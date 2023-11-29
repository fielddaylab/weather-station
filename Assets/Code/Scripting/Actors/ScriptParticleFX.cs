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

namespace WeatherStation.Scripting {
    public class ScriptParticleFX : ScriptObject {
        #region Inspector

        #endregion // Inspector
		
		private ParticleSystem m_PS;
		
        #region ILeafActor

        #endregion // ILeafActor

        #region Leaf

        [LeafMember("Play")]
        public void Play() {
			if(m_PS == null) {
				m_PS = transform.GetComponentInChildren<ParticleSystem>();
			}
			
			if(m_PS != null) {
				m_PS.Play();
			}
        }

        [LeafMember("Stop")]
        public void Stop() {
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