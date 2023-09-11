using System;
using BeauUtil;
using FieldDay;
using FieldDay.SharedState;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace WeatherStation {
    public class VRInputState : SharedStateComponent {
        [NonSerialized] public VRDataSources Available;
        [NonSerialized] public ushort InputFrame = Frame.InvalidIndex;

        [NonSerialized] public Pose LeftEye;
        [NonSerialized] public Pose RightEye;
        [NonSerialized] public Pose CenterEye;
        [NonSerialized] public Pose Head;
        [NonSerialized] public VRHandState LeftHand;
        [NonSerialized] public VRHandState RightHand;

        public VRHandState Hand(VRHandIndex index) {
            switch (index) {
                case VRHandIndex.Left: {
                    return LeftHand;
                }
                case VRHandIndex.Right: {
                    return RightHand;
                }
                default: {
                    return default;
                }
            }
        }
    }

    [Serializable]
    public struct VRHandState {
        public Pose Pose;
        public VRControllerButtons Buttons;
        public VRControllerButtons PrevButtons;
        public VRControllerAxis Axis;

        public bool Pressed(VRControllerButtons buttons) {
            return ((Buttons & ~PrevButtons) & buttons) != 0;
        }

        public bool Released(VRControllerButtons buttons) {
            return ((PrevButtons & ~Buttons) & buttons) != 0;
        }

        public bool Holding(VRControllerButtons buttons) {
            return (Buttons & buttons) != 0;
        }
    }

    [Flags]
    public enum VRDataSources : uint {
        LeftEye = 1 << XRNode.LeftEye,
        RightEye = 1 << XRNode.RightEye,
        CenterEye = 1 << XRNode.CenterEye,
        Head = 1 << XRNode.Head,
        LeftHand = 1 << XRNode.LeftHand,
        RightHand = 1 << XRNode.RightHand
    }

    [Flags]
    public enum VRControllerButtons {
        Stick = 0x01,
        Primary = 0x02,
        Secondary = 0x04,
        Menu = 0x08,
        Grip = 0x10,
        Trigger = 0x20,
    }

    public struct VRControllerAxis {
        public Vector2 Stick;
        public float Trigger;
        public float Grip;
    }

    public enum VRHandIndex : ushort {
        Left,
        Right,
        Any
    }
}