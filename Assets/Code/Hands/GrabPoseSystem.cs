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
				int closestSpot = -1;
				float dist = 9999f;
				for(int i = 0; i < component.GrabSpots.Count; ++i) {
					float currDist = Vector3.Distance(component.GrabberVisual.transform.position, component.GrabSpots[i].position);
					if(currDist < dist) {
						dist = currDist;
						closestSpot = i;
					}
				}
				
				if(closestSpot != -1) {
					//for position, instead walk through list of possible grab points... attach to closest...
					component.gameObject.transform.position = component.GrabSpots[closestSpot].transform.position;
				}
				
				component.gameObject.transform.rotation = component.GrabberVisual.transform.rotation;
				component.gameObject.transform.parent.SetParent(component.GrabberVisual.transform.parent);
			}
        }
	}
}
