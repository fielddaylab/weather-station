using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.FixedUpdate, 500)]
    public class GrabSystem : ComponentSystemBehaviour<Grabber> {
        static private readonly Collider[] OverlapCache = new Collider[32];

        public override void ProcessWorkForComponent(Grabber component, float deltaTime) {
            switch (component.State) {
                case GrabberState.AttemptGrab: {
                    int overlapCount = Physics.OverlapSphereNonAlloc(component.GripCenter.position, component.GripRadius, OverlapCache, component.GripMask, QueryTriggerInteraction.Ignore);
                    Grabbable closest = null;
                    if (overlapCount > 0) {
                        closest = CollisionUtility.ClosestInParent<Grabbable>(component.GripCenter, (g) => g.GrabEnabled, OverlapCache, overlapCount);
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
                    }
                    break;
                }

                case GrabberState.AttemptRelease: {
                    GrabUtility.DropCurrent(component, true);
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
