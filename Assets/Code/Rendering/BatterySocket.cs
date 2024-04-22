using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FieldDay;

namespace WeatherStation {
	public class BatterySocket : MonoBehaviour
	{
		[SerializeField] ItemSocket Socket;
		
		private GameObject BatteryCover;
		
		// Start is called before the first frame update
		void Awake() {
			if(Socket != null) {
				Socket.OnAdded.Register(FindCover);
			}
		}
	

		public void OpenCover()	 {
			
			if(BatteryCover != null)
			{
				Animator CoverAnim = BatteryCover.GetComponent<Animator>();
				if(CoverAnim != null)
				{
					CoverAnim.SetBool("Open", true);
					WSAnalytics w = Find.State<WSAnalytics>();
					if(w != null) {
						w.LogBatteryBoxOpen();
					}
				}
			}
		}
		
		private void FindCover() {
			if(BatteryCover == null) {
				if( transform.childCount > 0) {
					Transform c1 = transform.GetChild(0);
					if(c1 != null) {
						if(c1.childCount > 0) {
							Transform c2 = c1.GetChild(0);
							if(c2.childCount > 1) {
								if(c2 != null) {
									Transform c3 = c2.GetChild(1);
									if(c3 != null) {
										BatteryCover = c3.gameObject;
									}
								}
							}
						}
					}
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
					WSAnalytics w = Find.State<WSAnalytics>();
					if(w != null) {
						w.LogBatteryBoxOpen();
					}
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