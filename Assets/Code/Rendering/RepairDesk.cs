using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class RepairDesk : SharedStateComponent {
		#region Inspector
		
		public List<GameObject> TemperatureSensorButtons1 = new List<GameObject>(4);
		public List<GameObject> TemperatureSensorButtons2 = new List<GameObject>(4);
		public List<GameObject> TemperatureSensorButtons3 = new List<GameObject>(4);
		
		#endregion
		
		private void Awake() {

		}
		
	}
}