using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    public class SocketHighlightSystem : ComponentSystemBehaviour<Socketable> {
        public override void ProcessWorkForComponent(Socketable component, float deltaTime) {
            if (component.CurrentSocket != null) {
                return;
            }

            component.HighlightedSocket = GetClosest(component, component.PotentialSockets);
            // TODO: visual?
        }

        static private ItemSocket GetClosest(Socketable component, HashSet<ItemSocket> sockets) {
            Vector3 sourcePos = component.CachedTransform.position;
            ItemSocket closest = null;
            float closestDistSq = float.MaxValue;

            foreach(var socket in sockets) {
				if(socket.IsSocketAllowed(component.SocketType)) {
					Vector3 dist = socket.Location.position - sourcePos;
					if (dist.sqrMagnitude < closestDistSq) {
						closestDistSq = dist.sqrMagnitude;
						closest = socket;
					}
				}
            }

            return closest;
        }
    }
}
