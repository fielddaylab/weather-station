using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    
    public class GrabSpot : BatchedComponent {
		#region Inspector
        public int GrabPoseIndex = -1;
        public bool IsRight = true;
        #endregion // Inspector
		
		private void Awake() {
			
		}
	}
}
