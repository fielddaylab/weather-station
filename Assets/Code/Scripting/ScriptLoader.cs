using Leaf;
using UnityEngine;

namespace FieldDay.Scripting {
    public sealed class ScriptLoader : MonoBehaviour {
        public LeafAsset[] Scripts;

        public void OnEnable() {
            ScriptDatabase db = Find.State<ScriptDatabase>();
            foreach(var script in Scripts) {
                ScriptDatabaseUtility.LoadNow(db, script);
            }
        }

        public void OnDisable() {
            if (Game.IsShuttingDown) {
                return;
            }

            ScriptDatabase db = Find.State<ScriptDatabase>();
            foreach (var script in Scripts) {
                ScriptDatabaseUtility.Unload(db, script);
            }
        }
    }
}