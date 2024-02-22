using System;
using System.Collections;
using System.Collections.Generic;
using Aqua;
using BeauPools;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Blocks;
using BeauUtil.Tags;
using BeauUtil.Variants;
using Leaf;
using Leaf.Defaults;
using Leaf.Runtime;
using UnityEngine;
using WeatherStation;

namespace FieldDay.Scripting {
	
    public class ScriptPlugin : DefaultLeafManager<ScriptNode> {
        private readonly ScriptRuntimeState m_RuntimeState;
        private readonly Action LateEndCutsceneDelegate;
		
		static public bool ForceVOSkipSet = false;
		static public bool ForceKill = false;
		static public bool CompleteForceKill = false;
		
		static public AudioSource LastAudioSource = null;
		
		public readonly Func<bool> ForceVOSkip = new Func<bool>(() => (ForceVOSkipSet || ForceKill || CompleteForceKill));
		
        public ScriptPlugin(ScriptRuntimeState inHost, CustomVariantResolver inResolver, IMethodCache inCache = null, LeafRuntimeConfiguration inConfiguration = null)
            : base(inHost, inResolver, inCache, inConfiguration) {
            m_RuntimeState = inHost;

            BlockMetaCache.Default.Cache(typeof(ScriptNode));
            BlockMetaCache.Default.Cache(typeof(ScriptNodePackage));

            ConfigureTagStringHandling(new CustomTagParserConfig(), new TagStringEventHandler());

            LeafUtils.ConfigureDefaultParsers(m_TagParseConfig, this, null);
            LeafUtils.ConfigureDefaultHandlers(m_TagHandler, this);

            m_TagParseConfig.AddEvent("poses", "KeyFramePoses", ParsePoseData);

            m_TagHandler.Register(LeafUtils.Events.Character, () => { });
            m_TagHandler.Register("KeyFramePoses", HandlePoseEvent);

            LateEndCutsceneDelegate = LateDecrementNestedPauseCount;
        }

        public override LeafThreadHandle Run(ScriptNode inNode, ILeafActor inActor = null, VariantTable inLocals = null, string inName = null, bool inbImmediateTick = true) {
            if (inNode == null || !CheckPriorityState(inNode, m_RuntimeState)) {
                return default;
            }

            ScriptThread thread = m_RuntimeState.ThreadPool.Alloc();

            if ((inNode.Flags & ScriptNodeFlags.Cutscene) != 0) {
                m_RuntimeState.Cutscene.Kill();
                // TODO: Kill lower priority threads
            }

            TempAlloc<VariantTable> tempVars = m_RuntimeState.TablePool.TempAlloc();
            if (inLocals != null && inLocals.Count > 0) {
                inLocals.CopyTo(tempVars.Object);
                tempVars.Object.Base = inLocals.Base;
            }

            LeafThreadHandle handle = thread.Setup(inName, inActor, tempVars);
            tempVars.Dispose();
            thread.SetInitialNode(inNode);
            thread.AttachRoutine(Routine.Start(m_RoutineHost, LeafRuntime.Execute(thread, inNode)));

            m_RuntimeState.ActiveThreads.PushBack(handle);
            if ((inNode.Flags & ScriptNodeFlags.Cutscene) != 0) {
                m_RuntimeState.Cutscene = handle;
            }

            if (!inNode.TargetId.IsEmpty) {
                m_RuntimeState.ThreadMap.Threads[inNode.TargetId] = handle;
            }

            if (inbImmediateTick && m_RoutineHost.isActiveAndEnabled) {
                thread.ForceTick();
            }

            return handle;
        }

        public override void OnNodeEnter(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
            ScriptPersistence persistence = Lookup.State<ScriptPersistence>();
	
            StringHash32 nodeId = inNode.Id();
            persistence.RecentViewedNodeIds.PushFront(nodeId);
            if ((inNode.Flags & ScriptNodeFlags.Once) != 0) {
                switch (inNode.PersistenceType) {
                    case ScriptNodeMemoryTarget.Persistent: {
                        persistence.SessionViewedNodeIds.Add(nodeId);
                        persistence.SavedViewedNodeIds.Add(nodeId);
                        break;
                    }
                    case ScriptNodeMemoryTarget.Session: {
                        persistence.SessionViewedNodeIds.Add(nodeId);
                        break;
                    }
                }
            }

            if ((inNode.Flags & ScriptNodeFlags.Cutscene) != 0) {
                m_RuntimeState.NestedCutscenePauseCount++;
            }
        }

        public override void OnNodeExit(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
            if ((inNode.Flags & ScriptNodeFlags.Cutscene) != 0) {
                GameLoop.QueueEndOfFrame(LateEndCutsceneDelegate);
            }
        }

        private void LateDecrementNestedPauseCount() {
            m_RuntimeState.NestedCutscenePauseCount--;
            if (m_RuntimeState.NestedCutscenePauseCount == 0) {
                // TODO: End cutscene
            }
        }

        private void ParsePoseData(TagData inTag, object inContext, ref TagEventData ioData) {
            ioData.StringArgument = inTag.Data;
        }

        private void HandlePoseEvent(TagEventData inEvent, object inContext) {
            ArgoAnimator argo = LastAudioSource ? LastAudioSource.GetComponent<ArgoAnimator>() : null;
            if (argo != null) {
                var args = inEvent.ExtractStringArgs();
                for(int i = 0; i < args.Count; i++) {
                    TagData tag = TagData.Parse(args[i], Parsing.InlineEvent);
                    argo.QueuePoseChange(StringParser.ParseFloat(tag.Id), tag.Data);
                }
            }
        }

        public override IEnumerator RunLine(LeafThreadState<ScriptNode> inThreadState, LeafLineInfo inLine) {
            
			if (inLine.IsEmptyOrWhitespace)
                yield break;
			
			if(ForceKill) {
				yield break;
			}
			
			if(ForceVOSkipSet) {
				yield break;
			}
			
			LeafThreadHandle handle = inThreadState.GetHandle();
			
			if(CompleteForceKill) {
				CompleteForceKill = false;
				handle.Kill();
			}
			
            TagString eventString = inThreadState.TagString;
            TagStringEventHandler eventHandler = m_TagHandler;

            m_TagParser.Parse(ref eventString, inLine.Text, inThreadState);

            // TODO: Play VO?
            StringHash32 voiceoverLineCode = default;
            GameObject voiceCharacter = null;
            StringHash32 pose = default;

            // TODO: Voiceover
            if (eventString.TryFindEvent(LeafUtils.Events.Character, out TagEventData charData)) {
                StringHash32 charId = charData.GetStringHash();
                voiceoverLineCode = inLine.LineCode;
                VoiceoverUtility.QueueImmediateLineLoad(inLine.LineCode);
                // TODO: Find the character in the scene that maps to the character id
                // Play the VO from there
                voiceCharacter = VoiceoverUtility.GetCharacterForLineCode(charId);
                pose = charData.Argument1.AsStringHash();
            } else if (eventString.TryFindEvent(LeafUtils.Events.Pose, out TagEventData poseData)) {
                pose = poseData.GetStringHash();
            }

            TagStringEventHandler overrideHandler = m_TextDisplayer.PrepareLine(eventString, eventHandler);
            if (overrideHandler != null) {
                overrideHandler.Base = eventHandler;
                eventHandler = overrideHandler;
            }

            ArgoAnimator argo = null;

            if (!voiceoverLineCode.IsEmpty) {
				
                AudioClip clip = null;
                while (!VoiceoverUtility.TryGetClip(voiceoverLineCode, out clip)) {
                    yield return null;
                }

                // TODO: Prepare next line so we aren't left sitting
                //LeafRuntime.PredictLine(inThreadState);
				if(clip != null) {
                    if(voiceCharacter != null) {
                        AudioSource a = voiceCharacter.GetComponent<AudioSource>();
                        if(a != null) {
							LastAudioSource = a;
                            a.clip = clip;
                            a.Play();
                        }

                        if (voiceCharacter.TryGetComponent(out argo)) {
                            argo.SetPoseById(pose);
                        }

                    } else {
					    AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
                        LastAudioSource = null;
                    }
					
					if(((SubtitleDisplay)m_TextDisplayer).SubtitlesOn) {
						((SubtitleDisplay)m_TextDisplayer).ClipDisplayLength = clip.length;
					}
				}
            }

            for (int i = 0; i < eventString.Nodes.Length; i++) {
                TagNodeData node = eventString.Nodes[i];
                switch (node.Type) {
                    case TagNodeType.Event: {
                        IEnumerator coroutine;
                        if (eventHandler.TryEvaluate(node.Event, inThreadState, out coroutine)) {
                            // if executing this event somehow killed this thread, stop here
                            if (!handle.IsRunning())
                                yield break;

                            if (coroutine != null)
                                yield return coroutine;
                        }
                        break;
                    }

                    //case TagNodeType.Text: {
                    //    yield return Routine.Inline(m_TextDisplayer.TypeLine(eventString, node.Text));
                    //    break;
                    case TagNodeType.Text: {
						if(((SubtitleDisplay)m_TextDisplayer).SubtitlesOn) {
							List<IEnumerator> routineList = new List<IEnumerator>(2);
							routineList.Add(m_TextDisplayer.TypeLine(eventString, node.Text));
							routineList.Add(Routine.WaitCondition(ForceVOSkip));
							yield return Routine.Race(routineList);
						} else {
							List<IEnumerator> routineList = new List<IEnumerator>(2);
							routineList.Add(RoutineShortcuts.WaitToComplete(voiceCharacter.GetComponent<AudioSource>()));
							routineList.Add(Routine.WaitCondition(ForceVOSkip));
							yield return Routine.Race(routineList);
						}
						
						//if should skip vo fails passes here, turn off audio...
						if(ForceVOSkipSet || ForceKill) {
							if(voiceCharacter != null) {
								AudioSource a = voiceCharacter.GetComponent<AudioSource>();
								a.Stop();
							}
							ForceVOSkipSet = false;	//this just skips one VO...
						}
						break;
					}
                }
            }
			
			if(((SubtitleDisplay)m_TextDisplayer).SubtitlesOn) {
				if (eventString.RichText.Length > 0) {
					yield return m_TextDisplayer.CompleteLine();
				}
			}

            yield return Routine.Command.BreakAndResume;
        }

        public override void OnEnd(LeafThreadState<ScriptNode> inThreadState) {
            if (m_RuntimeState.Cutscene == inThreadState.GetHandle()) {
                m_RuntimeState.Cutscene = default;
            }
			//Debug.Log("HIT END");
			ForceKill = false;
            base.OnEnd(inThreadState);
        }
    
        public TagStringParser NewParser() {
            TagStringParser parser = new TagStringParser();
            parser.EventProcessor = m_TagParseConfig;
            parser.ReplaceProcessor = m_TagParseConfig;
            parser.Delimiters = Parsing.InlineEvent;
            return parser;
        }

        static private bool CheckPriorityState(ScriptNode node, ScriptRuntimeState runtime) {
            StringHash32 target = node.TargetId;
            if (target.IsEmpty) {
                return true;
            }

            ScriptThread thread = runtime.ThreadMap.GetCurrentThread(target);
            if (thread != null) {
                if (!ScriptDatabaseUtility.CanInterrupt(node, thread.Priority())) {
                    return false;
                }

                thread.Kill();
                runtime.ThreadMap.Threads.Remove(target);
            }

            return true;
        }

        public override bool TryLookupObject(StringHash32 inObjectId, LeafThreadState inThreadState, out object outObject) {
            bool result = m_RuntimeState.NamedActors.TryGetValue(inObjectId, out var evt);
            outObject = evt;
            return result;
        }
    }
}