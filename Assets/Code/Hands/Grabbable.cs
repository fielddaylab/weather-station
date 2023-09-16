using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using TMPro;
using UnityEngine;

namespace WeatherStation {
    [RequireComponent(typeof(Rigidbody))]
    public class Grabbable : BatchedComponent {
        public bool GrabEnabled = true;

        [Header("Configuration")]
        [Range(1, 4)] public int MaxGrabbers = 2;
        public bool IsHeavy = false;
		
        [NonSerialized] public Rigidbody Rigidbody;
        [NonSerialized] public Grabber[] CurrentGrabbers;
        [NonSerialized] public int CurrentGrabberCount;

        public readonly CastableEvent<Grabber> OnGrabbed = new CastableEvent<Grabber>();
        public readonly CastableEvent<Grabber> OnReleased = new CastableEvent<Grabber>();

        private void Awake() {
            this.CacheComponent(ref Rigidbody);
            CurrentGrabbers = new Grabber[MaxGrabbers];
        }
    }

    static public class GrabUtility {
        static public bool Grab(Grabber grabber, Grabbable grabbable) {
            if (!grabbable || grabber.Holding == grabbable || !SocketUtility.TryReleaseFromCurrentSocket(grabbable, false)) {
                return false;
            }

            if (grabbable.CurrentGrabberCount >= grabbable.MaxGrabbers) {
                DetachOldest(grabbable);
            }

            if (!ReferenceEquals(grabber.Holding, null)) {
                grabber.Holding.OnReleased.Invoke(grabber);
                grabber.OnRelease.Invoke(grabber.Holding);
            }

            grabber.Holding = grabbable;
            grabbable.CurrentGrabbers[grabbable.CurrentGrabberCount++] = grabber;

            if (!grabber.Joint) {
                grabber.Joint = grabber.gameObject.AddComponent<FixedJoint>();
            }

            grabber.Joint.connectedBody = grabbable.Rigidbody;

            SerializedFixedJoint jointConfig = grabber.JointConfig;
            jointConfig.ConnectedMassScale *= Mathf.Clamp(grabbable.Rigidbody.mass, grabber.MinGripForce, grabber.MaxGripForce);
            if (grabbable.IsHeavy) {
                jointConfig.ConnectedMassScale *= grabber.HeavyGripForceMultiplier;
            }
            jointConfig.Apply(grabber.Joint);

            grabber.State = GrabberState.Holding;
            grabber.HoldStartTime = Frame.Timestamp();

            grabbable.OnGrabbed.Invoke(grabber);
            grabber.OnGrab.Invoke(grabbable);
			
            return true;
        }

        static private bool DetachOldest(Grabbable grabbable) {
            if (grabbable.CurrentGrabberCount <= 0) {
                return false;
            }

            Grabber least = grabbable.CurrentGrabbers[0];
            long leastTS = least.HoldStartTime;

            for(int i = 1; i < grabbable.CurrentGrabberCount; i++) {
                Grabber check = grabbable.CurrentGrabbers[i];
                if (check.HoldStartTime < leastTS) {
                    least = check;
                    leastTS = check.HoldStartTime;
                }
            }

            DropCurrent(least, false);
			
            return true;
        }

        static public void DetachAll(Grabbable grabbable) {
            while(grabbable.CurrentGrabberCount > 0) {
                DropCurrent(grabbable.CurrentGrabbers[grabbable.CurrentGrabberCount - 1], false);
            }
        }

        static public bool DropCurrent(Grabber grabber, bool applyReleaseForce) {
            if (!ReferenceEquals(grabber.Joint, null)) {
                if (!ReferenceEquals(grabber.Holding, null)) {

                    int idx = Array.IndexOf(grabber.Holding.CurrentGrabbers, grabber);
                    Assert.True(idx >= 0);
                    ArrayUtils.FastRemoveAt(grabber.Holding.CurrentGrabbers, ref grabber.Holding.CurrentGrabberCount, idx);

                    grabber.Holding.OnReleased.Invoke(grabber);
                    grabber.OnRelease.Invoke(grabber.Holding);
                    grabber.Holding = null;
                }

                if (applyReleaseForce && grabber.ReleaseThrowForce > 0) {
                    Rigidbody connected = grabber.Joint.connectedBody;
                    if (connected) {
                        Vector3 anchor = grabber.Joint.connectedAnchor;
                        anchor = connected.transform.TransformPoint(anchor);
                        Vector3 velocity = grabber.CachedRB.velocity;

                        connected.AddForceAtPosition(velocity * grabber.ReleaseThrowForce, anchor, ForceMode.Impulse);
                    }
                }
                
                grabber.Joint.connectedBody = null;
                Joint.Destroy(grabber.Joint);
                grabber.Joint = null;

                grabber.HoldStartTime = -1;
                grabber.State = GrabberState.Empty;
				
                return true;
            }

            return false;
        }
    }
}
