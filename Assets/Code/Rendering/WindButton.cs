using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class WindButton : MonoBehaviour
	{
		[SerializeField] GameObject FanBlade;
		[SerializeField] Transform FanRotate;
		[SerializeField] ItemSocket Socket;
		[SerializeField] PuzzleButton TestButton;
		
		bool IsStopped = false;
		bool IsTesting = false;
		
		private PuzzleSocket BladeSocket = null;
		private GameObject BrokenProp;
		
		// Start is called before the first frame update
		void Awake() {
			TestButton.OnPressed.Register(TestComplete);
			if(Socket != null) {
				Socket.OnRemoved.Register(OnSensorRemoved);
				Socket.OnAdded.Register(UnlockSocket);
			}
		}

		void Update() {

		}
		
		public void UnlockSocket(Socketable s) {
			Socket.Locked = false;
			if(BladeSocket == null)
			{
				BladeSocket = transform.parent.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<PuzzleSocket>();
				if(BladeSocket != null)
				{
					BrokenProp = BladeSocket.gameObject.transform.GetChild(0).GetChild(0).gameObject;
				}
			}
		}
				
		private void OnSensorRemoved() {
			if(FanBlade != null) {
				IsStopped = true;
			}
			
			TestButton.Untoggle();
			IsTesting = false;
		}
		
        private void TestComplete(PuzzleButton button) {
			
			if(BladeSocket != null)
			{
				//Debug.Log("Testing");
				if(BladeSocket.Current) {
					if(BladeSocket.IsMatched()) {
						//Debug.Log("Testing matched");
						//rotate propeller, highlight green and chime sound...
						if(IsTesting) {
							//stop the propeller
							IsStopped = true;
						} else {
							IsTesting = true;
							BladeSocket.Locked = true;
							StartCoroutine(RotateAndFinish(BladeSocket, BladeSocket.Current, 120f));
						}
					} else if(BladeSocket.Current == BrokenProp) {
						if(!IsTesting) {
							//Debug.Log("Testing broken");
							IsTesting = true;
							BladeSocket.Locked = true;
							//rotate a bit, then have it detach and fall..
							StartCoroutine(RotateAndFall(BladeSocket, BladeSocket.Current, 5f));
						}
						else
						{
							IsStopped = true;
						}				
					} else {
						if(!IsTesting) {
							//Debug.Log("Testing wrong");
							IsTesting = true;
							BladeSocket.Locked = true;
							//rotate a bit, then have it detach and fall..
							StartCoroutine(RotateAndFail(BladeSocket, BladeSocket.Current, 10f));
						}
						else
						{
							IsStopped = true;
						}
					}
				}
				else
				{
					TestButton.Untoggle();
				}
			}
        }

        private IEnumerator RotateAndFall(ItemSocket socket, Socketable socketable, float duration) {
            float t = 0f;
            while(t < duration && !IsStopped) {
				//Debug.Log("Rotating");
				if(Socket.Current)
				{
					SocketUtility.RotateSocketed(socket, socket.Current, 1.5f);
					FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, 1.5f);
				}
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

			Socket.Locked = false;
			
            //unsocket and have it fall to the ground...
			SocketUtility.TryReleaseFromCurrentSocket(socket.Current, true);
			
			TestButton.Untoggle();
			
			
			IsTesting = false;
			IsStopped = false;
        }
		
		private IEnumerator RotateAndFail(ItemSocket socket, Socketable socketable, float duration) {
            float t = 0f;
            while(t < duration && !IsStopped) {
				//Debug.Log("Rotating");
				if(BladeSocket.Current)
				{
					SocketUtility.RotateSocketed(socket, socket.Current, 2.5f);
					FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, 2.5f);
				}
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
			
			Socket.Locked = false;
			
            //unsocket and have it fall to the ground...
			SocketUtility.TryReleaseFromCurrentSocket(Socket.Current, true);
			
			TestButton.Untoggle();
			
			Socket.Locked = false;
			IsTesting = false;
			IsStopped = false;
        }


        private IEnumerator RotateAndFinish(ItemSocket socket, Socketable socketable, float duration) {
            
			AudioSource audioSource = GetComponent<AudioSource>();
			if(audioSource != null && audioSource.clip != null) {
				audioSource.Play();
			}
			
			float t = 0f;
            while(t < duration && !IsStopped) {
                //Debug.Log("Rotating");
				SocketUtility.RotateSocketed(socket, socket.Current, 10f);
				FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, 20f);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
			
			TestButton.Untoggle();
			
			IsTesting = false;
			IsStopped = false;
        }
	}
}