using System;
using System.Runtime.InteropServices;
using BeauRoutine;
using BeauUtil;
using UnityEngine;

namespace WeatherStation {

    #region Playback Data

    public struct SfxActiveData {
        public UniqueId16 Handle;
        public SfxPlaybackFlags Flags;
        public StringHash32 Tag;
        public AudioSource Src;
        public ushort FrameStarted;
        public short PositionUpdateIndex;
        public short VolumeTweenIndex;
        public short PitchTweenIndex;
    }

    public struct SfxPositionUpdateData {
        public Transform AudioPosition;
        public Transform Reference;
        public Vector3 RefOffset;
        public Space RefOffsetSpace;
    }

    public struct SfxTweenData {
        public AudioSource Source;
        public SfxParamType Type;
        public Curve Curve;
        public float Start;
        public float Delta;
        public float InvDeltaTime;
        public float Progress;
    }

    #endregion // Playback Data

    #region Command Data

    public enum SfxCommandType : ushort {
        PlayClipFromName,
        PlayFromAssetRef,
        StopWithTag,
        StopAll,
        StopWithHandle,
        SetParameter
    }

    public enum SfxParamType : ushort {
        Volume,
        Pitch
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SfxCommand {
        [FieldOffset(0)] public SfxCommandType Type;
        [FieldOffset(4)] public SfxPlayData Play;
        [FieldOffset(4)] public SfxStopData Stop;
        [FieldOffset(4)] public SfxParamChangeData ParamChange;
    }

    public struct SfxPlayData {
        public SfxAssetRef Asset;
        public float Volume;
        public float Pitch;
        public float Delay;
        public StringHash32 Tag;
        public SfxPlaybackFlags Flags;
        public UniqueId16 Handle;

        public int TransformOrAudioSourceId;
        public Vector3 TransformOffset;
        public Space TransformOffsetSpace;
    }

    public struct SfxStopData {
        public SfxIdRef Id;
    }

    public struct SfxParamChangeData {
        public UniqueId16 Handle;
        public SfxParamType Param;
        public float Target;
        public float Duration;
        public Curve Easing;
    }

    #region Unions

    [StructLayout(LayoutKind.Explicit)]
    public struct SfxAssetRef {
        [FieldOffset(0)] public StringHash32 AssetId;
        [FieldOffset(0)] public int InstanceId;

        static public implicit operator SfxAssetRef(StringHash32 id) {
            return new SfxAssetRef() {
                AssetId = id
            };
        }

        static public implicit operator SfxAssetRef(UnityEngine.Object obj) {
            return new SfxAssetRef() {
                InstanceId = UnityHelper.Id(obj)
            };
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SfxIdRef {
        [FieldOffset(0)] public StringHash32 Id;
        [FieldOffset(0)] public UniqueId16 Handle;

        static public implicit operator SfxIdRef(StringHash32 id) {
            return new SfxIdRef() {
                Id = id
            };
        }

        static public implicit operator SfxIdRef(UniqueId16 handle) {
            return new SfxIdRef() {
                Handle = handle
            };
        }
    }

    #endregion // Unions

    /// <summary>
    /// Playback flags.
    /// </summary>
    [Flags]
    public enum SfxPlaybackFlags : ushort {
        Loop = 0x01,
        UserProvidedSource = 0x02,
        RandomizePlaybackStart = 0x04,
        ForceNonPositional = 0x08
    }

    #endregion // Command Data
}