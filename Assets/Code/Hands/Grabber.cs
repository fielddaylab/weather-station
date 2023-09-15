using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    [RequireComponent(typeof(Rigidbody))]
    public class Grabber : BatchedComponent {
        #region Inspector

        public Transform GripCenter;
        public float GripRadius = 1;
        public LayerMask GripMask;

        [Header("Configuration")]
        public float MinGripForce = 20;
        public float MaxGripForce = 100;
        [Range(0, 1)] public float HeavyGripForceMultiplier = 0.7f;
        public float ReleaseThrowForce = 0.4f;
        public SerializedFixedJoint JointConfig = SerializedFixedJoint.Default;

        #endregion // Inspector

        [NonSerialized] public Transform CachedTransform;
        [NonSerialized] public Rigidbody CachedRB;

        [NonSerialized] public FixedJoint Joint;
        [NonSerialized] public GrabberState State = GrabberState.Empty;
        [NonSerialized] public Grabbable Holding;
        [NonSerialized] public long HoldStartTime;

        public CastableEvent<Grabbable> OnGrab = new CastableEvent<Grabbable>();
        public ActionEvent OnGrabFailed = new ActionEvent();
        public CastableEvent<Grabbable> OnRelease = new CastableEvent<Grabbable>();

        private void Awake() {
            this.CacheComponent(ref CachedTransform);
            this.CacheComponent(ref CachedRB);

            if (!GripCenter) {
                GripCenter = CachedTransform;
            }
			
			OnRelease.Register(OnReleaseGrab);
        }
		
		private void OnReleaseGrab(Grabbable priorHeld) {
			if(priorHeld.IsKinematic) {
				priorHeld.Rigidbody.isKinematic = true;
			}
		}
    }

    public enum GrabberState {
        Empty,
        AttemptGrab,
        Holding,
        AttemptRelease
    }
}
