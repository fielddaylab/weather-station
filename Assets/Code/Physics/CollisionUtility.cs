using System;
using UnityEngine;

namespace WeatherStation {
    static public class CollisionUtility {
        static public Collider ClosestCollider(Transform target, Collider[] overlaps, int overlapCount) {
            Collider bestCollider = null;
            float bestDist = float.MaxValue;
            Collider check;

            Vector3 targetPos = target.position;

            for (int i = 0; i < overlapCount; i++) {
                check = overlaps[i];
                float distSq = (check.ClosestPoint(targetPos) - targetPos).sqrMagnitude;
                if (distSq < bestDist) {
                    bestCollider = check;
                    bestDist = distSq;
                }
            }

            return bestCollider;
        }

        static public TResult Closest<TResult>(Transform target, Collider[] overlaps, int overlapCount) where TResult : Component {
            TResult bestComponent = null;
            float bestDist = float.MaxValue;
            Collider check;
            TResult checkComponent;

            Vector3 targetPos = target.position;

            for (int i = 0; i < overlapCount; i++) {
                check = overlaps[i];
                if (check.TryGetComponent(out checkComponent)) {
                    float distSq = (check.ClosestPoint(targetPos) - targetPos).sqrMagnitude;
                    if (distSq < bestDist) {
                        bestComponent = checkComponent;
                        bestDist = distSq;
                    }
                }
            }

            return bestComponent;
        }

        static public TResult Closest<TResult>(Transform target, Predicate<TResult> predicate, Collider[] overlaps, int overlapCount) where TResult : Component {
            TResult bestComponent = null;
            float bestDist = float.MaxValue;
            Collider check;
            TResult checkComponent;

            Vector3 targetPos = target.position;

            for (int i = 0; i < overlapCount; i++) {
                check = overlaps[i];
                if (check.TryGetComponent(out checkComponent) && predicate(checkComponent)) {
                    float distSq = (check.ClosestPoint(targetPos) - targetPos).sqrMagnitude;
                    if (distSq < bestDist) {
                        bestComponent = checkComponent;
                        bestDist = distSq;
                    }
                }
            }

            return bestComponent;
        }

        static public TResult ClosestInParent<TResult>(Transform target, Collider[] overlaps, int overlapCount) where TResult : Component {
            TResult bestComponent = null;
            float bestDist = float.MaxValue;
            Collider check;
            TResult checkComponent;

            Vector3 targetPos = target.position;

            for (int i = 0; i < overlapCount; i++) {
                check = overlaps[i];
                if ((checkComponent = check.GetComponentInParent<TResult>()) != null) {
                    float distSq = (check.ClosestPoint(targetPos) - targetPos).sqrMagnitude;
                    if (distSq < bestDist) {
                        bestComponent = checkComponent;
                        bestDist = distSq;
                    }
                }
            }

            return bestComponent;
        }

        static public TResult ClosestInParent<TResult>(Transform target, Predicate<TResult> predicate, Collider[] overlaps, int overlapCount) where TResult : Component {
            TResult bestComponent = null;
            float bestDist = float.MaxValue;
            Collider check;
            TResult checkComponent;

            Vector3 targetPos = target.position;

            for (int i = 0; i < overlapCount; i++) {
                check = overlaps[i];
                if ((checkComponent = check.GetComponentInParent<TResult>()) != null && predicate(checkComponent)) {
                    float distSq = (check.ClosestPoint(targetPos) - targetPos).sqrMagnitude;
                    if (distSq < bestDist) {
                        bestComponent = checkComponent;
                        bestDist = distSq;
                    }
                }
            }

            return bestComponent;
        }
    }
}