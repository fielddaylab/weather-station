using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    [RequireComponent(typeof(Rigidbody))]
    public class Socketable : BatchedComponent {
        [NonSerialized] public Transform CachedTransform;
        [NonSerialized] public Rigidbody CachedRB;
        [NonSerialized] public Transform OriginalParent;
        [NonSerialized] public ItemSocket CurrentSocket;

        [NonSerialized] public HashSet<ItemSocket> PotentialSockets = new HashSet<ItemSocket>(4);
        [NonSerialized] public ItemSocket HighlightedSocket;

        public readonly CastableEvent<ItemSocket> OnAddedToSocket = new CastableEvent<ItemSocket>();
        public readonly CastableEvent<ItemSocket> OnRemovedFromSocket = new CastableEvent<ItemSocket>();

        // TODO: Events for highlighting/unhighlighting?

        private void Awake() {
            this.CacheComponent(ref CachedTransform);
            this.CacheComponent(ref CachedRB);
            OriginalParent = CachedTransform.parent;
        }
    }

    static public class SocketUtility {
        static public bool TryReleaseFromCurrentSocket(Socketable socketable, bool applyReleaseForce) {
            if (socketable.CurrentSocket) {
                if (socketable.CurrentSocket.Locked) {
                    return false;
                }

                ReleaseCurrent(socketable.CurrentSocket, applyReleaseForce);
            }

            return true;
        }

        static public bool TryReleaseFromCurrentSocket(Grabbable grabbable, bool applyReleaseForce) {
            if (!grabbable.TryGetComponent(out Socketable socketable)) {
                return true;
            }

            return TryReleaseFromCurrentSocket(socketable, applyReleaseForce);
        }

        static public bool TryAddToSocket(ItemSocket socket, Socketable socketable, bool force) {
            if (!socketable) {
                return false;
            }

            if (!force && (socket.Locked || socket.Current)) {
                return false;
            }

            ReleaseCurrent(socket, socket.Current != socketable);

            socket.Current = socketable;
            socketable.CurrentSocket = socket;

            if (socketable.TryGetComponent(out Grabbable grabbable)) {
                GrabUtility.DetachAll(grabbable);
            }

            SnapTransform(socketable.CachedTransform, socket.Location);

            switch (socket.Mode) {
                case ItemSocketMode.Reparent: {
                    socketable.CachedTransform.SetParent(socket.Location, true);
                    socketable.CachedRB.isKinematic = true;
                    break;
                }

                case ItemSocketMode.FixedJoint: {
                    if (!socket.CurrentJoint) {
                        socket.CurrentJoint = socket.gameObject.AddComponent<FixedJoint>();
                    }
                    socket.CurrentJoint.connectedBody = socketable.CachedRB;
                    socket.JointConfig.Apply(socket.CurrentJoint);
                    break;
                }
            }

            socket.Detector.enabled = false;

            grabbable.OriginalSocket = socket;
            
            socketable.OnAddedToSocket.Invoke(socket);
            socket.OnAdded.Invoke(socketable);

            return true;
        }

        static public void ReleaseCurrent(ItemSocket socket, bool applyReleaseForce) {
            if (!socket.Current) {
                return;
            }

            if (socket.CurrentJoint) {
                Rigidbody connected = socket.CurrentJoint.connectedBody;

                if (connected != null && applyReleaseForce) {
                    Vector3 force = socket.ReleaseForce;
                    force = socket.Location.TransformDirection(force);
                    connected.AddForce(force, ForceMode.Impulse);
                }

                socket.CurrentJoint.connectedBody = null;
                Joint.Destroy(socket.CurrentJoint);
                socket.CurrentJoint = null;
            }

            if (socket.Mode == ItemSocketMode.Reparent) {
                socket.Current.CachedTransform.SetParent(socket.Current.OriginalParent, true);
                socket.Current.CachedRB.isKinematic = false;
            }

            if (socket.ReleaseLocation) {
                SnapTransform(socket.Current.CachedTransform, socket.ReleaseLocation);
            }

            socket.Detector.enabled = true;
            socket.Current.CurrentSocket = null;
            socket.Current.OnRemovedFromSocket.Invoke(socket);
            socket.OnRemoved.Invoke(socket.Current);
            socket.Current = null;
        }

        static public void SnapTransform(Transform a, Transform target) {
            a.SetPositionAndRotation(target.position, target.rotation);
        }

        static public void RotateSocketed(ItemSocket socket, Socketable socketed, float angle) {
            socketed.gameObject.transform.RotateAround(socket.transform.position, socket.transform.right, angle);
        }
    }
}
