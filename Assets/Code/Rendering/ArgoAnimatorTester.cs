using System;
using System.Collections;
using System.Diagnostics;
using BeauRoutine;
using BeauUtil;
using BeauUtil.Debugger;
using FieldDay;
using FieldDay.Debugging;
using Leaf.Runtime;
using UnityEngine;
using WeatherStation.Scripting;

public class ArgoAnimatorTester : MonoBehaviour {
    public ArgoAnimator Animator;
    public AudioClip TestClip;

    private void Start() {
        Routine.Start(this, TestRoutine());
    }

    private IEnumerator TestRoutine() {
        while (true) {

            yield return TestPose(default);

            for(int i = 0; i < Animator.NamedPoses.Length; i++) {
                yield return TestPose(Animator.NamedPoses[i].Id);
            }

            yield return TestPose("error_fallback");
        }
    }

    private IEnumerator TestPose(StringHash32 id) {
        Animator.SetPoseById(id);
        DebugDraw.AddWorldText(Animator.transform.position, id.IsEmpty ? "[default]" : id.ToDebugString(), Color.yellow, 2, TextAnchor.MiddleCenter, DebugTextStyle.BackgroundDarkOpaque);
        if (TestClip != null) {
            yield return 1;
            yield return PlayClip();
            yield return 0.5f;
        } else {
            yield return 4;
        }
    }

    private IEnumerator PlayClip() {
        Animator.Audio.Stop();
        Animator.Audio.clip = TestClip;
        Animator.Audio.Play();
        yield return TestClip.length;
        while(Animator.Audio.isPlaying) {
            yield return null;
        }
    }
}