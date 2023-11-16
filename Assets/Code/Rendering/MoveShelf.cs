using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeatherStation {
	public class MoveShelf : MonoBehaviour
	{
		Animator ShelfAnimator = null;
		int CurrentShelf = 0;
		// Start is called before the first frame update
		void Awake()
		{
			ShelfAnimator = GetComponent<Animator>();
		}

		public void TurnShelf()
		{
			for(int i = 0; i < 3; ++i) {
				if(ShelfAnimator != null) {
					ShelfAnimator.SetBool("MoveShelf"+i.ToString(), false);
				}	
			}
			
			if(ShelfAnimator != null) {
				ShelfAnimator.SetBool("MoveShelf"+CurrentShelf.ToString(), true);
				CurrentShelf++;
			}
		}
	}
}