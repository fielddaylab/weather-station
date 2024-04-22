using Leaf.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting
{
    public class ScriptResetRotation : ScriptComponent
    {
        private Quaternion OriginalRotation;

        private void Awake()
        {
            OriginalRotation = this.gameObject.transform.rotation;
        }

        [LeafMember("ResetRotation"), Preserve]
        public void ResetRotation()
        {
            this.gameObject.transform.rotation = OriginalRotation;
        }
    }
}