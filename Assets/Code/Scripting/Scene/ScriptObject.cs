using System;
//using Aqua.Scripting;
using BeauPools;
using BeauUtil;
using BeauUtil.Debugger;
using BeauUtil.Variants;
using Leaf.Runtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation
{
    [AddComponentMenu("Aqualab/Scripting/Script Object")]
    public sealed class ScriptObject : MonoBehaviour, IPoolAllocHandler, IPoolConstructHandler, IKeyValuePair<StringHash32, ScriptObject>, ILeafActor
    {
        #region Inspector

        [SerializeField] private SerializedHash32 m_Id = "";
        [SerializeField] private bool m_IsPersistent = false;
        [SerializeField] private bool m_StartDisabled = false;

        #endregion // Inspector

        [NonSerialized] private IScriptComponent[] m_ScriptComponents;
        [NonSerialized] private VariantTable m_Locals;
        [NonSerialized] private bool m_Pooled;

        #region KeyValue

        StringHash32 IKeyValuePair<StringHash32, ScriptObject>.Key { get { return m_Id; } }
        ScriptObject IKeyValuePair<StringHash32, ScriptObject>.Value { get { return this; } }

        #endregion // KeyValue

        public StringHash32 Id() { return m_Id; }

        #region ILeafActor

        StringHash32 ILeafActor.Id { get { return m_Id; } }
        public VariantTable Locals { get { return m_Locals ?? (m_Locals = new VariantTable()); } }

        #endregion // ILeafActor

        #region Leaf

        [LeafMember("Activate"), Preserve]
        private void Activate()
        {
            gameObject.SetActive(true);
        }

        [LeafMember("Deactivate"), Preserve]
        private void Deactivate()
        {
            gameObject.SetActive(false);
        }

        [LeafMember("ToggleActive"), Preserve]
        private void ToggleActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        [LeafMember("Unload"), Preserve]
        private void Unload()
        {
            Destroy(gameObject);
        }

        #endregion // Leaf

        #region Events

        private void Awake()
        {
            RegisterScriptObject();
        }

        private void Start()
        {
            if (m_StartDisabled)
                gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DeregisterScriptObject();
        }

        #endregion // Events

        #region Register/Deregister

        private void RegisterScriptObject()
        {
            if (Services.Script.TryRegisterObject(this))
            {
                if (m_ScriptComponents == null)
                    m_ScriptComponents = GetComponents<IScriptComponent>();

                for(int i = m_ScriptComponents.Length - 1; i >= 0; i--)
                    m_ScriptComponents[i].OnRegister(this);

                for(int i = m_ScriptComponents.Length - 1; i >= 0; i--) {
                    m_ScriptComponents[i].PostRegister();
                }
            }
        }

        private void DeregisterScriptObject()
        {
            if (Services.Script && Services.Script.TryDeregisterObject(this))
            {
                for(int i = m_ScriptComponents.Length - 1; i >= 0; i--)
                    m_ScriptComponents[i].OnDeregister(this);
            }
        }

        #endregion // Register/Deregister

        #region IPoolAllocHandler

        void IPoolAllocHandler.OnAlloc()
        {
            RegisterScriptObject();
        }

        void IPoolAllocHandler.OnFree()
        {
            DeregisterScriptObject();
            m_Locals?.Clear();
        }

        void IPoolConstructHandler.OnConstruct()
        {
            m_Pooled = true;
        }

        void IPoolConstructHandler.OnDestruct()
        {
        }

        #endregion // IPoolAllocHandler

        #if UNITY_EDITOR

        private void Reset() {
            if (m_Id.IsEmpty) {
                m_Id = Guid.NewGuid().ToString();
            }
        }

        [ContextMenu("New Id")]
        private void ResetId() {
            m_Id = Guid.NewGuid().ToString();
        }

        #endif // UNITY_EDITOR

        /// <summary>
        /// Returns the id to use for persisting a value for a ScriptObject.
        /// </summary>
        static public StringHash32 MapPersistenceId(ScriptObject inObject, string inKey = null)
        {
            var currentMap = Assets.Map(MapDB.LookupCurrentMap());
            using(PooledStringBuilder psb = PooledStringBuilder.Create())
            {
                psb.Builder.Append(currentMap.name).Append('.').Append(inObject.m_Id.Source());
                if (!string.IsNullOrEmpty(inKey))
                {
                    psb.Builder.Append('.').Append(inKey);
                }
                return new StringBuilderSlice(psb.Builder).Hash32();
            }
        }

        /// <summary>
        /// Returns the id to use for persisting a value for a ScriptObject.
        /// </summary>
        static public StringHash32 PersistenceId(ScriptObject inObject, string inKey = null)
        {
            if (!inObject.m_IsPersistent)
            {
                return MapPersistenceId(inObject, inKey);
            }
            
            using(PooledStringBuilder psb = PooledStringBuilder.Create())
            {
                psb.Builder.Append(inObject.m_Id.Source());
                if (!string.IsNullOrEmpty(inKey))
                {
                    psb.Builder.Append('.').Append(inKey);
                }
                return new StringBuilderSlice(psb.Builder).Hash32();
            }
        }

        static public ScriptThreadHandle Inspect(ScriptObject inObject)
        {
            Assert.NotNull(inObject);
            using(var table = TempVarTable.Alloc())
            {
                table.Set("objectId", inObject.m_Id.Hash());
                table.Set("objectGroupId", ScriptGroupId.GetGroup(inObject));
                return Services.Script.TriggerResponse(GameTriggers.InspectObject, null, inObject, table);
            }
        }

        static public ScriptThreadHandle Talk(ScriptObject inObject, StringHash32 inCharacterId)
        {
            Assert.NotNull(inObject);
            using(var table = TempVarTable.Alloc())
            {
                table.Set("objectId", inObject.m_Id.Hash());
                table.Set("objectGroupId", ScriptGroupId.GetGroup(inObject));
                return Services.Script.TriggerResponse(GameTriggers.Talk, inCharacterId, inObject, table);
            }
        }

        static public ScriptThreadHandle Interact(ScriptObject inObject, bool inbLocked = false, StringHash32 inTarget = default)
        {
            Assert.NotNull(inObject);
            using(var table = TempVarTable.Alloc())
            {
                table.Set("objectId", inObject.m_Id.Hash());
                table.Set("objectGroupId", ScriptGroupId.GetGroup(inObject));
                table.Set("locked", inbLocked);
                table.Set("target", inTarget);
                return Services.Script.TriggerResponse(GameTriggers.InteractObject, null, inObject, table);
            }
        }

        static public StringHash32 FindId(GameObject inObject, StringHash32 inDefault = default(StringHash32))
        {
            Assert.NotNull(inObject);
            ScriptObject obj = inObject.GetComponent<ScriptObject>();
            if (!obj.IsReferenceNull())
                return obj.m_Id;

            return inDefault;
        }

        static public StringHash32 FindId(Component inObject, StringHash32 inDefault = default(StringHash32))
        {
            Assert.NotNull(inObject);
            ScriptObject obj = inObject.GetComponent<ScriptObject>();
            if (!obj.IsReferenceNull())
                return obj.m_Id;

            return inDefault;
        }
    }
}