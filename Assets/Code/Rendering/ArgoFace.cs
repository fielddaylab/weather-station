using System;
using System.Runtime.CompilerServices;
using ScriptableBake;
using UnityEngine;

public class ArgoFace : MonoBehaviour, IBaked {
    #region Types

    public enum PartId {
        Mouth,
        LEye,
        LEye_Pupil,
        REye,
        REye_Pupil,
        LBrow,
        RBrow
    }

    public enum MouthState {
        Open,
        Closed,
        Smirk,
        Frown,
        Frown_Right,
        Open_Smaller
    }

    public enum EyeState {
        Open,
        Half,
        Wink,
        Blink,
        Error
    }

    public enum EyebrowState {
        Hidden,
        Worried,
        Intrigued
    }

    public enum BackgroundState {
        Normal,
        Celebrate,
        Pleased,
        Alert
    }

    #endregion // Types

    #region Inspector

    [SerializeField] private Camera m_Camera;

    [Header("Parts")]
    [SerializeField] private SpriteRenderer[] m_Parts = new SpriteRenderer[7];
    [SerializeField, HideInInspector] private Transform[] m_PartTransforms = Array.Empty<Transform>();
    [SerializeField, HideInInspector] private Vector2[] m_PartOrigins = Array.Empty<Vector2>();

    [Header("Sprites")]
    [SerializeField] private Sprite m_DefaultIris;
    [SerializeField] private Sprite m_NarrowedEye;
    [SerializeField] private Sprite m_WinkEye;
    [SerializeField] private Sprite m_BlinkEye;
    [SerializeField] private Sprite m_OpenMouth;
    [SerializeField] private Sprite m_SmirkMouth;
    [SerializeField] private Sprite m_ClosedMouth;
    [SerializeField] private Sprite m_OpenMouthSmall;

    [Header("Colors")]
    [SerializeField] private Color32 m_DefaultBackground;
    [SerializeField] private Color32 m_CelebrationBackground;
    [SerializeField] private Color32 m_AlertBackground;
    [SerializeField] private Color32 m_PleasedBackground;

    #endregion // Inspector

    #region Unity Events

    private void Awake() {
        
    }

    #endregion // Unity Events

    #region Operations

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetBackgroundColor(Color color) {
        m_Camera.backgroundColor = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetSpriteEnabled(PartId part, bool enabled) {
        m_Parts[(int) part].enabled = enabled;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetSprite(PartId part, Sprite sprite) {
        m_Parts[(int) part].sprite = sprite;
    }

    private void SetSpriteFlip(PartId part, bool x, bool y) {
        SpriteRenderer rend = m_Parts[(int) part];
        rend.flipX = x;
        rend.flipY = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetSpriteFlipX(PartId part, bool x) {
        m_Parts[(int) part].flipX = x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetSpriteFlipY(PartId part, bool y) {
        m_Parts[(int) part].flipY = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetSpriteOffset(PartId part, Vector2 offset) {
        m_PartTransforms[(int) part].localPosition = m_PartOrigins[(int) part] + offset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetSpriteOffset(PartId part) {
        m_PartTransforms[(int) part].localPosition = m_PartOrigins[(int) part];
    }

    public void SetBackgroundState(BackgroundState backgroundState) {
        switch (backgroundState) {
            case BackgroundState.Normal: {
                SetBackgroundColor(m_DefaultBackground);
                break;
            }
            case BackgroundState.Alert: {
                SetBackgroundColor(m_AlertBackground);
                break;
            }
            case BackgroundState.Celebrate: {
                SetBackgroundColor(m_CelebrationBackground);
                break;
            }
            case BackgroundState.Pleased: {
                SetBackgroundColor(m_PleasedBackground);
                break;
            }
        }
    }

    public void SetLeftEyeState(EyeState eyeState) {
        switch (eyeState) {
            case EyeState.Open: {
                SetSpriteEnabled(PartId.LEye_Pupil, true);
                SetSprite(PartId.LEye, m_DefaultIris);
                break;
            }
            case EyeState.Half: {
                SetSpriteEnabled(PartId.LEye_Pupil, false);
                SetSprite(PartId.LEye, m_NarrowedEye);
                break;
            }
            case EyeState.Wink: {
                SetSpriteEnabled(PartId.LEye_Pupil, false);
                SetSprite(PartId.LEye, m_WinkEye);
                break;
            }
            case EyeState.Blink: {
                SetSpriteEnabled(PartId.LEye_Pupil, false);
                SetSprite(PartId.LEye, m_BlinkEye);
                break;
            }
            case EyeState.Error: {
                SetSpriteEnabled(PartId.LEye_Pupil, false);
                SetSprite(PartId.LEye, m_DefaultIris);
                break;
            }
        }
    }

    public void SetRightEyeState(EyeState eyeState) {
        switch (eyeState) {
            case EyeState.Open: {
                SetSpriteEnabled(PartId.REye_Pupil, true);
                SetSprite(PartId.REye, m_DefaultIris);
                break;
            }
            case EyeState.Half: {
                SetSpriteEnabled(PartId.REye_Pupil, false);
                SetSprite(PartId.REye, m_NarrowedEye);
                break;
            }
            case EyeState.Wink: {
                SetSpriteEnabled(PartId.REye_Pupil, false);
                SetSprite(PartId.REye, m_WinkEye);
                break;
            }
            case EyeState.Blink: {
                SetSpriteEnabled(PartId.REye_Pupil, false);
                SetSprite(PartId.REye, m_BlinkEye);
                break;
            }
            case EyeState.Error: {
                SetSpriteEnabled(PartId.REye_Pupil, false);
                SetSprite(PartId.REye, m_DefaultIris);
                break;
            }
        }
    }

    public void SetMouthState(MouthState mouthState) {
        switch (mouthState) {
            case MouthState.Open: {
                SetSprite(PartId.Mouth, m_OpenMouth);
                ResetSpriteOffset(PartId.Mouth);
                SetSpriteFlip(PartId.Mouth, false, false);
                break;
            }
            case MouthState.Closed: {
                SetSprite(PartId.Mouth, m_ClosedMouth);
                ResetSpriteOffset(PartId.Mouth);
                SetSpriteFlip(PartId.Mouth, false, false);
                break;
            }
            case MouthState.Smirk: {
                SetSprite(PartId.Mouth, m_SmirkMouth);
                SetSpriteOffset(PartId.Mouth, new Vector2(-2, 0));
                SetSpriteFlip(PartId.Mouth, false, false);
                break;
            }
            case MouthState.Frown: {
                SetSprite(PartId.Mouth, m_SmirkMouth);
                SetSpriteOffset(PartId.Mouth, new Vector2(-2, 0));
                SetSpriteFlip(PartId.Mouth, false, true);
                break;
            }
            case MouthState.Frown_Right: {
                SetSprite(PartId.Mouth, m_SmirkMouth);
                ResetSpriteOffset(PartId.Mouth);
                SetSpriteFlip(PartId.Mouth, true, true);
                break;
            }
            case MouthState.Open_Smaller: {
                SetSprite(PartId.Mouth, m_OpenMouthSmall);
                ResetSpriteOffset(PartId.Mouth);
                SetSpriteFlip(PartId.Mouth, false, false);
                break;
            }
        }
    }

    public void SetBrowState(PartId browIndex, EyebrowState browState) {
        switch (browState) {
            case EyebrowState.Hidden: {
                SetSpriteEnabled(browIndex, false);
                break;
            }
            case EyebrowState.Worried: {
                SetSpriteEnabled(browIndex, true);
                SetSpriteFlipY(browIndex, true);
                break;
            }
            case EyebrowState.Intrigued: {
                SetSpriteEnabled(browIndex, true);
                SetSpriteFlipY(browIndex, false);
                break;
            }
        }
    }

    #endregion // Operations

    #region IBaked

#if UNITY_EDITOR

    private void BuildData() {
        m_PartTransforms = new Transform[m_Parts.Length];
        m_PartOrigins = new Vector2[m_Parts.Length];

        for (int i = 0; i < m_PartTransforms.Length; i++) {
            if (m_Parts[i]) {
                m_PartTransforms[i] = m_Parts[i].transform;
                m_PartOrigins[i] = m_PartTransforms[i].localPosition;
            }
        }
    }

    int IBaked.Order => 0;

    bool IBaked.Bake(BakeFlags flags, BakeContext context) {
        m_Camera.orthographicSize = m_Camera.targetTexture.height / 2;
        BuildData();
        return true;
    }

#endif // UNITY_EDITOR

    #endregion // IBaked
}