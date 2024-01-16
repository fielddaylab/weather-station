using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class RepairDeskControl : MonoBehaviour
	{
		[SerializeField] GameObject RepairDesk;
		
		[SerializeField] bool MoveUp = false;
		
		// Start is called before the first frame update
		void Awake() {
			
		}
		
		public void MoveDesk()
		{ 
			if(RepairDesk != null) {
				
				if(MoveUp) {
					RepairDesk.transform.Translate(Vector3.up * 0.1f, Space.World);
				}
				else {
					RepairDesk.transform.Translate(-Vector3.up * 0.1f, Space.World);
				}
			}
		}

		void Update() {

		}
	}
}