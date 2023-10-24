using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Variants;
//using Aqua.Profile;
//using Aqua.Debugging;
using BeauUtil.Debugger;
using System.Text;
using Leaf.Runtime;

namespace WeatherStation {
    internal class TriggerResponseSet : IReadOnlyCollection<ScriptNode>
    {
        private readonly List<ScriptNode> m_TriggerNodes = new List<ScriptNode>(16);
        private bool m_Sorted = true;

        #region Add/Remove

        /// <summary>
        /// Adds a node to the response set.
        /// </summary>
        public bool AddNode(ScriptNode inNode)
        {
            if (m_TriggerNodes.Contains(inNode))
            {
                Log.Error("[TriggerResponseSet] Cannot add node '{0} twice", inNode.Id());
                return false;
            }

            int prevCount = m_TriggerNodes.Count;

            m_TriggerNodes.Add(inNode);
            m_Sorted &= prevCount == 0 || m_TriggerNodes[prevCount - 1].TriggerData.Score < inNode.TriggerData.Score;
            return true;
        }

        /// <summary>
        /// Removes a node from the response set.
        /// </summary>
        public bool RemoveNode(ScriptNode inNode)
        {
            if (m_TriggerNodes.FastRemove(inNode))
            {
                m_Sorted = false;
                return true;
            }

            return false;
        }

        #endregion // Add/Remove

        #region Locating

        /// <summary>
        /// Returns the highest-scoring nodes for this response set.
        /// </summary>
        public int GetHighestScoringNodes(LeafEvalContext inContext, ScriptingData inScriptData, StringHash32 inTarget, Dictionary<StringHash32, ScriptThread> inTargetStates, ICollection<ScriptNode> outNodes, ref int ioMinScore)
        {
            Optimize();

            ScriptNode node;
            TriggerNodeData triggerData;
            int count = 0;
            for(int nodeIdx = 0, nodeCount = m_TriggerNodes.Count; nodeIdx < nodeCount; ++nodeIdx)
            {
                node = m_TriggerNodes[nodeIdx];
                triggerData = node.TriggerData;

                DebugService.Log(LogMask.Scripting, "Evaluating trigger node '{0}'...", node.Id().ToDebugString());

                if (!node.Package().IsActive())
                {
                    DebugService.Log(LogMask.Scripting, "... node package is being unloaded");
                    continue;
                }

                // score cutoff
                if (triggerData.Score < ioMinScore)
                {
                    DebugService.Log(LogMask.Scripting, "...higher-scoring node has already been found");
                    break;
                }

                // not the right target
                if ((node.Flags() & ScriptNodeFlags.AnyTarget) == 0 && !inTarget.IsEmpty && inTarget != node.TargetId())
                {
                    DebugService.Log(LogMask.Scripting, "...node has mismatched target (desired '{0}', node '{1}')", inTarget.ToDebugString(), node.TargetId().ToDebugString());
                    continue;
                }

                // cannot play during cutscene
                if ((node.Flags() & ScriptNodeFlags.SuppressDuringCutscene) != 0 && Script.ShouldBlock())
                {
                    DebugService.Log(LogMask.Scripting, "...cutscene is playing");
                    continue;
                }

                // cannot play due to once
                if (triggerData.OnceLevel != PersistenceLevel.Untracked && inScriptData.HasSeen(node.Id(), triggerData.OnceLevel))
                {
                    DebugService.Log(LogMask.Scripting, "...node has already been seen");
                    continue;
                }

                // cannot play due to repetition
                if (triggerData.RepeatDuration > 0 && inScriptData.HasRecentlySeen(node.Id(), triggerData.RepeatDuration))
                {
                    DebugService.Log(LogMask.Scripting, "...node was seen too recently");
                    continue;
                }

                // cannot play due to priority
                if (node.TargetId() != StringHash32.Null)
                {
                    ScriptThread currentThread;
                    if (inTargetStates.TryGetValue(node.TargetId(), out currentThread))
                    {
                        bool higherPriority;
                        if ((node.Flags() & ScriptNodeFlags.Interrupt) != 0)
                            higherPriority = currentThread.Priority() > triggerData.TriggerPriority;
                        else
                            higherPriority = currentThread.Priority() >= triggerData.TriggerPriority;
                            
                        if (higherPriority)
                        {
                            DebugService.Log(LogMask.Scripting, "...higher-priority node ({0}) is executing for target '{1}'", currentThread.InitialNodeName(), node.TargetId().ToDebugString());
                            continue;
                        }
                    }
                }

                // cannot play due to conditions
                if (node.TriggerOrFunctionConditions().Count > 0)
                {
                    LeafExpression failure;
                    Variant result = node.TriggerOrFunctionConditions().Evaluate(inContext, out failure);
                    if (!result.AsBool())
                    {
                        if (DebugService.IsLogging(LogMask.Scripting))
                        {
                            DebugService.Log(LogMask.Scripting, "...node condition '{0}' failed", failure.ToDebugString(node));
                        }

                        continue;
                    }
                }

                DebugService.Log(LogMask.Scripting, "...node passed!");
                outNodes.Add(node);
                ioMinScore = triggerData.Score;
                ++count;
            }

            return count;
        }

        #endregion // Locating

        #region Sorting

        /// <summary>
        /// Optimizes the response set.
        /// </summary>
        public void Optimize()
        {
            if (m_Sorted)
                return;

            m_TriggerNodes.Sort(ScriptNodeComparer);
            m_Sorted = true;
        }

        static private readonly Comparison<ScriptNode> ScriptNodeComparer = (l, r) => {
            return Math.Sign(r.TriggerData.Score - l.TriggerData.Score);
        };

        #endregion // Sorting

        #region ICollection

        public int Count { get { return m_TriggerNodes.Count; } }

        public IEnumerator<ScriptNode> GetEnumerator()
        {
            return m_TriggerNodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion // ICollection

        internal void Dump(StringBuilder ioBuilder, StringHash32 inTriggerId)
        {
            ioBuilder.Append('\n').Append(inTriggerId.ToDebugString()).Append(" (").Append(m_TriggerNodes.Count).Append(")");
            foreach(var node in m_TriggerNodes)
            {
                ioBuilder.Append("\n - ").Append(node.FullName());
            }
        }
    }
}