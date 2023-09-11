using System;
using BeauUtil;
using FieldDay;
using FieldDay.SharedState;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.DebugUpdate, -8000)]
    public class VRDebugInputSystem : SharedStateSystemBehaviour<VRInputState, VRDebugInputState> {
        public override void ProcessWork(float deltaTime) {
            if (!m_StateB.ResetPressed) {
                if (m_StateA.LeftHand.Holding(VRControllerButtons.Stick) && m_StateA.RightHand.Holding(VRControllerButtons.Stick)) {
                    m_StateB.ResetPressed = true;
                    SceneManager.LoadScene(SceneHelper.ActiveScene().BuildIndex, LoadSceneMode.Single);
                }
            } else {
                if (!m_StateA.LeftHand.Holding(VRControllerButtons.Stick) && !m_StateA.RightHand.Holding(VRControllerButtons.Stick)) {
                    m_StateB.ResetPressed = false;
                }
            }
        }
    }
}