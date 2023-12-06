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
    public class ScriptFader : ScriptComponent {
		
        #region Inspector

		[SerializeField]
        private OVRScreenFade Fader;
		
		#endregion // Inspector
		
        #region Leaf
	
        
        private void Awake() {
            
        }
		
        [LeafMember("FadeIn"), Preserve]
        void FadeIn(float duration, float waitTime) {
            if(Fader != null) {
                Fader.FadeIn(duration, waitTime);
            }
        }
		
        [LeafMember("FadeOut"), Preserve]
        void FadeOut(float duration) {
            if(Fader != null) {
                Fader.FadeOut(duration);
            }
        }
        #endregion // Leaf
		
    }
}