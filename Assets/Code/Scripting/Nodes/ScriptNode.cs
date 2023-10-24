using System;
using BeauUtil.Blocks;
using System.Collections.Generic;
using UnityEngine.Scripting;
using BeauUtil;
using BeauPools;
using BeauUtil.Variants;
using UnityEngine;
using Leaf;
using BeauUtil.Debugger;
using Leaf.Runtime;

namespace WeatherStation
{
    internal class ScriptNode : LeafNode
    {
        #region Serialized

        // Properties
        private string m_FullName = null;
        private ScriptNodeFlags m_Flags = 0;
        private ScriptNodePackage m_ScriptPackage = null;
        private StringHash32 m_ScriptPackageRoot = null;
        private TriggerNodeData m_TriggerData = null;
        private StringHash32 m_TriggerOrFunctionId = null;
        private LeafExpressionGroup m_TriggerOrFunctionConditions;
        private StringHash32 m_Target = null;

        #endregion // Serialized

        public ScriptNode(ScriptNodePackage inPackage, string inFullId)
            : base(inFullId, inPackage)
        {
            m_ScriptPackage = inPackage;
            m_ScriptPackageRoot = inPackage.RootPath();
            m_Id = inFullId;
            m_FullName = inFullId;
        }

        public string FullName() { return m_FullName; }

        public ScriptNodeFlags Flags() { return m_Flags; }
        public new ScriptNodePackage Package() { return m_ScriptPackage; }

        public bool IsCutscene() { return (m_Flags & ScriptNodeFlags.Cutscene) != 0; }
        public bool IsTrigger() { return (m_Flags & ScriptNodeFlags.TriggerResponse) != 0; }
        public bool IsFunction() { return (m_Flags & ScriptNodeFlags.Function) != 0; }

        public TriggerNodeData TriggerData { get { return m_TriggerData; } }
        public StringHash32 TriggerOrFunctionId() { return m_TriggerOrFunctionId; }
        public LeafExpressionGroup TriggerOrFunctionConditions() { return m_TriggerOrFunctionConditions; }

        public PersistenceLevel TrackingLevel()
        {
            if (m_TriggerData != null)
            {
                if (m_TriggerData.OnceLevel != PersistenceLevel.Untracked)
                    return m_TriggerData.OnceLevel;
                if (m_TriggerData.RepeatDuration > 0)
                    return PersistenceLevel.Session;
            }

            return PersistenceLevel.Untracked;
        }

        public StringHash32 TargetId()
        {
            return m_Target;
        }

        public TriggerPriority Priority()
        {
            if ((m_Flags & ScriptNodeFlags.Cutscene) != 0)
                return TriggerPriority.Cutscene;

            if (m_TriggerData != null)
            {
                return m_TriggerData.TriggerPriority;
            }

            return TriggerPriority.Low;
        }

        internal void ApplyDefaults(StringHash32 inTarget)
        {
            if (m_Target.IsEmpty)
                m_Target = inTarget;
            if (Bits.ContainsAny(m_Flags, ScriptNodeFlags.Cutscene) && m_TriggerData != null) {
                m_TriggerData.TriggerPriority = TriggerPriority.Cutscene;
            }
        }

        #region Parser

        [BlockMeta("cutscene"), Preserve]
        private void SetCutscene()
        {
            m_Flags |= ScriptNodeFlags.Cutscene;
        }

        [BlockMeta("interrupt"), Preserve]
        private void SetInterrupt()
        {
            m_Flags |= ScriptNodeFlags.Interrupt;
        }

        [BlockMeta("noDelay"), Preserve]
        private void SetNoDelay()
        {
            m_Flags |= ScriptNodeFlags.NoDelay;
        }

        [BlockMeta("ignoreDuringCutscene"), Preserve]
        private void IgnoreDuringCutscene()
        {
            m_Flags |= ScriptNodeFlags.SuppressDuringCutscene;
        }

        [BlockMeta("chatter"), Preserve]
        private void SetChatter()
        {
            m_Flags |= ScriptNodeFlags.CornerChatter | ScriptNodeFlags.SuppressDuringCutscene;
            if (m_Target.IsEmpty) {
                m_Target = GameConsts.Target_V1ctor;
            }
        }

        [BlockMeta("entrypoint"), Preserve]
        private void SetEntrypoint()
        {
            m_Flags |= ScriptNodeFlags.Entrypoint;
        }

        [BlockMeta("autosave"), Preserve]
        private void SetAutosave()
        {
            m_Flags |= ScriptNodeFlags.Autosave;
        }

        [BlockMeta("trigger"), Preserve]
        private void SetTriggerResponse(StringHash32 inTriggerId)
        {
            m_Flags |= ScriptNodeFlags.TriggerResponse;
            if (m_TriggerData == null)
            {
                m_TriggerData = new TriggerNodeData();
            }
            m_TriggerOrFunctionId = inTriggerId;

            // Mapping Shortcut - Partner requests are always towards guide
            if (inTriggerId == GameTriggers.RequestPartnerHelp)
            {
                m_Target = GameConsts.Target_V1ctor;;
            }
        }

        [BlockMeta("function"), Preserve]
        private void SetFunction(StringHash32 inFunctionId)
        {
            m_TriggerOrFunctionId = inFunctionId;
            m_Flags |= ScriptNodeFlags.Function;
        }

        [BlockMeta("who"), Preserve]
        private void SetTriggerTarget(StringHash32 inTargetId)
        {
            m_Target = inTargetId;
            if (inTargetId == "*") {
                m_Flags |= ScriptNodeFlags.AnyTarget;
            }
        }

        [BlockMeta("triggerPriority"), Preserve]
        private void SetTriggerPriority(TriggerPriority inPriority)
        {
            if (m_TriggerData != null)
            {
                m_TriggerData.TriggerPriority = inPriority;
            }
            else if (IsFunction())
            {
                Log.Warn("[ScriptNode] `triggerPriority` is not implemented for function node '{0}'", m_FullName);
            }
        }

        [BlockMeta("when"), Preserve]
        private void SetTriggerConditions(StringSlice inConditionsList)
        {
            m_TriggerOrFunctionConditions = LeafUtils.CompileExpressionGroup(this, inConditionsList);
            if (m_TriggerData != null)
            {
                m_TriggerData.Score += m_TriggerOrFunctionConditions.Count;
            }
        }

        [BlockMeta("boostScore"), Preserve]
        private void AdjustTriggerScore(int inScore)
        {
            if (m_TriggerData != null)
            {
                m_TriggerData.Score += inScore;
            }
            else if (IsFunction())
            {
                Log.Warn("[ScriptNode] `boostScore` is not implemented for function node '{0}'", m_FullName);
            }
        }

        [BlockMeta("once"), Preserve]
        private void SetOnce(StringSlice inCategory)
        {
            if (m_TriggerData != null)
            {
                m_TriggerData.RepeatDuration = 0;
                m_TriggerData.OnceLevel = inCategory.Equals("session") ? PersistenceLevel.Session : PersistenceLevel.Profile;
            }
            else if (IsFunction())
            {
                Log.Warn("[ScriptNode] `once` is not implemented for function node '{0}'", m_FullName);
            }
        }

        [BlockMeta("repeat"), Preserve]
        private void SetRepeat(uint inDuration)
        {
            if (m_TriggerData != null)
            {
                m_TriggerData.RepeatDuration = (int) inDuration;
                m_TriggerData.OnceLevel = PersistenceLevel.Untracked;
            }
            else if (IsFunction())
            {
                Log.Warn("[ScriptNode] `repeat` is not implemented for function node '{0}'", m_FullName);
            }
        }

        #endregion // Parser

        /// <summary>
        /// Resolves a node id given a relative or absolute path.
        /// Relative paths start with '.', whereas absolute ones do not.
        /// </summary>
        static public StringHash32 ResolveNodeId(ScriptNode inFrom, StringSlice inPath) {
            if (inPath.StartsWith('.')) {
                return inFrom.m_ScriptPackageRoot.Concat(inPath);
            }

            return inPath;
        }
    }
    
    [Flags]
    internal enum ScriptNodeFlags
    {
        Cutscene = 0x01,
        Entrypoint = 0x02,
        TriggerResponse = 0x04,
        CornerChatter = 0x10,
        SuppressDuringCutscene = 0x20,
        Function = 0x40,
        Autosave = 0x80,
        Interrupt = 0x100,
        NoDelay = 0x200,
        AnyTarget = 0x400
    }
}