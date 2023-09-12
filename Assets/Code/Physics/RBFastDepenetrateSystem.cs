using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil;
using FieldDay;
using FieldDay.Components;
using FieldDay.Systems;
using UnityEngine;

namespace WeatherStation {
    [SysUpdate(GameLoopPhase.LateFixedUpdate)]
    public class RBFastDepenetrateSystem : ComponentSystemBehaviour<RBFastDepenetrate> {
        public override void ProcessWorkForComponent(RBFastDepenetrate component, float deltaTime) {
            float sep;
            float adjustedSep;
            Vector3 sepVector;
            Vector3 accumulatedSepVec = default;

            while(component.Contacts.TryPopFront(out ContactPoint contact)) {
                sep = contact.separation + CollisionUtility.ContactOverlapThreshold;
                sepVector = contact.normal;
                adjustedSep = sep + Vector3.Dot(accumulatedSepVec, sepVector);

                if (adjustedSep < 0) {
                    sepVector.x *= -adjustedSep;
                    sepVector.y *= -adjustedSep;
                    sepVector.z *= -adjustedSep;

                    accumulatedSepVec += sepVector;
                }
            }

            if (accumulatedSepVec.sqrMagnitude > 0) {
                component.CachedRB.position += accumulatedSepVec;
            }
        }
    }
}
