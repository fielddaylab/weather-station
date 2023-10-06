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
		public float GripAmount = 0.0f;
		public List<Transform> GrabSpots = new List<Transform>(8);
        #endregion // Inspector
		
		[NonSerialized] public bool IsGrabPosed = false;
		//[NonSerialized] public bool UsedGravity = false;
		
		private void Awake() {
			//set the grip parameter here...
			Animator a = GetComponent<Animator>();
			if(a != null) {
				a.SetFloat(Animator.StringToHash("Flex"), GripAmount);
			}
		}
	}
}
