using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    
    public class GrabPose : BatchedComponent {
		#region Inspector
        public Grabber GrabbableBy;
        public GameObject GrabberVisual;
		public GameObject GrabberTracked;
        #endregion // Inspector
		
		[NonSerialized] public bool IsGrabPosed = false;
		[NonSerialized] public bool UsedGravity = false;
		
		private void Awake() {

		}
	}
}
