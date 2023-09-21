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
        
        private float ReturnTime = 2f;

        public override void ProcessWorkForComponent(Grabbable component, float deltaTime) {
            if(component.HitGround && component.ReturnOnGroundHit) {
                //todo - some objects we don't want to be able to auto-return
				component.HitGround = false;
                StartCoroutine(WaitToReturn(component, ReturnTime));
            }
        }

        public void CollisionHitFloor(Collision c) {
            Grabbable g = c.gameObject.GetComponent<Grabbable>();
            if(g != null) {
                g.HitGround = true;
            }
        }

        private IEnumerator WaitToReturn(Grabbable component, float duration) {
            
            yield return new WaitForSeconds(duration);

            if(component.OriginalSocket) {
                if(component.TryGetComponent(out Socketable s)) {
                    SocketUtility.TryAddToSocket(component.OriginalSocket, s, true);
                }
            } else {
                component.Rigidbody.isKinematic = component.WasKinematic;
                component.transform.position = component.OriginalPosition;
                component.transform.rotation = component.OriginalRotation;
                component.transform.SetParent(component.OriginalParent, true);
            }
        }
    }
}
