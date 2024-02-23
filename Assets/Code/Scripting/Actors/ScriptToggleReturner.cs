using Leaf.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace WeatherStation.Scripting {
    public class ScriptToggleReturner : ScriptComponent
    {
        private LayerMask OriginalLayer;

        private void Awake()
        {
            OriginalLayer = this.gameObject.layer;
        }

        [LeafMember("SetReturner"), Preserve]
        public void SetReturner(bool returnerParam)
        {
            if (returnerParam)
            {
                this.gameObject.layer = 1 << LayerMask.NameToLayer("Return");
            }
            else
            {
                this.gameObject.layer = OriginalLayer == 1 << LayerMask.NameToLayer("Return") ? 1 << LayerMask.NameToLayer("Default") : OriginalLayer;
            }
        }
    }
}