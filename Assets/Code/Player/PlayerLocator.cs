using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class PlayerLocator : SharedStateComponent {
		#region Inspector
		public Transform InsideLocation;
		public Transform OutsideLocation;
		//public Transform InsideSledLocation;
		//public Transform OutsideSledLocation;
		public Vector3 SledOffset;
		public GameObject Sled;
		public OVRScreenFade Fader;
		#endregion //
		
		//[NonSerialized] public HashSet<Grabbable> ItemsInSled = new HashSet<Grabbable>(8);
		
		private bool IsInside = false;
		private bool IsTeleporting = false;
		private bool IsArgoIn = false;
		
		private void Awake() {
	
		}
		
		/*public void ItemAddedToSled(Collider c) {
			Grabbable g = c.gameObject.GetComponent<Grabbable>();
			Socketable s = c.gameObject.GetComponent<Socketable>();
			if(g.CurrentGrabberCount == 0 && s.CurrentSocket == null)	//make sure we aren't holding the object still, and that this object isn't socketed to a parent.
			{
				//Debug.Log(c.gameObject.name + " added");
				ItemsInSled.Add(c.gameObject.GetComponent<Grabbable>());
				if(c.gameObject.name.Contains("Argo")) {
					//c.isTrigger = false;
					StartTeleportCountdown(c);
				}
			}
		}*/
		
		/*public void ItemRemovedFromSled(Collider c) {
			//Debug.Log(c.gameObject.name + " removed");
			ItemsInSled.Remove(c.gameObject.GetComponent<Grabbable>());
		}*/
		
		public void CheckArgoRemoved(Collider c) {
			if(c.gameObject.name.Contains("Argo")) {
				IsArgoIn = false;
			}
		}
		
		public void StartTeleportCountdown(Collider c) {
			if(c.gameObject.name.Contains("Argo") && !IsArgoIn) {
				if(!IsTeleporting) {
					IsTeleporting = true;
					StartCoroutine(WaitForTeleport(c, 3f));
				}
			}
		}
		
		IEnumerator WaitForTeleport(Collider c, float duration)
		{
			yield return new WaitForSeconds(duration);
			
			if(Fader) {
				Fader.FadeOut(1f);
			}
			
			yield return new WaitForSeconds(1f);
			
			if(Fader) {
				Fader.FadeIn(1f);
			}
			
			if(IsInside) {

				/*foreach(Grabbable g in ItemsInSled) {
					g.gameObject.transform.position -= (SledOffset);
				}*/
				
				transform.position = OutsideLocation.position;
				transform.rotation = OutsideLocation.rotation;
				Sled.transform.position = Sled.transform.position - SledOffset;
			} else {

				/*foreach(Grabbable g in ItemsInSled) {
					g.gameObject.transform.position += (SledOffset);
				}*/
				
				transform.position = InsideLocation.position;
				transform.rotation = InsideLocation.rotation;
				Sled.transform.position = Sled.transform.position + SledOffset;
			}
			
			//c.isTrigger = true;
			IsInside = !IsInside;
			IsArgoIn = true;
			IsTeleporting = false;
		}
    }
}