using System;
using System.Collections.Generic;
using BeauUtil;
using BeauUtil.Variants;
using FieldDay;
using FieldDay.Components;
using FieldDay.Scenes;
using FieldDay.Scripting;
using Leaf.Runtime;
using UnityEngine;

namespace WeatherStation.Scripting {
    public class ScriptObject : BatchedComponent, ILeafActor {
        #region Inspector

        [SerializeField] private SerializedHash32 m_Id;
        [SerializeField] private bool m_DisableOnStart;

        #endregion // Inspector

        [NonSerialized] private VariantTable m_Locals;

        #region ILeafActor

        public StringHash32 Id { get { return m_Id; } }

        public VariantTable Locals { get { return m_Locals ?? (m_Locals = new VariantTable()); } }

        #endregion // ILeafActor

        #region Leaf

        [LeafMember("Activate")]
        public void Activate() {
            gameObject.SetActive(true);
        }

        [LeafMember("Deactivate")]
        public void Deactivate() {
            gameObject.SetActive(false);
        }

        [LeafMember("ToggleActive")]
        public void ToggleActive() {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        [LeafMember("Unload")]
        public void Unload() {
            Destroy(gameObject);
        }

        #endregion // Leaf

        #region Unity Events

        private void Awake() {
            if (ScriptUtility.Runtime.AllActors.Add(this)) {
                if (!m_Id.IsEmpty) {
                    ScriptUtility.Runtime.NamedActors[m_Id] = this;
                }
            }
        }

        private void Start() {
            if (m_DisableOnStart) {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy() {
            if (Game.IsShuttingDown) {
                return;
            }

            if (ScriptUtility.Runtime.AllActors.Remove(this)) {
                if (!m_Id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(m_Id, out ILeafActor act) && act == this) {
                    ScriptUtility.Runtime.NamedActors.Remove(m_Id);
                }
            }
        }

#if UNITY_EDITOR

        private void Reset() {
            if (m_Id.IsEmpty) {
                m_Id = Guid.NewGuid().ToString();
            }
        }

#endif // UNITY_EDITOR

        #endregion // Unity Events
    }

    public interface IScriptActorComponent { }
}