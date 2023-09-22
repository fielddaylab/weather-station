using FieldDay;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using BeauUtil.Debugger;
using FieldDay.Debugging;

namespace WeatherStation {
    [SysUpdate(GameLoopPhaseMask.FixedUpdate | GameLoopPhaseMask.Update | GameLoopPhaseMask.ApplicationPreRender, -8000)]
    public class VRInputUpdateSystem : SharedStateSystemBehaviour<VRInputState> {
        private readonly List<XRNodeState> m_NodeStateWorkList = new List<XRNodeState>(16);
        private readonly List<InputDevice> m_InputDeviceWorkList = new List<InputDevice>(16);

        public override bool HasWork() {
            return base.HasWork() && XRSettings.isDeviceActive;
        }

        public override void ProcessWork(float deltaTime) {
            VRDataSources found = 0;

            ref VRHandState leftHand = ref m_State.LeftHand;
            ref VRHandState rightHand = ref m_State.RightHand;

            // always poll node positions

            InputDevices.GetDevices(m_InputDeviceWorkList);
            //Log.Msg("[VRInputUpdateSystem] {0} devices available", m_InputDeviceWorkList.Count);

            InputTracking.GetNodeStates(m_NodeStateWorkList);
            //Log.Msg("[VRInputUpdateSystem] {0} nodes available", m_NodeStateWorkList.Count);
            foreach (var node in m_NodeStateWorkList) {
                switch (node.nodeType) {
                    case XRNode.LeftEye: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.LeftEye;
                            m_State.LeftEye = pose;
                        }
                        break;
                    }
                    case XRNode.RightEye: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.RightEye;
                            m_State.RightEye = pose;
                        }
                        break;
                    }
                    case XRNode.CenterEye: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.CenterEye;
                            m_State.CenterEye = pose;
                        }
                        break;
                    }
                    case XRNode.Head: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.Head;
                            m_State.Head = pose;
                        }
                        break;
                    }
                    case XRNode.LeftHand: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.LeftHand;
                            leftHand.Pose = pose;
                        }
                        break;
                    }
                    case XRNode.RightHand: {
                        if (TryGetPose(node, out Pose pose)) {
                            found |= VRDataSources.RightHand;
                            rightHand.Pose = pose;
                        }
                        break;
                    }
                }
            }

            // only poll controller state at beginning of frame

            if (Frame.Index != m_State.InputFrame) {
                m_State.InputFrame = Frame.Index;

                UpdateHandController(ref leftHand, XRNode.LeftHand);
                UpdateHandController(ref rightHand, XRNode.RightHand);

                DebugDraw.AddViewportText(new Vector2(0.5f, 1), new Vector2(0, -8), string.Format("{0} Input Devices\n{1} XR Nodes\nHead at {2}", m_InputDeviceWorkList.Count, m_NodeStateWorkList.Count, m_State.Head.position), Color.white, 0, TextAnchor.UpperCenter, DebugTextStyle.BackgroundDark);
                DebugDraw.AddViewportText(new Vector2(0, 0), new Vector2(8, 8), string.Format("Left Hand at {0}\nButtons {1}\nStick {2} | Trigger {3} | Grip {4}", leftHand.Pose, leftHand.Buttons, leftHand.Axis.Stick, leftHand.Axis.Trigger, leftHand.Axis.Grip), Color.white, 0, TextAnchor.LowerLeft, DebugTextStyle.BackgroundDark);
                DebugDraw.AddViewportText(new Vector2(1, 0), new Vector2(-8, 8), string.Format("Right Hand at {0}\nButtons {1}\nStick {2} | Trigger {3} | Grip {4}", rightHand.Pose, rightHand.Buttons, rightHand.Axis.Stick, rightHand.Axis.Trigger, rightHand.Axis.Grip), Color.white, 0, TextAnchor.LowerRight, DebugTextStyle.BackgroundDark);
            }

            m_State.Available = found;
        }

        #region OpenXR
		
        static private bool UpdateHandController(ref VRHandState hand, XRNode node) {
            hand.PrevButtons = hand.Buttons;
            hand.Buttons = 0;
            hand.Axis = default;

            InputDevice input = InputDevices.GetDeviceAtXRNode(node);

            if (input.isValid) {
                if (GetFeature(input, CommonUsages.primaryButton)) {
                    hand.Buttons |= VRControllerButtons.Primary;
                }
                if (GetFeature(input, CommonUsages.secondaryButton)) {
                    hand.Buttons |= VRControllerButtons.Secondary;
                }
                if (GetFeature(input, CommonUsages.menuButton)) {
                    hand.Buttons |= VRControllerButtons.Menu;
                }
                if (GetFeature(input, CommonUsages.primary2DAxisClick)) {
                    hand.Buttons |= VRControllerButtons.Stick;
                }
                if (GetFeature(input, CommonUsages.triggerButton)) {
                    hand.Buttons |= VRControllerButtons.Trigger;
                }
                if (GetFeature(input, CommonUsages.gripButton)) {
                    hand.Buttons |= VRControllerButtons.Grip;
                }

                hand.Axis.Stick = GetFeature(input, CommonUsages.primary2DAxis);
                hand.Axis.Trigger = GetFeature(input, CommonUsages.trigger);
                hand.Axis.Grip = GetFeature(input, CommonUsages.grip);
				
				HapticCapabilities caps;
				if(input.TryGetHapticCapabilities(out caps)) {
					if  (caps.supportsImpulse && hand.HapticImpulse != 0f) {
						//Debug.Log("Requesting haptics");
						input.SendHapticImpulse((uint)0, hand.HapticImpulse, 0.5f);
						//Ross 9/21/2023: assume we will constantly set it elsewhere for now if we want to continue pulsing
						hand.HapticImpulse = 0f;
					}
				} 

                return true;
            }

            return false;
        }

        #endregion // OpenXR

        #region Input Accessors

        static private bool TryGetPose(XRNodeState state, out Pose pose) {
            pose = default(Pose);
            return state.tracked && state.TryGetPosition(out pose.position) && state.TryGetRotation(out pose.rotation);
        }

        static private bool GetFeature(InputDevice device, InputFeatureUsage<bool> usage) {
            bool val = false;
            return device.TryGetFeatureValue(usage, out val) && val;
        }

        static private Vector2 GetFeature(InputDevice device, InputFeatureUsage<Vector2> usage) {
            Vector2 val = default;
            device.TryGetFeatureValue(usage, out val);
            return val;
        }

        static private float GetFeature(InputDevice device, InputFeatureUsage<float> usage) {
            float val = default;
            device.TryGetFeatureValue(usage, out val);
            return val;
        }

        static private Vector3 GetFeature(InputDevice device, InputFeatureUsage<Vector3> usage) {
            Vector3 val = default;
            device.TryGetFeatureValue(usage, out val);
            return val;
        }

        static private bool TryGetFeature(InputDevice device, InputFeatureUsage<Hand> usage, out Hand val) {
            return device.TryGetFeatureValue(usage, out val);
        }

        static private Quaternion GetFeature(InputDevice device, InputFeatureUsage<Quaternion> usage) {
            Quaternion val = default;
            if (!device.TryGetFeatureValue(usage, out val)) {
                val = Quaternion.identity;
            }
            return val;
        }

        #endregion // Input Accessors
    }
}