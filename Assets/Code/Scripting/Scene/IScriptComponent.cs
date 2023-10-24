using BeauUtil;
using UnityEngine;
using System.Collections;
using BeauUtil.Debugger;
using Leaf;

namespace WeatherStation
{
    public interface IScriptComponent
    {
        ScriptObject Parent { get; }
        void OnRegister(ScriptObject inObject);
        void PostRegister();
        void OnDeregister(ScriptObject inObject);
    }
}