using System;
using BeauUtil;
using FieldDay;
using FieldDay.Perf;
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

            if ((m_StateB.FpsToggleHold == 0 && m_StateA.LeftHand.Pressed(VRControllerButtons.Trigger))
                || (m_StateB.FpsToggleHold > 0 && m_StateA.LeftHand.Holding(VRControllerButtons.Trigger))) {
                m_StateB.FpsToggleHold += deltaTime;
                if (m_StateB.FpsToggleHold >= 1) {
                    if (FramerateDisplay.IsShowing()) {
                        FramerateDisplay.Hide();
                    } else {
                        FramerateDisplay.Show();
                    }
                    m_StateB.FpsToggleHold = 0;
                }
            } else if (!m_StateA.LeftHand.Holding(VRControllerButtons.Trigger)) {
                m_StateB.FpsToggleHold = 0;
            }
        }
    }
}