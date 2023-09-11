using System;
using BeauUtil;
using FieldDay.Components;
using FieldDay.SharedState;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class VRTrackedTransform : BatchedComponent {
        public XRNode Node;
        public bool RenderOnly;
    }
}