using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class RepairDeskControl : MonoBehaviour
	{
		[SerializeField] GameObject RepairDesk;
		
		[SerializeField] float MaxMove = 0.8975f;
		[SerializeField] float MinMove = 0.615f;
		
		[SerializeField] bool MoveUp = false;
		
		// Start is called before the first frame update
		void Awake() {
			
		}
		
		public void MoveDesk()
		{ 
			if(RepairDesk != null) {
				
				float currY = RepairDesk.transform.position.y;
				
				if(MoveUp) {
					if(currY + 0.02f <= MaxMove) {
						RepairDesk.transform.Translate(Vector3.up * 0.02f, Space.World);
					}
				}
				else {
					if(currY - 0.02f >= MinMove) {
						RepairDesk.transform.Translate(-Vector3.up * 0.02f, Space.World);
					}
				}
			}
		}

		void Update() {
			
		}
	}
}