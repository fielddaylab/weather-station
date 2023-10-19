using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class PlayerLocator : SharedStateComponent {
		#region Inspector
		public Transform InsideLocation;
		public Transform OutsideLocation;
		#endregion // Inspector


		[NonSerialized] bool IsInside = false;
		
		private void Awake() {
	
		}
		
		public void StartTeleportCountdown() {
			
		}
    }
}