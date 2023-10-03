using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class GrabPoseSystem : ComponentSystemBehaviour<GrabPose> {
		
		public override void ProcessWorkForComponent(GrabPose component, float deltaTime) {
			if(component.IsGrabPosed) {
				//update transform of grabbed object to match that of the tracked 
				GameObject rawGrab = component.GrabberTracked;
				component.gameObject.transform.parent.rotation = rawGrab.transform.rotation;
			}
        }

	}
}
