using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using ScriptableBake;
using UnityEngine;

public class ArgoAnimator : MonoBehaviour {
    public ArgoFace Face;
    public AudioSource Audio;
    public float FlapDelay = 0.1f;

    [NonSerialized] private long m_NextCheckTimestamp;
    [NonSerialized] private double m_LastVolume;
    [NonSerialized] private bool m_MouthFlapState;
    [NonSerialized] private bool m_MouthOpen;
    [NonSerialized] private float m_MouthFlapDelay;

    [NonSerialized] private float[] m_OutputData = new float[1024];

    private void Start() {
        Face.SetMouthState(ArgoFace.MouthState.Smirk);
    }

    public void HandlePose(StringHash32 poseId) {

    }

    private void LateUpdate() {
        bool flapping = UpdateMouthFlapState();
        if (!flapping) {
            if (m_MouthFlapState) {
                m_MouthFlapState = false;
                m_MouthOpen = false;
                m_MouthFlapDelay = 0;
                Face.SetMouthState(ArgoFace.MouthState.Smirk);
            }
        } else {
            m_MouthFlapState = true;
            m_MouthFlapDelay -= Frame.DeltaTime;
            if (m_MouthFlapDelay <= 0) {
                m_MouthFlapDelay += FlapDelay;

                m_MouthOpen = !m_MouthOpen;
                Face.SetMouthState(m_MouthOpen ? ArgoFace.MouthState.Open : ArgoFace.MouthState.Smirk);
            }
        }
    }

    private bool UpdateMouthFlapState() {
        if (!Audio.isPlaying) {
            return false;
        }

        long now = Frame.Timestamp();
        if (Frame.Timestamp() < m_NextCheckTimestamp) {
            return m_MouthFlapState;
        }

        m_NextCheckTimestamp = now + Stopwatch.Frequency / 16;

        float sum = 0;
        int channelCount = Audio.clip.channels;
        for (int c = 0; c < channelCount; c++) {
            Audio.GetOutputData(m_OutputData, c);
            for (int i = 0; i < m_OutputData.Length; i++) {
                sum += Math.Abs(m_OutputData[i]);
            }
        }
        sum /= m_OutputData.Length * channelCount;
        double volume = Math.Pow(sum, 0.2);

        m_LastVolume = volume;
        return m_LastVolume >= 0.2f;
    }
}