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

        [Header("Configuration")]
        public ItemSocketMode Mode = ItemSocketMode.Reparent;
        public SerializedFixedJoint JointConfig = SerializedFixedJoint.Default;
        public Transform Location;
        public TriggerListener Detector;
        [Space]
        public Transform ReleaseLocation;
        public Vector3 ReleaseForce;

        [NonSerialized] public FixedJoint CurrentJoint;

        public readonly CastableEvent<Socketable> OnAdded = new CastableEvent<Socketable>();
        public readonly CastableEvent<Socketable> OnRemoved = new CastableEvent<Socketable>();

        private void Awake() {
            if (!Location) {
                Location = transform;
            }

            if (Current) {
                SocketUtility.TryAddToSocket(this, Current, true);
            }
        }
    }

    public enum ItemSocketMode {
        FixedJoint,
        Reparent
    }
}
