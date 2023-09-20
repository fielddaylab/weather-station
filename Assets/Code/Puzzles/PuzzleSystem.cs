using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;
using BeauUtil;
using BeauUtil.Debugger;

namespace WeatherStation {
    //[SysUpdate(GameLoopPhase.LateUpdate)]
    public class PuzzleSystem : ComponentSystemBehaviour<Puzzle> {

        public override void ProcessWorkForComponent(Puzzle component, float deltaTime) {

            component.UpdatePuzzle();
            
            //Log.Msg("Processing work for: " + component.gameObject.name);
			if(component.IsComplete()) {
				//do something cool, advance game, etc.
			}
        }

    }
}
