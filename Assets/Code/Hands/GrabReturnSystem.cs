using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.LateFixedUpdate, 500)]
    public class GrabReturnSystem : ComponentSystemBehaviour<Grabbable> {
		
		public AudioSource GroundHitSound = null;	//temp - eventually different per object.
		
		static public bool ForceSkip = false;
		
        private float ReturnTime = 2f;

        public override void ProcessWorkForComponent(Grabbable component, float deltaTime) {
            if(component.HitGround && component.ReturnOnGroundHit) {
                //todo - some objects we don't want to be able to auto-return
				component.HitGround = false;
                StartCoroutine(WaitToReturn(component, ReturnTime));
            } else if(component.transform.position.y < -30f) {
				StartCoroutine(WaitToReturn(component, 0f));
			}
        }

        public void CollisionHitFloor(Collision c) {
            Grabbable g = c.gameObject.GetComponent<Grabbable>();
            if(g != null) {
                g.HitGround = true;
				if(GroundHitSound) {
					GroundHitSound.Play();
				}
            }
        }

        private IEnumerator WaitToReturn(Grabbable component, float duration) {
            
            yield return new WaitForSeconds(duration);
			
			if(!ForceSkip) {
				if(component.OriginalSocket) {
					if(component.TryGetComponent(out Socketable s)) {
						if(component.OriginalSocket.Current == null) {
							SocketUtility.TryAddToSocket(component.OriginalSocket, s, true);
						} else {
							GrabUtility.ReturnToOriginalSpawnPoint(component);
						}
					}
				} else {
					GrabUtility.ReturnToOriginalSpawnPoint(component);
				}
			}
			
			//ForceSkip = false;
        }
    }
}
