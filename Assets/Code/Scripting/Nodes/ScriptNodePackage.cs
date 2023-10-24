using BeauUtil;
using BeauUtil.Blocks;
using System.Collections.Generic;
using UnityEngine;
using BeauUtil.IO;
using System.IO;
using Leaf;
using Leaf.Compiler;
using Leaf.Runtime;
using BeauUtil.Debugger;
using BeauUtil.Tags;
using BeauUtil.Streaming;
using UnityEngine.Scripting;

namespace WeatherStation
{
    internal class ScriptNodePackage : LeafNodePackage<ScriptNode>
    {
        private LeafAsset m_Source;
        private IHotReloadable m_HotReload;
        private int m_UseCount;
        private bool m_Active;
        [BlockMeta("defaultWho"), Preserve] private StringHash32 m_DefaultWho;

        public ScriptNodePackage(string inName)
            : base(inName)
        {
        }

        /// <summary>
        /// Attempts to retrieve the entrypoint with the given id.
        /// </summary>
        public bool TryGetEntrypoint(StringHash32 inId, out ScriptNode outNode)
        {
            ScriptNode node;
            if (m_Nodes.TryGetValue(inId, out node))
            {
                if ((node.Flags() & ScriptNodeFlags.Entrypoint) == ScriptNodeFlags.Entrypoint)
                {
                    outNode = node;
                    return true;
                }
            }

            outNode = null;
            return false;
        }

        /// <summary>
        /// Returns all entrypoints.
        /// </summary>
        public IEnumerable<ScriptNode> Entrypoints()
        {
            foreach(var node in m_Nodes.Values)
            {
                if ((node.Flags() & ScriptNodeFlags.Entrypoint) != 0)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Returns all responses.
        /// </summary>
        public IEnumerable<ScriptNode> Responses()
        {
            foreach(var node in m_Nodes.Values)
            {
                if ((node.Flags() & ScriptNodeFlags.TriggerResponse) != 0)
                {
                    yield return node;
                }
            }
        }

        /// <summary>
        /// Returns all functions.
        /// </summary>
        public IEnumerable<ScriptNode> Functions()
        {
            foreach(var node in m_Nodes.Values)
            {
                if ((node.Flags() & ScriptNodeFlags.Function) != 0)
                {
                    yield return node;
                }
            }
        }

        #region Use Count

        public void IncrementUseCount()
        {
            m_UseCount++;
        }

        public void DecrementUseCount()
        {
            Assert.True(m_UseCount > 0, "Unbalanced Increment/Decrement");
            m_UseCount--;
        }

        public bool IsInUse()
        {
            return m_UseCount > 0;
        }

        public bool SetActive(bool inbActive)
        {
            if (m_Active != inbActive)
            {
                m_Active = inbActive;
                return true;
            }

            return false;
        }

        public bool IsActive()
        {
            return m_Active;
        }

        #endregion // Use Count

        #region Reload

        /// <summary>
        /// Binds a source asset for hot reloading.
        /// </summary>
        public void BindAsset(LeafAsset inAsset)
        {
            m_Source = inAsset;

            if (m_HotReload != null)
            {
                ReloadableAssetCache.Remove(m_HotReload);
                Ref.TryDispose(ref m_HotReload);
            }

            if (inAsset != null)
            {
                m_HotReload = new HotReloadableAssetProxy<LeafAsset>(inAsset, "ScriptNodePackage", ReloadFromAsset);
                ReloadableAssetCache.Add(m_HotReload);
            }
        }

        private void ReloadFromAsset(LeafAsset inAsset, HotReloadAssetRemapArgs<LeafAsset> inRemap, HotReloadOperation inOperation)
        {
            var mgr = Services.Script;
            if (mgr != null)
            {
                mgr.RemovePackage(this);
            }

            m_Nodes.Clear();
            m_LineTable.Clear();
            m_Instructions = default;
            m_RootPath = string.Empty;

            if (inOperation == HotReloadOperation.Modified)
            {
                #if UNITY_EDITOR
                if (object.ReferenceEquals(inAsset, m_Source))
                {
                    string sourcePath = UnityEditor.AssetDatabase.GetAssetPath(m_Source);
                    UnityEditor.AssetDatabase.ImportAsset(sourcePath);
                    m_Source = UnityEditor.AssetDatabase.LoadAssetAtPath<LeafAsset>(sourcePath);
                }
                #else
                m_Source = inAsset;
                #endif // UNITY_EDITOR
                
                var self = this;
                BlockParser.Parse(ref self, CharStreamParams.FromBytes(m_Source.Bytes(), m_Name), Parsing.Block, Generator.Instance);

                if (mgr != null)
                {
                    mgr.AddPackage(this);
                }
            }
        }

        /// <summary>
        /// Unbinds the source asset.
        /// </summary>
        public void UnbindAsset()
        {
            m_Source = null;

            if (m_HotReload != null)
            {
                ReloadableAssetCache.Remove(m_HotReload);
                Ref.TryDispose(ref m_HotReload);
            }
        }

        #endregion // Reload

        public string DebugName()
        {
            #if UNITY_EDITOR
            if (m_Source != null)
            {
                return UnityEditor.AssetDatabase.GetAssetPath(m_Source);
            }
            #endif

            return Name();
        }

        #region Generator

        public class Generator : LeafParser<ScriptNode, ScriptNodePackage>
        {
            static public readonly Generator Instance = new Generator();

            #if UNITY_EDITOR
            private LeafCompilerFlags? m_FlagsOverride;
            #endif // UNITY_EDITOR

            #if UNITY_EDITOR
            public Generator(LeafCompilerFlags? overrideFlags = null) {
                m_FlagsOverride = overrideFlags;
            }
            #endif // UNITY_EDITOR

            public override bool IsVerbose
            {
                get
                {
                    #if UNITY_EDITOR
                    return true;
                    #else
                    return false;
                    #endif // UNITY_EDITOR
                }
            }

            public override LeafCompilerFlags CompilerFlags {
                get {
                    #if UNITY_EDITOR
                    if (m_FlagsOverride.HasValue)
                        return m_FlagsOverride.Value;
                    #endif // UNITY_EDITOR 
                    #if PRODUCTION
                    return LeafCompilerFlags.Debug | LeafCompilerFlags.Validate_MethodInvocation;
                    #else
                    return base.CompilerFlags;
                    #endif 
                }
            }

            public override ScriptNodePackage CreatePackage(string inFileName)
            {
                return new ScriptNodePackage(inFileName);
            }

            protected override ScriptNode CreateNode(string inFullId, StringSlice inExtraData, ScriptNodePackage inPackage)
            {
                return new ScriptNode(inPackage, inFullId);
            }

            public override void CompleteBlock(IBlockParserUtil inUtil, ScriptNodePackage inPackage, ScriptNode inBlock, bool inbError)
            {
                base.CompleteBlock(inUtil, inPackage, inBlock, inbError);
                inBlock.ApplyDefaults(inPackage.m_DefaultWho);
            }

            public override void OnEnd(IBlockParserUtil inUtil, ScriptNodePackage inPackage, bool inbError) {
                base.OnEnd(inUtil, inPackage, inbError);

                if (inbError) {
                    UnityEngine.Debug.LogErrorFormat("[ScriptNodePackage] Package '{0}' failed to compile", inPackage.Name());
                }
            }
        }

        #endregion // Generator
    }
}