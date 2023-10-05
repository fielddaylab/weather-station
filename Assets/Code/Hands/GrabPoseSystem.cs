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
				component.gameObject.transform.position = component.GrabberVisual.transform.position;
				component.gameObject.transform.rotation = component.GrabberVisual.transform.rotation;
				component.gameObject.transform.parent.SetParent(component.GrabberVisual.transform.parent);
			}
        }
	}
}
