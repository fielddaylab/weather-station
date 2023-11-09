using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    [DefaultExecutionOrder(1)]
    public class ItemSocket : BatchedComponent {
        public bool Locked = false;
        public Socketable Current = null;
        public SocketFlags AllowedSockets = SocketFlags.Nothing;

        [Header("Configuration")]
        public ItemSocketMode Mode = ItemSocketMode.Reparent;
        public SerializedFixedJoint JointConfig = SerializedFixedJoint.Default;
        public Transform Location;
        public TriggerListener Detector;
        [Space]
        public Transform ReleaseLocation;
        public Vector3 ReleaseForce;
		
		public AudioSource SoundEffect;
		
        [NonSerialized] public FixedJoint CurrentJoint;

        public readonly CastableEvent<Socketable> OnAdded = new CastableEvent<Socketable>();
        public readonly CastableEvent<Socketable> OnRemoved = new CastableEvent<Socketable>();

        public bool IsSocketAllowed(SocketFlags Flags) {
            return ((AllowedSockets & Flags) != 0);
        }

        protected void Awake() {
            if (!Location) {
                Location = transform;
            }

            if (Current) {
                SocketUtility.TryAddToSocket(this, Current, true);
            }

            Detector.onTriggerEnter.AddListener(OnDetectorEntered);
            Detector.onTriggerExit.AddListener(OnDetectorExited);
			
			OnAdded.Register(OnSocketableAdded);
        }

        private void OnDetectorEntered(Collider collider) {
            Socketable socketable = collider.GetComponentInParent<Socketable>();
            if (socketable != null) {
                socketable.PotentialSockets.Add(this);
            }
        }

        private void OnDetectorExited(Collider collider) {
            if (!collider) {
                return;
            }

            Socketable socketable = collider.GetComponentInParent<Socketable>();
            if (socketable != null) {
                socketable.PotentialSockets.Remove(this);
            }
        }
		
		private void OnSocketableAdded(Socketable socketable) {
			//Debug.Log(socketable.gameObject.name + " added to " + gameObject.name);
			if(SoundEffect != null && SoundEffect.clip != null) {
				SoundEffect.Play();
			}
		}
    }

    public enum ItemSocketMode {
        FixedJoint,
        Reparent
    }
}
