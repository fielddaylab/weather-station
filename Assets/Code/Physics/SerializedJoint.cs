using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    [Serializable]
    public struct SerializedFixedJoint {
        public float BreakForce;
        public float BreakTorque;
        public float MassScale;
        public float ConnectedMassScale;

        public void Apply(FixedJoint joint) {
            joint.breakForce = BreakForce;
            joint.breakTorque = BreakTorque;
            joint.massScale = MassScale;
            joint.connectedMassScale = ConnectedMassScale;
        }

        static public readonly SerializedFixedJoint Default = new SerializedFixedJoint() {
            BreakForce = float.PositiveInfinity,
            BreakTorque = float.PositiveInfinity,
            MassScale = 1,
            ConnectedMassScale = 1
        };
    }
}
