using System;
using BeauPools;
using BeauUtil;
using FieldDay;
using FieldDay.Pipes;
using FieldDay.SharedState;
using UnityEngine;

namespace WeatherStation {
    public class SfxState : SharedStateComponent, IRegistrationCallbacks {
        [Header("Configuration")]
        public float DefaultMinDistance = 1;
        public float DefaultMaxDistance = 5;
        public float DefaultSpread = 5;

        [NonSerialized] public Pipe<SfxCommand> Commands = new Pipe<SfxCommand>(32, true);

        // active list
        [NonSerialized] public RingBuffer<SfxActiveData> ActiveSfx = new RingBuffer<SfxActiveData>(32, RingBufferMode.Expand);

        // update tables

        [NonSerialized] public LLTable<SfxPositionUpdateData> PositionUpdateTable;
        [NonSerialized] public LLIndexList PositionUpdateList;

        [NonSerialized] public LLTable<SfxTweenData> TweenUpdateTable;
        [NonSerialized] public LLIndexList TweenUpdateList;

        [NonSerialized] public DynamicPool<AudioSource> SourceAllocator;
        [NonSerialized] public UniqueIdAllocator16 HandleAllocator = new UniqueIdAllocator16(64);

        void IRegistrationCallbacks.OnDeregister() {
            SourceAllocator.Clear();
            ActiveSfx.Clear();
            HandleAllocator.Reset();
        }

        void IRegistrationCallbacks.OnRegister() {
            SourceAllocator = new DynamicPool<AudioSource>(32, (p) => {
                GameObject go = new GameObject("sfx");
                go.transform.SetParent(transform);
                AudioSource src = go.AddComponent<AudioSource>();
                src.enabled = false;
                src.loop = false;
                src.playOnAwake = false;
                src.spatialBlend = 1;
                src.spatialize = true;
                src.rolloffMode = AudioRolloffMode.Linear;
                src.minDistance = DefaultMinDistance;
                src.maxDistance = DefaultMaxDistance;
                src.spread = DefaultSpread;
                return src;
            });
            SourceAllocator.Config.RegisterOnDestruct((p, a) => Destroy(a.gameObject));
            SourceAllocator.Config.RegisterOnFree((p, a) => {
                a.Stop();
                a.clip = null;
#if UNITY_EDITOR
                a.gameObject.name = "sfx";
#endif // UNITY_EDITOR
                a.enabled = false;
            });

            SourceAllocator.Prewarm(8);

            PositionUpdateTable = new LLTable<SfxPositionUpdateData>(16);
            TweenUpdateTable = new LLTable<SfxTweenData>(16);

            PositionUpdateList = LLIndexList.Empty;
            TweenUpdateList = LLIndexList.Empty;
        }
    }

    static public class Sfx {
        #region OneShot

        static public void OneShot(AudioClip clip, Vector3 position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = clip,
                    TransformOffset = position,
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(SfxAsset asset, Vector3 position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = asset,
                    TransformOffset = position,
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(StringHash32 assetId, Vector3 position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayClipFromName,
                Play = new SfxPlayData() {
                    Asset = assetId,
                    TransformOffset = position,
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(AudioClip clip, Transform position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = clip,
                    TransformOrAudioSourceId = UnityHelper.Id(position),
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(SfxAsset asset, Transform position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = asset,
                    TransformOrAudioSourceId = UnityHelper.Id(position),
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(StringHash32 assetId, Transform position) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayClipFromName,
                Play = new SfxPlayData() {
                    Asset = assetId,
                    TransformOrAudioSourceId = UnityHelper.Id(position),
                    Pitch = 1,
                    Volume = 1
                }
            });
        }

        static public void OneShot(AudioClip clip, AudioSource output) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = clip,
                    TransformOrAudioSourceId = UnityHelper.Id(output),
                    Pitch = 1,
                    Volume = 1,
                    Flags = SfxPlaybackFlags.UserProvidedSource
                }
            });
        }

        static public void OneShot(SfxAsset asset, AudioSource output) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayFromAssetRef,
                Play = new SfxPlayData() {
                    Asset = asset,
                    TransformOrAudioSourceId = UnityHelper.Id(output),
                    Pitch = 1,
                    Volume = 1,
                    Flags = SfxPlaybackFlags.UserProvidedSource
                }
            });
        }

        static public void OneShot(StringHash32 assetId, AudioSource output) {
            Find.State<SfxState>().Commands.Write(new SfxCommand() {
                Type = SfxCommandType.PlayClipFromName,
                Play = new SfxPlayData() {
                    Asset = assetId,
                    TransformOrAudioSourceId = UnityHelper.Id(output),
                    Pitch = 1,
                    Volume = 1,
                    Flags = SfxPlaybackFlags.UserProvidedSource
                }
            });
        }

        #endregion // OneShot
    }
}