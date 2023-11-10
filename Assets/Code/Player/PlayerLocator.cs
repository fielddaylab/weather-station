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

		public void StartTeleportCountdown(Socketable s ) {
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
			} else {

				transform.position = InsideLocation.position;
				transform.rotation = InsideLocation.rotation;
				Sled.transform.position = SledInsideLocation.transform.position;
				Sled.transform.rotation = SledInsideLocation.transform.rotation;
				SocketUtility.TryAddToSocket(ArgoInsideSocket, s, false);
			}
			
			IsInside = !IsInside;
			IsTeleporting = false;
		}
    }
}