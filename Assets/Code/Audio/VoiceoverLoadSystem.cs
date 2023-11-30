using System;
using BeauUtil;
using BeauUtil.Debugger;
using EasyAssetStreaming;
using FieldDay;
using FieldDay.Systems;
using UnityEngine;
using UnityEngine.Networking;

namespace WeatherStation {
    [SysUpdate(GameLoopPhaseMask.PreUpdate | GameLoopPhaseMask.UnscaledUpdate)]
    public class VoiceoverLoadSystem : SharedStateSystemBehaviour<VoiceoverLoadState> {
        public override void ProcessWork(float deltaTime) {
            // TODO: Implement unloading
            //if (m_State.EntryUnloadQueue.TryPopFront(out StringHash32 lineCode)) {
            //    m_State.EntryMap.TryGetValue()
            //}

            bool hasLoad = m_State.CurrentLoad != null;
            if (hasLoad) {
                if (m_State.CurrentLoad.isDone) {
                    if (m_State.CurrentLoad.result != UnityWebRequest.Result.Success) {
                        //Log.Error("[VoiceoverLoadSystem] Could not load clip '{0}'", m_State.CurrentLoadingFileId);
                        m_State.FileMap.Add(m_State.CurrentLoadingFileId, default);
                    } else {
                        AudioClip clip = ((DownloadHandlerAudioClip) m_State.CurrentLoad.downloadHandler).audioClip;
                        m_State.FileMap.Add(m_State.CurrentLoadingFileId, new VOFileEntry() {
                            Clip = clip
                        });
                        Log.Msg("[VoiceoverLoadSystem] Loaded clip '{0}'", m_State.CurrentLoadingFileId);
                    }
                    hasLoad = false;
                    m_State.CurrentLoad.Dispose();
                    m_State.CurrentLoadingFileId = default;
                    m_State.CurrentLoad = null;
                }
            }

            while(!hasLoad && m_State.EntryLoadQueue.TryPopFront(out StringHash32 lineCode)) {
                var entry = VoiceoverUtility.GetEntryForLine(lineCode);
                if (m_State.FileMap.ContainsKey(entry.Path)) {
                    continue;
                }

                Uri uri = new Uri(Streaming.ResolveAddressToURL(entry.Path));
                m_State.CurrentLoadingFileId = entry.PathHash;

                Log.Msg("[VoiceoverLoadSystem] Beginning load of clip '{0}' at '{1}'", m_State.CurrentLoadingFileId, uri.ToString());

                m_State.CurrentLoad = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET, new DownloadHandlerAudioClip(uri, AudioType.UNKNOWN), null);
                m_State.CurrentLoad.SendWebRequest();
                hasLoad = true;
                break;
            }
        }
    }
}