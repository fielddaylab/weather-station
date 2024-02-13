using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using BeauUtil;
using BeauUtil.Debugger;
using BeauRoutine;
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
		public bool StayKinematic = false;
		public bool UseGrabPoses = false;
		public bool UsePerHandGrabPose = false;
        public bool ReturnOnGroundHit = true;
		
		public float GripAmount = 0.0f;
		public int GripPoseIndex = -1;
		public bool ConstrainGripPosition = false;
		
		public List<Transform> GrabSpots = new List<Transform>(8);
		public List<Transform> GrabSpotsLeft = new List<Transform>(8);
		
        [NonSerialized] public Rigidbody Rigidbody;
        [NonSerialized] public Grabber[] CurrentGrabbers;
        [NonSerialized] public int CurrentGrabberCount;

        

        [NonSerialized] public bool WasGrabbed = false;

        [NonSerialized] public bool WasKinematic = false;
        [NonSerialized] public Vector3 OriginalPosition;
        [NonSerialized] public Quaternion OriginalRotation;
        [NonSerialized] public Transform OriginalParent;
        [NonSerialized] public ItemSocket OriginalSocket;
		
		[NonSerialized] public Vector3 LastPosition = Vector3.zero;
		
        public readonly CastableEvent<Grabber> OnGrabbed = new CastableEvent<Grabber>();
        public readonly CastableEvent<Grabber> OnReleased = new CastableEvent<Grabber>();
		
		private Routine ReturnProcess;
		
        private void Awake() {
            this.CacheComponent(ref Rigidbody);
            
            WasKinematic = Rigidbody.isKinematic;
            OriginalPosition = transform.position;
            OriginalRotation = transform.rotation;
            OriginalParent = transform.parent;
			
            CurrentGrabbers = new Grabber[MaxGrabbers];	
        }
		
		public void OnCollisionEnter(Collision c) {
			if(c.gameObject.layer == 12 && !gameObject.GetComponent<Rigidbody>().isKinematic) {
				if(ReturnOnGroundHit && !ReturnProcess.Exists()) {
					ReturnProcess = Routine.Start(ReturnToStart());
				}
			}
		}
		
		private IEnumerator ReturnToStart() {
			yield return 1;
			if(OriginalSocket) {
				if(TryGetComponent(out Socketable s)) {
					if(OriginalSocket.Current == null) {
						SocketUtility.TryAddToSocket(OriginalSocket, s, true);
					} else {
						GrabUtility.ReturnToOriginalSpawnPoint(this);
					}
				}
			} else {
				GrabUtility.ReturnToOriginalSpawnPoint(this);
			}
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
            grabbable.WasGrabbed = true;
            
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
			
			if(!grabbable.StayKinematic) {
				grabbable.Rigidbody.isKinematic = false;
			}
			
			if(grabbable.UseGrabPoses) {
				PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();
				VRInputState data = Game.SharedState.Get<VRInputState>();
				if(handRig.LeftHandGrab.GrabbableBy == grabber) {
					data.LeftHand.HapticImpulse = 0.25f;
					
					GrabUtility.GrabPoseOn(handRig.LeftHandGrab, grabbable, true);
				}

				if(handRig.RightHandGrab.GrabbableBy == grabber) {
					data.RightHand.HapticImpulse = 0.25f;
					
					GrabUtility.GrabPoseOn(handRig.RightHandGrab, grabbable, false);
				}
			}
			
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
					
					if(grabber.Holding != null) {

						if(grabber.Holding.UseGrabPoses) {
							PlayerHandRig handRig = Game.SharedState.Get<PlayerHandRig>();

							if(handRig.LeftHandGrab.GrabbableBy == grabber && handRig.LeftHandGrab.IsGrabPosed) {
								GrabUtility.GrabPoseOff(handRig.LeftHandGrab, grabber.Holding, grabber, applyReleaseForce, handRig.RightHandGrab);
							}

							if(handRig.RightHandGrab.GrabbableBy == grabber && handRig.RightHandGrab.IsGrabPosed) {
								GrabUtility.GrabPoseOff(handRig.RightHandGrab, grabber.Holding, grabber, applyReleaseForce, handRig.LeftHandGrab);
							}
						}
					}
					
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
		
		static public void GrabPoseOn(GrabPose gp, Grabbable grabbable, bool isLeft=false) {
			gp.GrabberVisual.SetActive(false);
			gp.gameObject.SetActive(true);
			
			AudioSource aSource = gp.gameObject.GetComponent<AudioSource>();
			if(aSource)
			{
				aSource.Play();
			}
			
			gp.IsGrabPosed = true;
			
			grabbable.Rigidbody.useGravity = false;
			grabbable.Rigidbody.isKinematic = true;
			
			Animator a = gp.gameObject.GetComponent<Animator>();
			if(a != null) {
				if(grabbable.GripPoseIndex == -1) {
					a.SetInteger(Animator.StringToHash("Pose"), -1);
					a.SetFloat(Animator.StringToHash("Flex"), grabbable.GripAmount);	
				}
			}
			
			int closestSpot = -1;
			if(isLeft && grabbable.UsePerHandGrabPose) {
				
				int specificGripPose = -1;
				float dist = 9999f;
				for(int i = 0; i < grabbable.GrabSpotsLeft.Count; ++i) {
					float currDist = Vector3.Distance(gp.GrabberVisual.transform.position, grabbable.GrabSpotsLeft[i].position);
					if(currDist < dist) {
						dist = currDist;
						closestSpot = i;
					}
				}	
				
				if(closestSpot != -1) {
					//for position, instead walk through list of possible grab points... attach to closest...
					gp.gameObject.transform.position = grabbable.GrabSpotsLeft[closestSpot].transform.position;
					gp.gameObject.transform.rotation = grabbable.GrabSpotsLeft[closestSpot].transform.rotation;
					if(grabbable.ConstrainGripPosition) {
						gp.ConstrainGripPosition = true;
						gp.ConstrainedGripTransform = grabbable.GrabSpotsLeft[closestSpot].transform;
						//gp.ConstrainedGripPosition = gp.gameObject.transform.position;
					} else {
						gp.ConstrainGripPosition = false;
						gp.ConstrainedGripPosition = Vector3.zero;
					}
				}
				
				/*if(a != null) {
					GrabSpot gs = grabbable.GrabSpotsLeft[closestSpot].gameObject.GetComponent<GrabSpot>();
					if(gs != null) {
						specificGripPose = gs.GrabPoseIndex;
					}
					
					if(specificGripPose != -1) {
						a.SetInteger(Animator.StringToHash("Pose"), specificGripPose);
					}
				}*/
			}
			else {
				
				int specificGripPose = -1;
				float dist = 9999f;
				for(int i = 0; i < grabbable.GrabSpots.Count; ++i) {
					float currDist = Vector3.Distance(gp.GrabberVisual.transform.position, grabbable.GrabSpots[i].position);
					if(currDist < dist) {
						dist = currDist;
						closestSpot = i;
					}
				}
				
				if(closestSpot != -1) {
					//for position, instead walk through list of possible grab points... attach to closest...
					gp.gameObject.transform.position = grabbable.GrabSpots[closestSpot].transform.position;
					gp.gameObject.transform.rotation = grabbable.GrabSpots[closestSpot].transform.rotation;
					if(grabbable.ConstrainGripPosition) {
						gp.ConstrainGripPosition = true;
						gp.ConstrainedGripTransform = grabbable.GrabSpots[closestSpot].transform;
						//gp.ConstrainedGripPosition = gp.gameObject.transform.position;
					} else {
						gp.ConstrainGripPosition = false;
						gp.ConstrainedGripPosition = Vector3.zero;
					}
				}

				/*if(a != null) {
					GrabSpot gs = grabbable.GrabSpots[closestSpot].gameObject.GetComponent<GrabSpot>();
					if(gs != null) {
						specificGripPose = gs.GrabPoseIndex;
					}
					
					if(specificGripPose != -1) {
						a.SetInteger(Animator.StringToHash("Pose"), specificGripPose);
					}
				}*/
			}
			
			gp.gameObject.transform.SetParent(grabbable.transform);
			
			//we want to temporarily set the parent of the grab pose component to the thing we grabbed, but also set the thing we grabbed'd parent to the grabber visual
			/*if(isLeft && grabbable.UsePerHandGrabPose) 
			{
				gp.gameObject.transform.SetParent(grabbable.GrabSpotsLeft[closestSpot].transform);
			}
			else
			{
				gp.gameObject.transform.SetParent(grabbable.GrabSpots[closestSpot].transform);
			}*/

			if(!grabbable.ConstrainGripPosition) {
				grabbable.transform.SetParent(gp.GrabberVisual.transform.parent);
			}
			
		}
		
		static public void ForceGrabPoseOff(GrabPose gp) {
			gp.GrabberVisual.SetActive(true);
			gp.SetToOriginalParent();
			gp.gameObject.SetActive(false);
			gp.IsGrabPosed = false;
		}
		
		static public void GrabPoseOff(GrabPose gp, Grabbable grabbable, Grabber grabber, bool applyReleaseForce, GrabPose otherGrabPose) 
		{
			gp.GrabberVisual.SetActive(true);
			gp.SetToOriginalParent();
			gp.gameObject.SetActive(false);
			gp.IsGrabPosed = false;
			
			//Debug.Log(grabbable.CurrentGrabberCount);
			if(grabbable.CurrentGrabberCount == 0) {

				if(grabbable.GripPoseIndex != -1) {
					Animator a = gp.gameObject.GetComponent<Animator>();
					if(a)
					{
						a.SetInteger(Animator.StringToHash("Pose"), -1);
					}
				} 

				if(!grabbable.ConstrainGripPosition) {
					grabbable.gameObject.transform.SetParent(grabbable.OriginalParent);
				}
				
				if(!grabbable.StayKinematic) {
					grabbable.Rigidbody.useGravity = true;	
					grabbable.Rigidbody.isKinematic = false;
				}
				
                if (applyReleaseForce && grabber.ReleaseThrowForce > 0) {
                    Rigidbody connected = grabbable.Rigidbody;
                    if (connected) {
                        Vector3 anchor = grabber.Joint.connectedAnchor;
                        anchor = connected.transform.TransformPoint(anchor);
                        Vector3 velocity = grabber.CachedRB.velocity;
						Vector3 forceVec = (grabbable.gameObject.transform.position - grabbable.LastPosition);
						forceVec.x *= velocity.x;
						forceVec.y *= velocity.y;
						forceVec.z *= velocity.z;
                        connected.AddForceAtPosition(forceVec * grabber.ReleaseThrowForce, anchor, ForceMode.Impulse);
                    } 
                }
			} else {
				GrabPoseOn(otherGrabPose, grabbable);
			}
		}
		
		static public void ReturnToOriginalSpawnPoint(Grabbable component) {
			if(component != null && component.Rigidbody != null) {
				component.Rigidbody.isKinematic = component.WasKinematic;
				component.transform.position = component.OriginalPosition;
				component.transform.rotation = component.OriginalRotation;
				component.transform.SetParent(component.OriginalParent, true);
			}
		}
    }
}
