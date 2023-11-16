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
        #endregion // Inspector
		
		[NonSerialized] public bool IsGrabPosed = false;
		[NonSerialized] public bool ConstrainGripPosition = false;
		[NonSerialized] public Vector3 ConstrainedGripPosition = Vector3.zero;
		[NonSerialized] public Transform ConstrainedGripTransform;
		
		private Transform OriginalParent;
		
		private void Awake() {
			OriginalParent = transform.parent;
		}
		
		public void SetToOriginalParent() {
			transform.parent = OriginalParent;
		}
	}
}
