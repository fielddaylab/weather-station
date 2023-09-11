using System;
using BeauUtil;
using FieldDay;
using FieldDay.SharedState;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace WeatherStation {
    public class VRDebugInputState : SharedStateComponent {
        [NonSerialized] public bool ResetPressed;
    }
}