using System;
using System.Collections.Generic;
using System.IO;
using BeauUtil;
using FieldDay.SharedState;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Networking;

namespace WeatherStation {
    public class VoiceoverLoadState : SharedStateComponent {
        public string BasePath = "vo/";
        public List<GameObject> VOCharacters = new List<GameObject>(8);

        [NonSerialized] public string LanguagePath = "en/";

        #region State

        public Dictionary<StringHash32, string> LineCodeToFileNameMap = MapUtils.Create<StringHash32, string>(1024);

        public Dictionary<StringHash32, VOLineEntry> EntryMap = MapUtils.Create<StringHash32, VOLineEntry>(256);
        public Dictionary<StringHash32, VOFileEntry> FileMap = MapUtils.Create<StringHash32, VOFileEntry>(128);

        public RingBuffer<StringHash32> EntryLoadQueue = new RingBuffer<StringHash32>(64, RingBufferMode.Expand);
        public RingBuffer<StringHash32> EntryUnloadQueue = new RingBuffer<StringHash32>(64, RingBufferMode.Expand);

        public Dictionary<StringHash32, GameObject> CharacterAudioMap = MapUtils.Create<StringHash32, GameObject>(8);

        [NonSerialized] public StringHash32 CurrentLoadingFileId;
        [NonSerialized] public UnityWebRequest CurrentLoad;

        #endregion // State
    }

    public struct VOFileEntry {
        public AudioClip Clip;
    }

    public struct VOLineEntry {
        public StringHash32 PathHash;
        public string Path;
    }

    static public class VoiceoverUtility {
        [SharedStateReference] static public VoiceoverLoadState Loader { get; private set; }

        /// <summary>
        /// Returns the lookup key for the given line.
        /// </summary>
        static public StringHash32 GetKeyForLine(StringHash32 lineCode) {
            return GetEntryForLine(lineCode).PathHash;
        }

        /// <summary>
        /// Returns the map entry for the given line.
        /// </summary>
        static public VOLineEntry GetEntryForLine(StringHash32 lineCode) {
            if (Loader.EntryMap.TryGetValue(lineCode, out var map)) {
                return map;
            }
            string name;
            if (!Loader.LineCodeToFileNameMap.TryGetValue(lineCode, out name)) {
                name = lineCode.HashValue.ToString("X8") + ".mp3";
            }
            VOLineEntry entry;
            entry.Path = Path.Combine(Loader.BasePath, Loader.LanguagePath, name);
            entry.PathHash = StringHash32.Fast(entry.Path);
            Loader.EntryMap.Add(lineCode, entry);
            return entry;
        }

        /// <summary>
        /// Returns the map entry for the given line.
        /// </summary>
        static public bool TryGetEntryForLine(StringHash32 lineCode, out VOLineEntry entry) {
            return Loader.EntryMap.TryGetValue(lineCode, out entry);
        }

        /// <summary>
        /// Attempts to get the clip for the given line code.
        /// </summary>
        static public bool TryGetClip(StringHash32 lineCode, out AudioClip clip) {
            StringHash32 key = GetKeyForLine(lineCode);
            bool found = Loader.FileMap.TryGetValue(key, out var entry);
            clip = entry.Clip;
            return found;
        }
    
        static public void QueueLineLoad(StringHash32 lineCode) {
            VOLineEntry entry = GetEntryForLine(lineCode);
            if (Loader.FileMap.ContainsKey(entry.PathHash)) {
                return;
            }
            Loader.EntryLoadQueue.PushBack(lineCode);
        }

        static public void QueueImmediateLineLoad(StringHash32 lineCode) {
            VOLineEntry entry = GetEntryForLine(lineCode);
            if (Loader.FileMap.ContainsKey(entry.PathHash)) {
                return;
            }
            Loader.EntryLoadQueue.PushFront(lineCode);
        }

        static public void UnloadLine(StringHash32 lineCode) {
            Loader.EntryUnloadQueue.PushBack(lineCode);
        }

        static public GameObject GetCharacterForLineCode(StringHash32 lineCode) {
            if(Loader.CharacterAudioMap.ContainsKey(lineCode)) {
                return Loader.CharacterAudioMap[lineCode];
            } else {
                //Debug.Log(lineCode);
                string s = lineCode.ToDebugString();
                for(int i = 0; i < Loader.VOCharacters.Count; ++i) {
                    if(s == Loader.VOCharacters[i].name) {
                        Loader.CharacterAudioMap.Add(lineCode, Loader.VOCharacters[i]);
                        return Loader.VOCharacters[i];
                    }
                }
            }

            return null;
        }
    }
}