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
	
    public class ScriptPlayerCode : ScriptComponent {
		
        #region Inspector
		[SerializeField]
        AssignPlayerCode _code;
		
		#endregion // Inspector
		
        #region Leaf
		
        private void Awake() {
            
        }
		
        [LeafMember("AssignCode"), Preserve]
        public void AssignCode() {
            if(_code != null)
            {
                if(RunMode.Current == RunMode.ApplicationRunMode.Device) {
			        OGD.Player.ClaimId(_code.GetTempCode(), null, HandleClaimNewIdSuccess, HandleClaimNewIdError);
                }
            }
        }

        private void HandleClaimNewIdSuccess()
        {
            WSAnalytics pa = Find.State<WSAnalytics>();
            if (pa)
            {
                pa.SetUserID(_code.GetTempCode());    
            }
        }

        private void HandleClaimNewIdError(OGD.Core.Error err)
        {
            Debug.LogError(err.ToString());
        }

        #endregion // Leaf
		
    }
}