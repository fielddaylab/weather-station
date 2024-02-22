using System;
using System.Collections.Generic;
using BeauPools;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay.SharedState;
using Leaf;
using Leaf.Runtime;
using UnityEngine;
using WeatherStation;

namespace FieldDay.Scripting {
    [DisallowMultipleComponent]
    public sealed class ScriptDatabase : SharedStateComponent {
        // loaded
        public HashSet<ScriptNodePackage> RegisteredPackages = new HashSet<ScriptNodePackage>(8);
        public Dictionary<LeafAsset, ScriptNodePackage> LoadedSourceAssetMap = new Dictionary<LeafAsset, ScriptNodePackage>(CompareUtils.DefaultEquals<LeafAsset>());
        public Dictionary<StringHash32, ScriptNodeBucket> LoadedNodeBuckets = new Dictionary<StringHash32, ScriptNodeBucket>(CompareUtils.DefaultEquals<StringHash32>());
        public Dictionary<StringHash32, ScriptNode> LoadedRandomAccessNodes = new Dictionary<StringHash32, ScriptNode>(CompareUtils.DefaultEquals<StringHash32>());

        // loading
        public RingBuffer<LeafAsset> LoadQueue = new RingBuffer<LeafAsset>();
        [NonSerialized] public LeafAsset CurrentLoadAsset;
        [NonSerialized] public ScriptNodePackage CurrentLoadPackage;
        [NonSerialized] public AsyncHandle CurrentLoadHandle;
        
        // unloading
        public RingBuffer<LeafAsset> UnloadQueue = new RingBuffer<LeafAsset>();
    }

    static public class ScriptDatabaseUtility {
        #region Loading

        static public void Load(ScriptDatabase db, LeafAsset asset) {
            db.UnloadQueue.FastRemove(asset);

            if (db.LoadedSourceAssetMap.TryGetValue(asset, out ScriptNodePackage package)) {
                package.SetActive(true);
                return;
            }

            if (db.CurrentLoadAsset == asset || db.LoadQueue.Contains(asset)) {
                return;
            }

            db.LoadQueue.PushBack(asset);
        }

        static public void LoadNow(ScriptDatabase db, LeafAsset asset) {
            db.UnloadQueue.FastRemove(asset);

            if (db.LoadedSourceAssetMap.TryGetValue(asset, out ScriptNodePackage package)) {
                package.SetActive(true);
                return;
            }

            CancelCurrentLoad(db, asset);
            db.LoadQueue.FastRemove(asset);

            package = LeafAsset.Compile(asset, ScriptNodePackage.Parser.Instance);
            RegisterPackage(db, package, asset);
        }

        static public void Unload(ScriptDatabase db, LeafAsset asset) {
            if (CancelCurrentLoad(db, asset)) {
                return;
            }

            if (db.LoadedSourceAssetMap.TryGetValue(asset, out ScriptNodePackage package)) {
                if (package.SetActive(false)) {
                    db.UnloadQueue.PushBack(asset);
                }
            }
        }

        static private bool CancelCurrentLoad(ScriptDatabase db, LeafAsset asset) {
            if (ReferenceEquals(asset, db.CurrentLoadAsset)) {
                db.CurrentLoadAsset = null;
                db.CurrentLoadPackage.Clear();
                db.CurrentLoadPackage = null;
                db.CurrentLoadHandle.Cancel();
                return true;
            }

            return false;
        }

        #endregion // Loading

        #region Package Registration

        static public void RegisterPackage(ScriptDatabase db, ScriptNodePackage package, LeafAsset sourceAsset) {
            package.SetActive(true);

            if (!db.RegisteredPackages.Add(package)) {
                return;
            }

            db.LoadedSourceAssetMap.Add(sourceAsset, package);

            foreach(var node in package.Responses()) {
                ScriptNodeBucket bucket;
                if (!db.LoadedNodeBuckets.TryGetValue(node.TriggerOrFunctionId, out bucket)) {
                    bucket = new ScriptNodeBucket();
                    db.LoadedNodeBuckets.Add(node.TriggerOrFunctionId, bucket);
                }

                bucket.AddSorted(node);
            }

            foreach (var node in package.Functions()) {
                ScriptNodeBucket bucket;
                if (!db.LoadedNodeBuckets.TryGetValue(node.TriggerOrFunctionId, out bucket)) {
                    bucket = new ScriptNodeBucket();
                    db.LoadedNodeBuckets.Add(node.TriggerOrFunctionId, bucket);
                }

                bucket.AddUnsorted(node);
            }

            foreach(var node in package.Queued()) {
                db.LoadedRandomAccessNodes.Add(node.Id(), node);
            }

            package.m_SourceAsset = sourceAsset;

            VoiceoverUtility.LoadCustomLineNameMappings(package);

            Log.Msg("[ScriptDatabase] Script '{0}' registered", package.Name());
        }

        static public void DeregisterPackage(ScriptDatabase db, ScriptNodePackage package) {
            package.SetActive(false);

            if (!db.RegisteredPackages.Remove(package)) {
                return;
            }

            foreach (var node in package.Responses()) {
                ScriptNodeBucket bucket;
                if (db.LoadedNodeBuckets.TryGetValue(node.TriggerOrFunctionId, out bucket)) {
                    bucket.RemoveSorted(node);
                }
            }

            foreach (var node in package.Functions()) {
                ScriptNodeBucket bucket;
                if (db.LoadedNodeBuckets.TryGetValue(node.TriggerOrFunctionId, out bucket)) {
                    bucket.RemoveUnsorted(node);
                }
            }

            foreach (var node in package.Queued()) {
                db.LoadedRandomAccessNodes.Remove(node.Id());
            }

            Log.Msg("[ScriptDatabase] Script '{0}' deregistered", package.Name());

            db.LoadedSourceAssetMap.Remove(package.m_SourceAsset);
            package.m_SourceAsset = null;
            package.Clear();
        }

        #endregion // Package Registration

        #region Lookups

        static public ScriptNode FindRandomTrigger(ScriptDatabase db, StringHash32 bucketId, LeafEvalContext context, StringHash32 targetId) {
            ScriptNodeBucket bucket;
            if (db.LoadedNodeBuckets.TryGetValue(bucketId, out bucket)) {
                using(PooledList<ScriptNode> lookupList = PooledList<ScriptNode>.Create()) {
                    int count = bucket.GetHighestScoringSorted(context, targetId, ScriptUtility.Persistence, ScriptUtility.Runtime, lookupList);
                    if (count > 0) {
                        return ScriptUtility.Runtime.Random.Choose(lookupList);
                    }
                }
            }

            return null;
        }

        static public ScriptNode FindSpecificNode(ScriptDatabase db, StringHash32 nodeId) {
            ScriptNode node;
            if (db.LoadedRandomAccessNodes.TryGetValue(nodeId, out node)) {
                return node;
            }

            return null;
        }

        static public int FindAllFunctions(ScriptDatabase db, StringHash32 bucketId, LeafEvalContext context, StringHash32 targetId, ICollection<ScriptNode> results) {
            ScriptNodeBucket bucket;
            if (db.LoadedNodeBuckets.TryGetValue(bucketId, out bucket)) {
                using (PooledList<ScriptNode> lookupList = PooledList<ScriptNode>.Create()) {
                    return bucket.GetAllUnsorted(context, targetId, ScriptUtility.Persistence, ScriptUtility.Runtime, lookupList);
                }
            }

            return 0;
        }

        #endregion // Lookups

        #region Checks

        static public bool CanInterrupt(ScriptNode node, ScriptNodePriority priority) {
            ScriptNodePriority checkPriority = node.Priority;
            if ((node.Flags & ScriptNodeFlags.InterruptSamePriority) != 0) {
                checkPriority++;
            }
            return checkPriority > priority;
        }

        #endregion // Checks
    }
}