using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class BatterySocket : MonoBehaviour
	{
		[SerializeField] ItemSocket Socket;
		
		private GameObject BatteryCover;
		// Start is called before the first frame update
		void Awake() {
			if(Socket != null) {
				Socket.OnAdded.Register(LockBase);
			}
		}

		public void OpenCover()	 {
			if(BatteryCover != null)
			{
				Animator CoverAnim = BatteryCover.GetComponent<Animator>();
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", true);
				}
			}
		}
		
		public void CloseCover() {
			if(BatteryCover != null)
			{
				Animator CoverAnim = BatteryCover.GetComponent<Animator>();
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", false);
				}
			}
		}
		
		private void LockBase(Socketable s) {
			if(Socket) {
				Socket.Locked = true;
				BatteryCover = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
				if(BatteryCover != null) {
					Animator CoverAnim = BatteryCover.GetComponent<Animator>();
					if(CoverAnim != null)
					{
						CoverAnim.SetBool("Open", true);
					}
				}
			}
		}
	}
}