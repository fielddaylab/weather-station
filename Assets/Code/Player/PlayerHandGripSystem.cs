using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.FixedUpdate)]
    public class PlayerHandGripSystem : ComponentSystemBehaviour<PlayerHand> {
        public override void ProcessWork(float deltaTime) {
            VRInputState inputState = Find.State<VRInputState>();
            WSAnalytics w = Find.State<WSAnalytics>();

            foreach(var component in m_Components) {
                VRHandState handInput = inputState.Hand(component.Hand);
                if (component.Physics.State == GrabberState.Empty && handInput.Pressed(VRControllerButtons.Grip)) {
                    if(w != null) {
                        w.LogGrabGesture(component.Hand == VRHandIndex.Left, component.transform.position, component.transform.rotation);
                    }
                    component.Physics.State = GrabberState.AttemptGrab;
                } else if (component.Physics.State == GrabberState.Holding && !handInput.Holding(VRControllerButtons.Grip)) {
                    if(w != null) {
                        w.LogGrabRelease(component.Hand == VRHandIndex.Left, component.transform.position, component.transform.rotation);
                    }
                    component.Physics.State = GrabberState.AttemptRelease;
                }
            }
        }
    }
}