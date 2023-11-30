using System;
using System.Collections;
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
	[RequireComponent(typeof(Puzzle))]
    public class ScriptPuzzle : ScriptComponent {
		
        #region Inspector
		
		
		#endregion // Inspector
		
        #region Leaf
		private Puzzle m_Puzzle=null;
        
        private void Awake() {
            m_Puzzle = GetComponent<Puzzle>();
        }
		
		public bool IsComplete() { return m_Puzzle.IsComplete(); }

        [LeafMember("PuzzleIsNotComplete")]
        static bool PuzzleIsComplete(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptPuzzle sp = ((ScriptObject)act).gameObject.GetComponent<ScriptPuzzle>();
				if(sp != null) {
					return !sp.IsComplete();
				}
			}
			
			return false;
		}
		
        #endregion // Leaf
		
    }
}