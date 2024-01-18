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
using UnityEngine.Scripting;

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
		
		[LeafMember("SetTestSuccess"), Preserve]
		void SetTestSuccess(bool success) {
			m_Puzzle.SetTestSuccess(success);
		}
		
		public bool IsComplete() { return m_Puzzle.IsComplete(); }
		
		public bool IsAlmostComplete() { return m_Puzzle.IsAlmostComplete(); }

        [LeafMember("PuzzleIsNotComplete"), Preserve]
        static bool PuzzleIsNotComplete(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptPuzzle sp = ((ScriptObject)act).gameObject.GetComponent<ScriptPuzzle>();
				if(sp != null) {
					return !sp.IsComplete();
				}
			}
			
			return false;
		}
		
        [LeafMember("PuzzleIsAlmostComplete"), Preserve]
        static bool PuzzleIsAlmostComplete(StringHash32 id) {
			if (!id.IsEmpty && ScriptUtility.Runtime.NamedActors.TryGetValue(id, out ILeafActor act)) {
				ScriptPuzzle sp = ((ScriptObject)act).gameObject.GetComponent<ScriptPuzzle>();
				if(sp != null) {
					return sp.IsAlmostComplete();
				}
			}
			
			return false;
		}
        #endregion // Leaf
		
    }
}