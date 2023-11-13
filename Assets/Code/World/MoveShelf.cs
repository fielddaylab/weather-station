using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class MoveShelf : MonoBehaviour
	{
		Animator ShelfAnimator = null;
		int CurrentShelf = 0;
		
		public CabinetShelf Shelf1;
		public CabinetShelf Shelf2;
		public CabinetShelf Shelf3;
		
		bool Turning = false;
		// Start is called before the first frame update
		void Awake() {
			ShelfAnimator = GetComponent<Animator>();
		}

		public void TurnShelf() {
			if(!Turning) {
				if(Shelf1.IsReady && Shelf2.IsReady && Shelf3.IsReady) {
					
					Turning = true;
					
					Shelf1.QueryColliders();
					Shelf2.QueryColliders();
					Shelf3.QueryColliders();
					
					StartCoroutine("DoTurn");
				}
			}
		}
		
		IEnumerator DoTurn() {
			yield return new WaitForSeconds(0.25f);
			
			for(int i = 0; i < 3; ++i) {
				if(ShelfAnimator != null) {
					//Debug.Log("MoveShelf"+i.ToString());
					ShelfAnimator.SetBool("MoveShelf"+i.ToString(), false);
				}	
			}
			
			if(ShelfAnimator != null) {
				ShelfAnimator.SetBool("MoveShelf"+CurrentShelf.ToString(), true);
				CurrentShelf++;
				if(CurrentShelf == 3) {
					CurrentShelf = 0;
				}
			}
			
			Turning = false;
		}
	}
}