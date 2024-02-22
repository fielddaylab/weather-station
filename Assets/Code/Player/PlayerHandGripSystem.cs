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
            VRInputState inputState = Lookup.State<VRInputState>();

            foreach(var component in m_Components) {
                VRHandState handInput = inputState.Hand(component.Hand);
                if (component.Physics.State == GrabberState.Empty && handInput.Pressed(VRControllerButtons.Grip)) {
                    component.Physics.State = GrabberState.AttemptGrab;
                } else if (component.Physics.State == GrabberState.Holding && !handInput.Holding(VRControllerButtons.Grip)) {
                    component.Physics.State = GrabberState.AttemptRelease;
                }
            }
        }
    }
}