using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.LateFixedUpdate, 500)]
    public class GrabSystem : ComponentSystemBehaviour<Grabber> {
        static private readonly Collider[] OverlapCache = new Collider[32];

        public override void ProcessWorkForComponent(Grabber component, float deltaTime) {
            switch (component.State) {
                case GrabberState.AttemptGrab: {
                    int overlapCount = Physics.OverlapSphereNonAlloc(component.GripCenter.position, component.GripRadius, OverlapCache, component.GripMask, QueryTriggerInteraction.Ignore);
                    Grabbable closest = null;
                    Collider closestCollider = null;
                    if (overlapCount > 0) {
                        closest = CollisionUtility.ClosestInParent<Grabbable>(component.GripCenter, (g) => g.GrabEnabled, OverlapCache, overlapCount, out closestCollider);
                    }
                    if (closest != null) {
                        // TODO: Find closest grab hint
                        GrabUtility.Grab(component, closest);
                    } else {
                        component.OnGrabFailed.Invoke();
                        component.State = GrabberState.Empty;
                    }
                    break;
                }

                case GrabberState.Holding: {
                    if (!component.Holding || !component.Holding.isActiveAndEnabled || !component.Holding.GrabEnabled) {
                        GrabUtility.DropCurrent(component, false);
                    } else {
						if(component.Holding.UseGrabPoses && component.Holding.ConstrainGripPosition) {
							
							PlayerHandRig handRig = Find.State<PlayerHandRig>();
							
							if(handRig.LeftHandGrab.GrabbableBy == component && handRig.LeftHandGrab.IsGrabPosed) {
								
								if(component.Holding.UsePerHandGrabPose) {
									int closestSpot = -1;
									float dist = 9999f;
									for(int i = 0; i < component.Holding.GrabSpotsLeft.Count; ++i) {
										float currDist = Vector3.Distance(handRig.LeftHandGrab.GrabberVisual.transform.position, component.Holding.GrabSpotsLeft[i].position);
										if(currDist < dist) {
											dist = currDist;
											closestSpot = i;
										}
									}
									
									handRig.LeftHandGrab.ConstrainedGripPosition = component.Holding.GrabSpotsLeft[closestSpot].position;
									
								}
								else {
									int closestSpot = -1;
									float dist = 9999f;
									for(int i = 0; i < component.Holding.GrabSpots.Count; ++i) {
										float currDist = Vector3.Distance(handRig.LeftHandGrab.GrabberVisual.transform.position, component.Holding.GrabSpots[i].position);
										if(currDist < dist) {
											dist = currDist;
											closestSpot = i;
										}
									}
									
									handRig.LeftHandGrab.ConstrainedGripPosition = component.Holding.GrabSpots[closestSpot].position;
								}
								
							} else if(handRig.RightHandGrab.GrabbableBy == component && handRig.RightHandGrab.IsGrabPosed) {
								
								int closestSpot = -1;
								float dist = 9999f;
								for(int i = 0; i < component.Holding.GrabSpots.Count; ++i) {
									float currDist = Vector3.Distance(handRig.RightHandGrab.GrabberVisual.transform.position, component.Holding.GrabSpots[i].position);
									if(currDist < dist) {
										dist = currDist;
										closestSpot = i;
									}
								}
								
								
								handRig.RightHandGrab.ConstrainedGripPosition = component.Holding.GrabSpots[closestSpot].position;
							}
						}
					}
                    break;
                }

                case GrabberState.AttemptRelease: {
                    if (component.Holding && component.Holding.TryGetComponent(out Socketable socketable) && socketable.HighlightedSocket) {
                        SocketUtility.TryAddToSocket(socketable.HighlightedSocket, socketable, false);
                    } else {
                        GrabUtility.DropCurrent(component, true);
                    }
                    component.State = GrabberState.Empty;
                    break;
                }
            }
        }

        protected override void OnComponentRemoved(Grabber component) {
            GrabUtility.DropCurrent(component, false);
        }
    }
}
