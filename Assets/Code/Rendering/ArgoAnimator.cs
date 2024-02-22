using System;
using System.Diagnostics;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using Leaf.Runtime;
using UnityEngine;
using WeatherStation.Scripting;

public class ArgoAnimator : ScriptComponent {
    public ArgoFace Face;

    [Header("Mouth")]
    public AudioSource Audio;
    public float FlapDelay = 0.1f;

    [Header("Blinking")]
    public float BlinkDelay = 0.5f;
    public float BlinkRandom = 0;
    public float BlinkClosedDuration = 0.1f;

    [Header("Eye Tracking")]
    public Transform LookRoot;
    public Transform LookTarget;
    public float LookScale = 2;

    [Header("Poses")]
    public ArgoFacePose Resting;
    public ArgoFacePose ErrorPose;
    public NamedArgoFacePose[] NamedPoses;

    [Header("Celebration")]
    public AudioSource CelebrationPlayer;
    public AudioClip CelebrationSfx;
    public AudioClip BigCelebrationSfx;

    [NonSerialized] private long m_NextTalkCheckTimestamp;
    [NonSerialized] private double m_LastVolume;
    [NonSerialized] private bool m_IsVoicing;
    
    [NonSerialized] private bool m_CurrentlyTalking;
    [NonSerialized] private bool m_MouthOpen;
    [NonSerialized] private float m_MouthFlapDelay;

    [NonSerialized] private bool m_CurrentlyBlinking;
    [NonSerialized] private float m_BlinkDelay;
    [NonSerialized] private bool m_EyeOpen;
    [NonSerialized] private bool m_EyeTracking;

    [NonSerialized] private StringHash32 m_CurrentPoseId;
    [NonSerialized] private ArgoFacePose m_CurrentPose;
    [NonSerialized] private float[] m_OutputData = new float[1024];

    [NonSerialized] private RingBuffer<ArgoFacePoseKeyFrame> m_QueuedPoses = new RingBuffer<ArgoFacePoseKeyFrame>(16, RingBufferMode.Expand);

    private void Start() {
        ApplyPose(Resting, default);
    }

    [LeafMember("SetFacePose")]
    public void SetPoseById(StringHash32 poseId) {
        if (m_CurrentPoseId == poseId) {
            return;
        }

        if (poseId.IsEmpty) {
            ApplyPose(Resting, default);
            return;
        }
        
        for(int i = 0; i < NamedPoses.Length; i++) {
            if (NamedPoses[i].Id == poseId) {
                ApplyPose(NamedPoses[i]);
                return;
            }
        }

        Log.Error("[ArgoAnimator] No named face pose '{0}'", poseId);
        ApplyPose(ErrorPose, "ERROR");
    }

    
    public void QueuePoseChange(float time, StringHash32 id) {
        m_QueuedPoses.PushBack(new ArgoFacePoseKeyFrame() {
            Id = id,
            Time = time
        });
    }

    private void LateUpdate() {
        bool audioIsPlaying = Audio.isPlaying;
        if (m_IsVoicing != audioIsPlaying) {
            m_IsVoicing = audioIsPlaying;
            if (!m_IsVoicing) {
                ApplyPose(Resting, default);
                m_QueuedPoses.Clear();
            }
        }

        if (m_IsVoicing) {
            if (m_QueuedPoses.TryPeekFront(out var keyframe)) {
                if (Audio.time >= keyframe.Time) {
                    SetPoseById(keyframe.Id);
                    m_QueuedPoses.PopFront();
                }
            }
        }

        UpdateMouthFlap();
        UpdateBlinking();

        if (Frame.DeltaTime > 0) {
            UpdateLooking();
        }
    }

    private void ApplyPose(in ArgoFacePose pose, StringHash32 id) {
        m_CurrentPoseId = id;
        m_CurrentPose = pose;
        m_EyeTracking = (pose.Flags & ArgoFacePoseFlags.DisableEyeTracking) == 0;

        bool shouldBlink = IsBlinkingEnabled(pose.LeftEye) || IsBlinkingEnabled(pose.RightEye);
        if (m_CurrentlyBlinking != shouldBlink) {
            m_CurrentlyBlinking = shouldBlink;
            m_BlinkDelay = BlinkDelay + RNG.Instance.NextFloat(BlinkRandom);

            m_EyeOpen = IsEyeOpen(pose.LeftEye) || IsEyeOpen(pose.RightEye);
        }

        if (m_EyeOpen || !IsBlinkingEnabled(pose.LeftEye)) {
            Face.SetLeftEyeState(pose.LeftEye);
        }

        if (m_EyeOpen || !IsBlinkingEnabled(pose.RightEye)) {
            Face.SetRightEyeState(pose.RightEye);
        }

        if (!m_MouthOpen) {
            Face.SetMouthState(pose.DefaultMouth);
        } else {
            Face.SetMouthState(pose.OpenMouth);
        }

        if (!m_EyeTracking || !m_CurrentlyBlinking) {
            Face.SetSpriteOffset(ArgoFace.PartId.LEye, pose.LeftEyeOffset);
            Face.SetSpriteOffset(ArgoFace.PartId.REye, pose.RightEyeOffset);
        }

        Face.SetSpriteOffset(ArgoFace.PartId.LEye_Pupil, pose.LeftPupilOffset);
        Face.SetSpriteOffset(ArgoFace.PartId.REye_Pupil, pose.RightPupilOffset);

        // eyebrows
        Face.SetBrowState(ArgoFace.PartId.LBrow, pose.LeftBrow);
        Face.SetBrowState(ArgoFace.PartId.RBrow, pose.RightBrow);
        Face.SetSpriteOffset(ArgoFace.PartId.LBrow, pose.LeftBrowOffset);
        Face.SetSpriteOffset(ArgoFace.PartId.RBrow, pose.RightBrowOffset);

        // background
        if ((pose.Flags & ArgoFacePoseFlags.BigCelebration) != 0) {
            Face.SetBackgroundState(ArgoFace.BackgroundState.Celebrate);
            Face.SetSparkleState(ArgoFace.SparkleState.All);
            CelebrationPlayer.Stop();
            CelebrationPlayer.clip = BigCelebrationSfx;
            CelebrationPlayer.Play();
        } else if ((pose.Flags & ArgoFacePoseFlags.Celebration) != 0) {
            Face.SetBackgroundState(ArgoFace.BackgroundState.Pleased);
            Face.SetSparkleState(ArgoFace.SparkleState.Top);
            CelebrationPlayer.Stop();
            CelebrationPlayer.clip = CelebrationSfx;
            CelebrationPlayer.Play();
        } else {
            Face.SetBackgroundState(ArgoFace.BackgroundState.Normal);
            Face.SetSparkleState(ArgoFace.SparkleState.Off);
        }

        // sparkles
    }

    private void ApplyPose(NamedArgoFacePose pose) {
        ApplyPose(pose.Pose, pose.Id);
    }

    #region Looking

    private void UpdateLooking() {
        if ((m_CurrentPose.Flags & ArgoFacePoseFlags.DisableEyeTracking) != 0) {
            return;
        }

        if (!TryGetLookVector(out Vector3 lookVec)) {
            return;
        }

        Vector2 vec2d = (Vector2) lookVec;
        vec2d.x = Mathf.Round(-vec2d.x * LookScale); // offsets are horizontally flipped
        vec2d.y = Mathf.Round(vec2d.y * LookScale);

        Face.SetSpriteOffset(ArgoFace.PartId.LEye, vec2d);
        Face.SetSpriteOffset(ArgoFace.PartId.REye, vec2d);
    }

    private bool TryGetLookVector(out Vector3 vector) {
        if (!LookTarget) {
            vector = default;
            return false;
        }

        Vector3 lookPos = LookTarget.position;
        Vector3 lookPosLocal = LookRoot.InverseTransformPoint(lookPos);
        vector = lookPosLocal.normalized;
        return true;
    }

    #endregion // Looking

    #region Mouth

    private bool CheckIsSpeaking() {
        if (!m_IsVoicing) {
            return false;
        }

        long now = Frame.Timestamp();
        if (Frame.Timestamp() < m_NextTalkCheckTimestamp) {
            return m_CurrentlyTalking;
        }

        m_NextTalkCheckTimestamp = now + Stopwatch.Frequency / 20;

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

    private void UpdateMouthFlap() {
        bool flapping = CheckIsSpeaking();
        if (!flapping) {
            if (m_CurrentlyTalking) {
                m_CurrentlyTalking = false;
                m_MouthOpen = false;
                m_MouthFlapDelay = 0;
                Face.SetMouthState(m_CurrentPose.DefaultMouth);
            }
        } else {
            m_CurrentlyTalking = true;
            m_MouthFlapDelay -= Frame.DeltaTime;
            if (m_MouthFlapDelay <= 0) {
                m_MouthFlapDelay += FlapDelay;

                m_MouthOpen = !m_MouthOpen;
                Face.SetMouthState(m_MouthOpen ? m_CurrentPose.OpenMouth : m_CurrentPose.DefaultMouth);
            }
        }
    }

    #endregion // Mouth

    #region Blinking

    private void UpdateBlinking() {
        if (m_CurrentlyBlinking) {
            m_BlinkDelay -= Frame.DeltaTime;
            if (m_BlinkDelay <= 0) {
                m_EyeOpen = !m_EyeOpen;

                if (m_CurrentPose.LeftEye != ArgoFace.EyeState.Wink) {
                    Face.SetLeftEyeState(m_EyeOpen ? m_CurrentPose.LeftEye : ArgoFace.EyeState.Blink);
                }

                if (m_CurrentPose.RightEye != ArgoFace.EyeState.Wink) {
                    Face.SetRightEyeState(m_EyeOpen ? m_CurrentPose.RightEye : ArgoFace.EyeState.Blink);
                }

                m_BlinkDelay += m_EyeOpen ? BlinkDelay + RNG.Instance.NextFloat(BlinkRandom) : BlinkClosedDuration;
            }
        }
    }

    static private bool IsBlinkingEnabled(ArgoFace.EyeState eyeState) {
        switch (eyeState) {
            case ArgoFace.EyeState.Blink:
            case ArgoFace.EyeState.Wink:
            case ArgoFace.EyeState.Error:
                return false;

            default:
                return true;
        }
    }

    static private bool IsEyeOpen(ArgoFace.EyeState eyeState) {
        switch (eyeState) {
            case ArgoFace.EyeState.Blink:
                return false;

            default:
                return true;
        }
    }

    #endregion // Blinking
}

[Serializable]
public struct ArgoFacePose {
    public ArgoFacePoseFlags Flags;
    [AutoEnum] public ArgoFace.MouthState DefaultMouth;
    [AutoEnum] public ArgoFace.MouthState OpenMouth;

    [Header("Eyes")]
    [AutoEnum] public ArgoFace.EyeState LeftEye;
    [AutoEnum] public ArgoFace.EyeState RightEye;
    public Vector2 LeftEyeOffset;
    public Vector2 RightEyeOffset;
    public Vector2 LeftPupilOffset;
    public Vector2 RightPupilOffset;

    [Header("Brows")]
    [AutoEnum] public ArgoFace.EyebrowState LeftBrow;
    [AutoEnum] public ArgoFace.EyebrowState RightBrow;
    public Vector2 LeftBrowOffset;
    public Vector2 RightBrowOffset;
}

[Serializable]
public struct NamedArgoFacePose {
    public SerializedHash32 Id;
    //[Space, Inline]
    public ArgoFacePose Pose;
}

[Flags]
public enum ArgoFacePoseFlags : ushort {
    DisableEyeTracking = 0x01,
    Celebration = 0x02,
    BigCelebration = 0x04
}

public struct ArgoFacePoseKeyFrame {
    public float Time;
    public StringHash32 Id;
}