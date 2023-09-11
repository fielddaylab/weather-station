using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.PreUpdate)]
    public class PlayerHandPositionUpdateSystem : SharedStateSystemBehaviour<PlayerHandRig> {
        public override void ProcessWork(float deltaTime) {
            
        }
    }
}