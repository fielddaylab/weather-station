using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhaseMask.LateFixedUpdate | GameLoopPhaseMask.UnscaledUpdate | GameLoopPhaseMask.UnscaledLateUpdate, 10000)]
    public class SfxSystem : SharedStateSystemBehaviour<SfxState> {

        public override void ProcessWork(float deltaTime) {
            if (GameLoop.IsPhase(GameLoopPhase.LateFixedUpdate)) {
                UpdateActiveTable();
            }

            FlushCommandPipe();

            if (GameLoop.IsPhase(GameLoopPhase.UnscaledLateUpdate)) {
                UpdateAudioPositions();
                UpdateTweens(deltaTime);
            }
        }

        #region Commands

        private void FlushCommandPipe() {
            while (m_State.Commands.TryRead(out SfxCommand cmd)) {
                switch (cmd.Type) {
                    case SfxCommandType.StopAll: {
                        Cmd_StopAll();
                        break;
                    }

                    case SfxCommandType.StopWithHandle: {
                        Cmd_StopWithHandle(cmd.Stop.Id.Handle);
                        break;
                    }

                    case SfxCommandType.StopWithTag: {
                        Cmd_StopWithTag(cmd.Stop.Id.Id);
                        break;
                    }

                    case SfxCommandType.SetParameter: {
                        Cmd_SetParameter(cmd.ParamChange);
                        break;
                    }

                    case SfxCommandType.PlayClipFromName: {
                        Cmd_PlayClipFromName(cmd.Play);
                        break;
                    }

                    case SfxCommandType.PlayFromAssetRef: {
                        Cmd_PlayClipFromAsset(cmd.Play);
                        break;
                    }
                }
            }
        }

        private void Cmd_StopAll() {
            var activeTable = m_State.ActiveSfx;
            for (int i = 0; i < activeTable.Count; i++) {
                SfxActiveData data = activeTable[i];

                data.Src.Stop();
                m_State.SourceAllocator.Free(data.Src);
            }

            m_State.ActiveSfx.Clear();
            m_State.HandleAllocator.Reset();
            m_State.PositionUpdateTable.Clear(ref m_State.PositionUpdateList);
            m_State.TweenUpdateTable.Clear(ref m_State.TweenUpdateList);
        }

        private void Cmd_StopWithHandle(UniqueId16 handle) {
            int idx = GetActiveIndexForHandle(handle);
            if (idx >= 0) {
                KillActiveSfx(ref m_State.ActiveSfx[idx]);
                m_State.ActiveSfx.FastRemoveAt(idx);
            }
        }

        private void Cmd_StopWithTag(StringHash32 tag) {
            var activeTable = m_State.ActiveSfx;
            for (int i = activeTable.Count - 1; i >= 0; i--) {
                ref SfxActiveData data = ref activeTable[i];
                if (data.Tag == tag) {
                    KillActiveSfx(ref data);
                    activeTable.FastRemoveAt(i);
                }
            }
        }

        private void Cmd_SetParameter(SfxParamChangeData changeData) {
            int idx = GetActiveIndexForHandle(changeData.Handle);
            if (idx < 0) {
                return;
            }

            ref SfxActiveData data = ref m_State.ActiveSfx[idx];

            switch (changeData.Param) {
                case SfxParamType.Volume: {
                    if (changeData.Duration <= 0) {
                        FreeTween(ref data.VolumeTweenIndex);
                        data.Src.volume = changeData.Target;
                    } else {
                        SfxTweenData tweenData;
                        tweenData.Source = data.Src;
                        tweenData.Start = data.Src.volume;
                        tweenData.Delta = changeData.Target - tweenData.Start;
                        tweenData.InvDeltaTime = 1f / changeData.Duration;
                        tweenData.Progress = 0;
                        tweenData.Type = SfxParamType.Volume;
                        tweenData.Curve = changeData.Easing;

                        if (data.VolumeTweenIndex >= 0) {
                            m_State.TweenUpdateTable[data.VolumeTweenIndex] = tweenData;
                        } else {
                            data.VolumeTweenIndex = (short) m_State.TweenUpdateTable.PushBack(ref m_State.TweenUpdateList, tweenData);
                        }
                    }
                    break;
                }

                case SfxParamType.Pitch: {
                    if (changeData.Duration <= 0) {
                        FreeTween(ref data.PitchTweenIndex);
                        data.Src.pitch = changeData.Target;
                    } else {
                        SfxTweenData tweenData;
                        tweenData.Source = data.Src;
                        tweenData.Start = data.Src.pitch;
                        tweenData.Delta = changeData.Target - tweenData.Start;
                        tweenData.InvDeltaTime = 1f / changeData.Duration;
                        tweenData.Progress = 0;
                        tweenData.Type = SfxParamType.Pitch;
                        tweenData.Curve = changeData.Easing;

                        if (data.PitchTweenIndex >= 0) {
                            m_State.TweenUpdateTable[data.PitchTweenIndex] = tweenData;
                        } else {
                            data.PitchTweenIndex = (short) m_State.TweenUpdateTable.PushBack(ref m_State.TweenUpdateList, tweenData);
                        }
                    }
                    break;
                }
            }
        }

        private void Cmd_PlayClipFromName(SfxPlayData playData) {
            if (!Game.Assets.TryGetNamed(playData.Asset.AssetId, out SfxAsset asset)) {
                Log.Warn("[SfxSystem] No SfxAssets loaded with name '{0}'", playData.Asset.AssetId);
                FreeHandle(ref playData.Handle);
                return;
            }

            PlayClipInternal(playData, asset, null);
        }

        private void Cmd_PlayClipFromAsset(SfxPlayData playData) {
            var instance = Find.FromId(playData.Asset.InstanceId);
            if (instance == null) {
                Log.Warn("[SfxSystem] No clips or SfxAssets loaded with instance id '{0}'", playData.Asset.InstanceId);
                FreeHandle(ref playData.Handle);
                return;
            }

            AudioClip clip = instance as AudioClip;
            SfxAsset asset = instance as SfxAsset;

            if (clip == null && asset == null) {
                Log.Warn("[SfxSystem] No clips or SfxAssets loaded with instance id '{0}'", playData.Asset.InstanceId);
                FreeHandle(ref playData.Handle);
                return;
            }

            PlayClipInternal(playData, asset, clip);
        }

        private void PlayClipInternal(SfxPlayData playData, SfxAsset asset, AudioClip clip) {
            float spread = m_State.DefaultSpread;
            float range = m_State.DefaultMaxDistance;
            float spatialBlend = 1;
            AudioRolloffMode rolloff = AudioRolloffMode.Linear;

            if (asset != null) {
                if (asset.Randomizer == null) {
                    asset.Randomizer = new RandomDeck<AudioClip>(asset.Clips);
                }
                clip = asset.Randomizer.Next();

                playData.Volume *= asset.Volume.Generate();
                playData.Pitch *= asset.Pitch.Generate();
                playData.Delay += asset.Delay.Generate();

                if (playData.Tag.IsEmpty) {
                    playData.Tag = asset.Tag;
                }

                spread = asset.Spread;
                range = asset.Range;
                spatialBlend = asset.SpatialBlend;
                rolloff = asset.Rolloff;
            }

            UnityEngine.Object providedObj = Find.FromId(playData.TransformOrAudioSourceId);

            Transform trackPos;
            if ((playData.Flags & SfxPlaybackFlags.ForceNonPositional) != 0) {
                trackPos = null;
                spatialBlend = 0;
            } else {
                trackPos = providedObj as Transform;
            }

            AudioSource src;
            if ((playData.Flags & SfxPlaybackFlags.UserProvidedSource) != 0) {
                src = providedObj as AudioSource;
                Assert.NotNull(src, "UserProvidedSource flag sent but AudioSource not sent alongside it");
            } else {
                src = m_State.SourceAllocator.Alloc();
            }

            src.clip = clip;
            src.volume = playData.Volume;
            src.pitch = playData.Pitch;
            src.loop = (playData.Flags & SfxPlaybackFlags.Loop) != 0;
            src.time = (playData.Flags & SfxPlaybackFlags.RandomizePlaybackStart) != 0 ? RNG.Instance.NextFloat(clip.length) : 0;

#if UNITY_EDITOR
            src.gameObject.name = clip.name;
#endif // UNITY_EDITOR

            src.spatialBlend = spatialBlend;
            src.spatialize = spatialBlend > 0;
            src.maxDistance = range;
            src.minDistance = m_State.DefaultMinDistance;
            src.rolloffMode = rolloff;

            SfxActiveData active;
            active.Handle = playData.Handle;
            active.Flags = playData.Flags;
            active.Tag = playData.Tag;
            active.Src = src;
            active.FrameStarted = Frame.Index;
            active.VolumeTweenIndex = active.PitchTweenIndex = (short) -1;

            if ((playData.Flags & SfxPlaybackFlags.UserProvidedSource) == 0) {
                if (trackPos != null) {
                    SfxPositionUpdateData posUpdate;
                    posUpdate.AudioPosition = src.transform;
                    posUpdate.Reference = trackPos;
                    posUpdate.RefOffset = playData.TransformOffset;
                    posUpdate.RefOffsetSpace = playData.TransformOffsetSpace;
                    active.PositionUpdateIndex = (short) m_State.PositionUpdateTable.PushBack(ref m_State.PositionUpdateList, posUpdate);
                    ForceUpdateAudioPosition(posUpdate);
                } else {
                    src.transform.position = playData.TransformOffset;
                    active.PositionUpdateIndex = -1;
                }
            } else {
                active.PositionUpdateIndex = -1;
            }

            src.enabled = true;
            src.PlayDelayed(playData.Delay);
            m_State.ActiveSfx.PushBack(active);
        }

        #endregion // Commands

        #region Update Tables

        private void UpdateActiveTable() {
            var activeTable = m_State.ActiveSfx;
            for (int i = activeTable.Count - 1; i >= 0; i--) {
                ref SfxActiveData data = ref activeTable[i];
                if (!data.Src.isPlaying) {
                    if (Frame.Age(data.FrameStarted) > 4) {
                        KillActiveSfx(ref data);
                        activeTable.FastRemoveAt(i);
                    }
                } else {
                    data.FrameStarted = Frame.InvalidIndex;
                }
            }
        }

        private void UpdateAudioPositions() {
            if (m_State.PositionUpdateList.Length <= 0) {
                return;
            }

            var enumerator = m_State.PositionUpdateTable.GetEnumerator(m_State.PositionUpdateList);
            while (enumerator.MoveNext()) {
                var entry = enumerator.Current.Tag;
                ForceUpdateAudioPosition(entry);
            }
        }

        private void ForceUpdateAudioPosition(SfxPositionUpdateData entry) {
            Vector3 pos = entry.Reference.position;
            if (entry.RefOffset.sqrMagnitude > 0) {
                switch (entry.RefOffsetSpace) {
                    case Space.Self: {
                        pos += entry.Reference.TransformVector(entry.RefOffset);
                        break;
                    }
                    default: {
                        pos += entry.RefOffset;
                        break;
                    }
                }
            }

            entry.AudioPosition.position = pos;
        }

        private void UpdateTweens(float deltaTime) {
            if (m_State.TweenUpdateList.Length <= 0) {
                return;
            }

            var enumerator = m_State.TweenUpdateTable.GetEnumerator(m_State.TweenUpdateList);
            while (enumerator.MoveNext()) {
                ref SfxTweenData data = ref m_State.TweenUpdateTable[enumerator.Current.Index];
                data.Progress += Mathf.Min(1, data.Progress + deltaTime * data.InvDeltaTime);

                float value = data.Start + data.Delta * TweenUtil.Evaluate(data.Curve, data.Progress);
                switch (data.Type) {
                    case SfxParamType.Volume: {
                        data.Source.volume = value;
                        break;
                    }
                    case SfxParamType.Pitch: {
                        data.Source.pitch = value;
                        break;
                    }
                }

                if (data.Progress >= 1) {
                    // kill this
                    m_State.TweenUpdateTable.Remove(ref m_State.TweenUpdateList, enumerator.Current.Index);
                }
            }
        }

        #endregion // Update Tables

        #region Cleanup

        private void KillActiveSfx(ref SfxActiveData data) {
            data.Src.Stop();
            FreeHandle(ref data.Handle);
            FreePositionUpdate(ref data.PositionUpdateIndex);
            FreeTween(ref data.VolumeTweenIndex);
            FreeTween(ref data.PitchTweenIndex);

            if ((data.Flags & SfxPlaybackFlags.UserProvidedSource) == 0) {
                m_State.SourceAllocator.Free(data.Src);
            } else {
                data.Src.Stop();
            }
        }

        private void FreePositionUpdate(ref short index) {
            if (index >= 0) {
                m_State.PositionUpdateTable.Remove(ref m_State.PositionUpdateList, index);
                index = 0;
            }
        }

        private void FreeTween(ref short index) {
            if (index >= 0) {
                m_State.TweenUpdateTable.Remove(ref m_State.TweenUpdateList, index);
                index = 0;
            }
        }

        private void FreeHandle(ref UniqueId16 handle) {
            if (handle != UniqueId16.Invalid) {
                m_State.HandleAllocator.Free(handle);
                handle = default;
            }
        }

        #endregion // Cleanup

        #region Utility

        private int GetActiveIndexForHandle(UniqueId16 handle) {
            if (!m_State.HandleAllocator.IsValid(handle)) {
                return -1;
            }

            var activeTable = m_State.ActiveSfx;
            for(int i = 0; i < activeTable.Count; i++) {
                if (activeTable[i].Handle == handle) {
                    return i;
                }
            }

            return -1;
        }

        #endregion // Utility
    }
}