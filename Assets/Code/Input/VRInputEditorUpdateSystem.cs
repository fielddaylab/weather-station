using System;
using BeauUtil;
using FieldDay;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate, -8000)]
    public class VRInputEditorUpdateSystem : SharedStateSystemBehaviour<VRInputState> {
        public override void ProcessWork(float deltaTime) {
            // TODO: Update
        }

        public override bool HasWork() {
            return base.HasWork() && !XRSettings.isDeviceActive;
        }
    }
}