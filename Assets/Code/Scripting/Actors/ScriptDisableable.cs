using Leaf.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting
{
    public class ScriptDisableable : ScriptComponent
    {
        [LeafMember("SetDisabled"), Preserve]
        public void SetDisabled(bool disableParam)
        {
            this.gameObject.SetActive(!disableParam);
        }
    }
}
