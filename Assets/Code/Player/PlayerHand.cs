using FieldDay.Components;
using FieldDay.SharedState;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class PlayerHand : BatchedComponent {
        public VRHandIndex Hand;
        public Grabber Physics;
        public Transform Visual;

        private void Awake() {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}