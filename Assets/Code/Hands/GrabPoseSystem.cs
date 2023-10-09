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
				
				//we want to temporarily set the parent of the grab pose component to the thing we grabbed, but also set the thing we grabbed'd parent to the grabber visual
				component.gameObject.transform.rotation = component.GrabberVisual.transform.rotation;
			}
        }
	}
}
