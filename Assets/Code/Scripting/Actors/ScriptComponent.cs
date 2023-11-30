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

namespace WeatherStation.Scripting
{
    [RequireComponent(typeof(ScriptObject))]
    public abstract class ScriptComponent : MonoBehaviour, IScriptActorComponent
    {
        [NonSerialized] protected ScriptObject m_Parent;

        public ScriptObject Parent { get { return m_Parent; } }

        public virtual void OnDeregister(ScriptObject inObject) { m_Parent = null; }
        public virtual void OnRegister(ScriptObject inObject) { m_Parent = inObject; }
        public virtual void PostRegister() { }

        /*public ScriptThreadHandle Trigger(StringHash32 inTriggerId) {
            return ScriptUtility.Trigger(inTriggerId, null, m_Parent);
        }

        public ScriptThreadHandle Trigger(StringHash32 inTriggerId, TempVarTable inTable) {
            return ScriptUtility.Trigger(inTriggerId, null, m_Parent, inTable);
        }*/
    }
}