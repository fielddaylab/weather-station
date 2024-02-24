using System;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhaseMask.FixedUpdate | GameLoopPhaseMask.Update | GameLoopPhaseMask.ApplicationPreRender, -7000)]
    public class VRTrackedTransformSystem : ComponentSystemBehaviour<VRTrackedTransform> {
        [SerializeField] private float m_PositionScale = 1;

        public override void ProcessWork(float deltaTime) {
            VRInputState data = Find.State<VRInputState>();

            foreach(var obj in m_Components) {
                if (obj.RenderOnly && !GameLoop.IsPhase(GameLoopPhase.ApplicationPreRender)) {
                    continue;
                }

                Transform t = obj.transform;
                Pose p;
                t.GetLocalPositionAndRotation(out p.position, out p.rotation);

                switch (obj.Node) {
                    case XRNode.CenterEye: {
                        p = data.CenterEye;
                        break;
                    }
                    case XRNode.LeftEye: {
                        p = data.LeftEye;
                        break;
                    }
                    case XRNode.RightEye: {
                        p = data.RightEye;
                        break;
                    }

                    case XRNode.Head: {
                        p = data.Head;
                        break;
                    }

                    case XRNode.LeftHand: {
                        p = data.LeftHand.Pose;
                        break;
                    }

                    case XRNode.RightHand: {
                        p = data.RightHand.Pose;
                        break;
                    }

                    default: {
                        Log.Error("[VRTrackedTransformSystem] Cannot track node '{0}'", obj.Node);
                        obj.enabled = false;
                        break;
                    }
                }

				if(obj.Node == XRNode.LeftHand || obj.Node == XRNode.RightHand) {
					t.SetLocalPositionAndRotation(p.position * m_PositionScale, p.rotation * obj.RotationOffset);
				} else {
					t.SetLocalPositionAndRotation(p.position * m_PositionScale, p.rotation);
				}
            }
        }
    }
}