using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using UnityEngine;

namespace WeatherStation {
    public class RBInterpolator : BatchedComponent {
        public Rigidbody Target;

        public float VelocityMultiplier = 1;
        public float MaxVelocity = 60;
        public float AngularVelocityMultiplier = 1;
    }
}
