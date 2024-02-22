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
		private Socketable BrokenProp;
		private WindSocket SocketRotation = null;

		private const float ROTATE_SPEED = 10f;
		
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
					BrokenProp = BladeSocket.Current;
				}
			}
		}
				
		private void OnSensorRemoved() {
			if(FanBlade != null) {
				IsStopped = true;
			}
			
			//TestButton.Untoggle();
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
							StartCoroutine(RotateBlade(120f, ROTATE_SPEED, true));
						}
					} else if(BladeSocket.Current == BrokenProp) {
						if(!IsTesting) {
							//Debug.Log("Testing broken");
							IsTesting = true;
							//BladeSocket.Locked = true;
							//rotate a bit, then have it detach and fall..
							StartCoroutine(RotateBlade(4f, ROTATE_SPEED, false));
						}
						else
						{
							IsStopped = true;
						}				
					} else {
						if(!IsTesting) {
							//Debug.Log("Testing wrong");
							IsTesting = true;
							//BladeSocket.Locked = true;
							//rotate a bit, then have it detach and fall..
							StartCoroutine(RotateBlade(8f, ROTATE_SPEED, false));
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
			else
			{
				TestButton.Untoggle();
			}
        }

		private IEnumerator RotateBlade(float duration, float angle, bool complete) {
			
			if(complete) {
				AudioSource audioSource = GetComponent<AudioSource>();
				if(audioSource != null && audioSource.clip != null) {
					audioSource.Play();
				}
			}
			
			if(SocketRotation == null) {
				SocketRotation = BladeSocket.gameObject.transform.GetChild(1).gameObject.GetComponent<WindSocket>();
			}
			
			Socket.Locked = !complete;
			
            float t = 0f;
            while(t < duration && !IsStopped) {
				//Debug.Log("Rotating");
				if(BladeSocket.Current)
				{
					BladeSocket.Current.gameObject.transform.RotateAround(SocketRotation.gameObject.transform.position, BladeSocket.gameObject.transform.right, angle);
					FanBlade.transform.RotateAround(FanRotate.position, -FanBlade.transform.forward, angle);
				}
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
			
            //unsocket and have it fall to the ground...
			if(!complete) {
				if(BladeSocket.Current != null) {
					SocketUtility.TryReleaseFromCurrentSocket(BladeSocket.Current, true);
				}
			}
			
			if(TestButton.IsOn())
			{
				TestButton.Untoggle();
			}
			
			IsTesting = false;
			IsStopped = false;
        }
		
	}
}