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

		public GameObject Sled;
		public GameObject PlaneExterior;
		public GameObject PlaneInterior;
		public GameObject ExteriorLight;
		public GameObject InteriorLight;
		public GameObject InteriorLight2;
		public OVRScreenFade Fader;

		public ItemSocket ArgoInsideSocket;

		public ItemSocket ArgoOutsideSocket;
		
		public Socketable Argo;

		public AudioClip OutsideMusic;
		public AudioClip InsideMusic;

		public AudioClip InsideDropEffect;

		public AudioClip OutsideDropEffect;

		#endregion //
		
		private Camera MainCamera = null;	//temp hack for playing music...
		private GameObject HeadRoot = null;
		public bool IsInside = false;
		private bool IsTeleporting = false;

		private void Awake() {

			MainCamera = Camera.main;
			HeadRoot = transform.GetChild(0).gameObject;
			StartCoroutine("InitialAlignment");
		}
		
		public void Teleport() {
			if(Argo != null) {
				StartTeleportCountdown(Argo);
			}
		}
		
		public void SocketArgoOutside()
		{
			SocketUtility.TryAddToSocket(ArgoOutsideSocket, Argo, false);
		}
		
		public void RotatePlayer(bool left)
		{
			//rotate the player from their current y-orientation left or right 30 degrees...
			transform.RotateAround(HeadRoot.transform.position, Vector3.up, left ? -30f : 30f);
		}
		
		public void StartTeleportCountdown(Socketable s) {
			
			//we should return any item in your hand to their original location before teleporting...
			PlayerHandRig handRig = Lookup.State<PlayerHandRig>();
			if(handRig.LeftHand.Physics.State == GrabberState.Holding) {
				Grabbable h = handRig.LeftHand.Physics.Holding;
				Socketable s2 = h.gameObject.GetComponent<Socketable>();
				if(s2 != s) {
					if(h != null)
					{
						if(h.OriginalSocket.Current == null) {
							SocketUtility.TryAddToSocket(h.OriginalSocket, s2, true);
						} else {
							GrabUtility.ReturnToOriginalSpawnPoint(h);
						}
					}
				}
			}
			
			if(handRig.RightHand.Physics.State == GrabberState.Holding) {
				Grabbable h = handRig.RightHand.Physics.Holding;
				Socketable s2 = h.gameObject.GetComponent<Socketable>();
				if(s2 != s) {
					if(h != null)
					{
						if(h.OriginalSocket.Current == null) {
							SocketUtility.TryAddToSocket(h.OriginalSocket, s2, true);
						} else {
							GrabUtility.ReturnToOriginalSpawnPoint(h);
						}
					}
				}
			}
			
			if(s.SocketType == SocketFlags.Argo) {
				if(!IsTeleporting) {
					IsTeleporting = true;
					StartCoroutine(WaitForTeleport(s, 1f));
				}
			}
		}
		
		IEnumerator InitialAlignment()
		{
			yield return new WaitForSeconds(2f);
			
			Vector3 headPos = transform.GetChild(0).transform.localPosition;
			Quaternion localHead = transform.GetChild(0).transform.localRotation;
			
			headPos.y = 0f;
			
			Quaternion qInv = (OutsideLocation.rotation * Quaternion.Inverse(localHead));
			
			Vector3 euler = qInv.eulerAngles;
			euler.x = 0f;
			euler.z = 0f;
			qInv.eulerAngles = euler;
			
			headPos = qInv * headPos;
			headPos.y = 0f;
			
			transform.position = OutsideLocation.position - headPos;
			transform.rotation = qInv;	
		}
		
		IEnumerator WaitForTeleport(Socketable s, float duration)
		{
			yield return new WaitForSeconds(duration);
			
			/*if(Fader) {
				Fader.FadeOut(1f);
			}
			
			yield return new WaitForSeconds(1f);
			
			if(Fader) {
				Fader.FadeIn(1f);
			}*/

			//release Argo from the Sled
			SocketUtility.TryReleaseFromCurrentSocket(s, false);
			
			if(IsInside) {
				
				Vector3 headPos = transform.GetChild(0).transform.localPosition;
				Quaternion localHead = transform.GetChild(0).transform.localRotation;
				
				headPos.y = 0f;
				
				Quaternion qInv = (OutsideLocation.rotation * Quaternion.Inverse(localHead));
				
				Vector3 euler = qInv.eulerAngles;
				euler.x = 0f;
				euler.z = 0f;
				qInv.eulerAngles = euler;
				
				headPos = qInv * headPos;
				headPos.y = 0f;
				
				transform.position = OutsideLocation.position - headPos;
				transform.rotation = qInv;
				
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
				
				if(InteriorLight2 != null) {
					InteriorLight2.SetActive(false);
				}
				
				if(OutsideDropEffect != null) {
					GetComponent<AudioSource>().clip = OutsideDropEffect;
				}
				
				if(MainCamera != null) {
					MainCamera.gameObject.GetComponent<AudioSource>().Stop();
					MainCamera.gameObject.GetComponent<AudioSource>().clip = OutsideMusic;
					MainCamera.gameObject.GetComponent<AudioSource>().Play();
				}
				
				RenderSettings.fog = true;
				
			} else {
				
				Vector3 headPos = transform.GetChild(0).transform.localPosition;
				Quaternion localHead = transform.GetChild(0).transform.localRotation;
				headPos.y = 0f;
				
				//transform.position = InsideLocation.position - (InsideLocation.rotation * Quaternion.Inverse(localHead)) * headPos;
				
				//transform.rotation = InsideLocation.rotation * Quaternion.Inverse(localHead);
				
				//zero out the x and z rotation in case user had head tilted at point of transport
				Quaternion qInv = (InsideLocation.rotation * Quaternion.Inverse(localHead));
				
				Vector3 euler = qInv.eulerAngles;
				euler.x = 0f;
				euler.z = 0f;
				qInv.eulerAngles = euler;
				
				headPos = qInv * headPos;
				headPos.y = 0f;
				
				transform.position = InsideLocation.position - headPos;
				transform.rotation = qInv;
				
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
				
				if(InteriorLight2 != null) {
					InteriorLight2.SetActive(true);
				}
				
				if(InsideDropEffect != null) {
					GetComponent<AudioSource>().clip = InsideDropEffect;
				}
				
				if(MainCamera != null) {
					MainCamera.gameObject.GetComponent<AudioSource>().Stop();
					MainCamera.gameObject.GetComponent<AudioSource>().clip = InsideMusic;
					MainCamera.gameObject.GetComponent<AudioSource>().Play();
				}
				
				RenderSettings.fog = false;
			}
			
			IsInside = !IsInside;
			IsTeleporting = false;
		}
    }
}