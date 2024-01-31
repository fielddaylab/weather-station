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

		}
	

		public void OpenCover()	 {
			
			if(BatteryCover == null) {
				BatteryCover = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
			}
			
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
			
			if(BatteryCover == null) {
				BatteryCover = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
			}
			
			if(BatteryCover != null)
			{
				Animator CoverAnim = BatteryCover.GetComponent<Animator>();
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", false);
				}
			}
		}
		
		public void LockBase(bool lockBase) {
			if(Socket != null) {
				Socket.Locked = lockBase;
			}
		}
	}
}