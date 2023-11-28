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
		public Transform SledInsideLocation;
		public Transform SledOutsideLocation;
		//public Vector3 SledOffset;	//3.585, 0, 0.228
		public GameObject Sled;
		public GameObject PlaneExterior;
		public GameObject PlaneInterior;
		public GameObject ExteriorLight;
		public GameObject InteriorLight;
		public OVRScreenFade Fader;

		public ItemSocket ArgoInsideSocket;

		public ItemSocket ArgoOutsideSocket;

		public ItemSocket ArgoSledSocket;

		#endregion //
		
		private bool IsInside = false;
		private bool IsTeleporting = false;

		private void Awake() {
			if(ArgoSledSocket != null) {
				ArgoSledSocket.OnAdded.Register(StartTeleportCountdown);
			}
		}

		public void StartTeleportCountdown(Socketable s) {
			
			//we should return any item in your hand to their original location before teleporting...
			PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
			if(handRig.LeftHand.Physics.State == GrabberState.Holding) {
				Grabbable h = handRig.LeftHand.Physics.Holding;
				Socketable s2 = h.gameObject.GetComponent<Socketable>();
				if(s2 != s) {
					if(h.OriginalSocket.Current == null) {
						SocketUtility.TryAddToSocket(h.OriginalSocket, s2, true);
					} else {
						GrabUtility.ReturnToOriginalSpawnPoint(h);
					}
				}
			}
			
			if(handRig.RightHand.Physics.State == GrabberState.Holding) {
				Grabbable h = handRig.RightHand.Physics.Holding;
				Socketable s2 = h.gameObject.GetComponent<Socketable>();
				if(s2 != s) {
					if(h.OriginalSocket.Current == null) {
						SocketUtility.TryAddToSocket(h.OriginalSocket, s2, true);
					} else {
						GrabUtility.ReturnToOriginalSpawnPoint(h);
					}
				}
			}
			
			if(s.SocketType == SocketFlags.Argo) {
				if(!IsTeleporting) {
					IsTeleporting = true;
					StartCoroutine(WaitForTeleport(s, 3f));
				}
			}
		}
		
		IEnumerator WaitForTeleport(Socketable s, float duration)
		{
			yield return new WaitForSeconds(duration);
			
			if(Fader) {
				Fader.FadeOut(1f);
			}
			
			yield return new WaitForSeconds(1f);
			
			if(Fader) {
				Fader.FadeIn(1f);
			}

			//release Argo from the Sled
			SocketUtility.TryReleaseFromCurrentSocket(s, false);
			
			if(IsInside) {

				transform.position = OutsideLocation.position;
				transform.rotation = OutsideLocation.rotation;
				Sled.transform.position = SledOutsideLocation.transform.position;
				Sled.transform.rotation = SledOutsideLocation.transform.rotation;

				SocketUtility.TryAddToSocket(ArgoOutsideSocket, s, false);
				
				if(PlaneExterior != null) {
					PlaneExterior.SetActive(true);
				}
				
				if(PlaneInterior != null) {
					PlaneInterior.SetActive(false);
				}
				
				if(ExteriorLight != null) {
					ExteriorLight.SetActive(true);
				}
				
				if(InteriorLight != null) {
					InteriorLight.SetActive(false);
				}
			} else {

				transform.position = InsideLocation.position;
				transform.rotation = InsideLocation.rotation;
				Sled.transform.position = SledInsideLocation.transform.position;
				Sled.transform.rotation = SledInsideLocation.transform.rotation;
				SocketUtility.TryAddToSocket(ArgoInsideSocket, s, false);
				
				if(PlaneExterior != null) {
					PlaneExterior.SetActive(false);
				}
				
				if(PlaneInterior != null) {
					PlaneInterior.SetActive(true);
				}
				
				if(ExteriorLight != null) {
					ExteriorLight.SetActive(false);
				}
				
				if(InteriorLight != null) {
					InteriorLight.SetActive(true);
				}
			}
			
			IsInside = !IsInside;
			IsTeleporting = false;
		}
    }
}